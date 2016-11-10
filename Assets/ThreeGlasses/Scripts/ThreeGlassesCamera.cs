using UnityEngine;
using System.Collections;
using System.Reflection;

namespace ThreeGlasses
{
    public class ThreeGlassesCamera : MonoBehaviour {
        // camera
        const int CAMERA_NUM = 2;
        private GameObject[] subCamera = new GameObject[CAMERA_NUM];
        private float near, far;
        private float fieldOfView = 90;
        private string[] cameraName = new string[]{"leftCamera", "rightCamera"};
        
        // RenderTexture
        private static RenderTexture[] renderTexture = new RenderTexture[CAMERA_NUM];
        private const int renderWidth = 2880;
        private const int renderHeight = 1440;
        // eye's distance
        public float eyeDistance = 0.1f;
       
        public LayerMask layerMask = -1;

        // whether to active the wand
        public bool enableJoypad = true;
        const int JOYPAD_NUM = 2;
        public static ThreeGlassesWand[] joyPad = new ThreeGlassesWand[JOYPAD_NUM];

        // maincamera can displayer
        public bool onlyHeadDisplay = false;

        // when display upside down use it
        public bool flipDisplay = false;

        void Awake()
        {
            // create life manager object
            if(GameObject.FindObjectOfType(typeof(ThreeGlassesHeadDisplayLife)) == null)
            {
                GameObject life = new GameObject("ThreeGlassesHeadDisplayLife");
                life.AddComponent<ThreeGlassesHeadDisplayLife>();
                GameObject.DontDestroyOnLoad(life);
            }
            

            // init RenderTexture
            for (int i = 0; i < CAMERA_NUM; i++)
            {
                renderTexture[i] = new RenderTexture(renderWidth / 2, renderHeight, 24,
                                                     RenderTextureFormat.BGRA32,
                                                     RenderTextureReadWrite.Default);
                renderTexture[i].Create();
            }            
        }
        IEnumerator Start ()
        {
            ThreeGlassesUtils.Log("MainCamera init");
            // init camera
            VRCameraInit();
            if (enableJoypad)
            {
                // init wand
                ThreeGlassesUtils.Log("init joypad");
                joyPad[0] = new ThreeGlassesWand(InputType.LeftWand);
                joyPad[1] = new ThreeGlassesWand(InputType.RightWand);
            }

            yield return StartCoroutine("CallPluginAtEndOfFrames");
        }

        public void Update()
        {
            
        }
        
        void VRCameraInit ()
        {
            // get maincamera's nearClip and farClip
            Camera thiscam = GetComponent<Camera>();
            near = thiscam.nearClipPlane;
            far = thiscam.farClipPlane;
            fieldOfView = (float)(ThreeGlassesDllInterface.SZVRPluginGetFOV());

            // get components
            ArrayList needAdd = new ArrayList();
            System.Type[] needAddTypes = new System.Type[] { typeof(GUILayer), typeof(FlareLayer)};
            Component[] coms = gameObject.GetComponents<Component>();
            // non-script component
            foreach (var com in coms)
            {
                foreach(var type in needAddTypes)
                {
                    if(com.GetType() == type)
                    {
                        needAdd.Add(com);
                    }
                }
            }
            // script component
            foreach (var com in coms)
            {
               
                if (com is MonoBehaviour)
                {
                        
                    if (com != this)
                    {
                        System.Type t = com.GetType();


                        MemberInfo meth = t.GetMethod("OnRenderImage", BindingFlags.Instance |
                                                      BindingFlags.NonPublic | BindingFlags.Public);
                        if (meth != null)
                        {
                            needAdd.Add(com);
                        }
                    }
                      
                }
                
            }

            // create and set camera
            for (int i=0; i<CAMERA_NUM; i++)
            {
                // create camera
                subCamera[i] = new GameObject();
                
                // rename，add ThreeGlassesSubCamera
                subCamera[i].name = cameraName[i];
                Camera cam = subCamera[i].AddComponent<Camera>();
                cam.fieldOfView = fieldOfView;
                cam.nearClipPlane = near;
                cam.farClipPlane = far;
                cam.cullingMask = thiscam.cullingMask;
                cam.depth = thiscam.depth;
                subCamera[i].transform.SetParent(this.transform);

                // add the components
                foreach(var item in needAdd)
                {
                    ThreeGlassesUtils.CopyComponent((Component)item, subCamera[i]);
                }

                // add subCamera script after add all component
                ThreeGlassesSubCamera subCameraScript = subCamera[i].AddComponent<ThreeGlassesSubCamera>();
                subCameraScript.type = (ThreeGlassesSubCamera.CameraType)i;
                if(!flipDisplay)
                {
                    subCameraScript.FLIP = true;
                }
                

            }
            var eyeDis = new Vector3(eyeDistance/2, 0, 0);
            subCamera[0].transform.localPosition = -eyeDis;
            subCamera[1].transform.localPosition = eyeDis;

            // set the camera who bind ThreeGlassesSubCamera
            ThreeGlassesSubCamera[] cams = GameObject.FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            foreach(var cam in cams)
            {
                Camera tempCamera = cam.gameObject.GetComponent<Camera>();
                if (ThreeGlassesSubCamera.CameraType.Screen == cam.type)
                {
                    tempCamera.targetTexture = null;
                    continue;
                }

                tempCamera.targetTexture = renderTexture[(int)cam.type];
            }

            thiscam.enabled = !onlyHeadDisplay;
        }
        
        private IEnumerator CallPluginAtEndOfFrames()
        {
            while (true)
            {
                // after OnRenderImage
                yield return new WaitForEndOfFrame();

                ThreeGlassesDllInterface.UpdateTextureFromUnity(renderTexture[0].GetNativeTexturePtr(),
                                                                renderTexture[1].GetNativeTexturePtr());

                GL.IssuePluginEvent(ThreeGlassesDllInterface.GetRenderEventFunc(), 1);


                // update headdisplay position and rotation
                var hmd = new float[] { 0, 0, 0, 0, 0, 0, 0 };
                float[] wand_left = new float[] { 0, 0, 0, 0, 0, 0, 0 };
                float[] wand_right = new float[] { 0, 0, 0, 0, 0, 0, 0 };
                ThreeGlassesDllInterface.GetTrackedPost(hmd, wand_left, wand_right);
                transform.localPosition = new Vector3(hmd[0], hmd[1], hmd[2]);
                transform.localRotation = new Quaternion(hmd[5], hmd[4], hmd[3], -hmd[6]);


                // update wand info
                if (enableJoypad)
                {
                    joyPad[0].pack.position = new Vector3(wand_left[0], wand_left[1], wand_left[2]);
                    joyPad[0].pack.rotation = new Quaternion(wand_left[5], wand_left[4], wand_left[3], -wand_left[6]);
                    joyPad[1].pack.position = new Vector3(wand_right[0], wand_right[1], wand_right[2]);
                    joyPad[1].pack.rotation = new Quaternion(wand_right[5], wand_right[4], wand_right[3], -wand_right[6]);
                    for (int i = 0; i < JOYPAD_NUM; i++)
                    {
                        joyPad[i].Update();
                    }
                }
            }
        }

        void OnDestroy()
        {
        
        }

        // get
        public RenderTexture LeftEyeRT
        {
            get { return renderTexture[0]; }
        }
        public RenderTexture RightEyeRT
        {
            get { return renderTexture[1]; }
        }

    }
}
