using UnityEngine;
using System.Collections;
using System.Reflection;
// ReSharper disable IteratorNeverReturns
// ReSharper disable InconsistentNaming

namespace ThreeGlasses
{
    public class ThreeGlassesManager : MonoBehaviour {
        // camera
        const int CAMERA_NUM = 2;
        private GameObject[] subCamera = new GameObject[CAMERA_NUM];
        private float near, far;
        private float fieldOfView = 90;
        private string[] cameraName = new string[]{"leftCamera", "rightCamera"};
        private Camera thisCam;
        private Camera[] subCameraCam = new Camera[CAMERA_NUM];
        private ThreeGlassesSubCamera[] subCameraScript = new ThreeGlassesSubCamera[CAMERA_NUM];
        public static Vector3 headDisplayPosition = new Vector3();
        public static Quaternion headDisplayRotation = new Quaternion();
        public bool alignHeadDisplay = true;

        // RenderTexture
        private static RenderTexture[] renderTexture = new RenderTexture[CAMERA_NUM];
        private uint renderWidth = 2880;
        private uint renderHeight = 1440;

        // eye's distance
        public float eyeDistance = 0.1f;
       
        public LayerMask layerMask = -1;

        // whether to active the wand
        public bool enableJoypad = true;

        const int JOYPAD_NUM = 2;
        public static ThreeGlassesWand[] joyPad = new ThreeGlassesWand[JOYPAD_NUM] { null, null};

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
        }

        void Start ()
        {
            ThreeGlassesUtils.Log("MainCamera init");
            
            // init RenderTexture
            if (renderTexture[0] == null && renderTexture[1] == null)
            {
                uint[] buffsize = { renderWidth, renderHeight };
                ThreeGlassesDllInterface.GetRenderSize(buffsize);
                renderWidth = buffsize[0];
                renderHeight = buffsize[1];

                for (var i = 0; i < CAMERA_NUM; i++)
                {
                    renderTexture[i] = new RenderTexture((int)renderWidth / 2,
                        (int)renderHeight, 24,
                        RenderTextureFormat.ARGB32,
                        RenderTextureReadWrite.Default);
                    renderTexture[i].Create();
                }
            }

            // init camera
            VRCameraInit();

            if (enableJoypad)
            {
                // init wand
                ThreeGlassesUtils.Log("init joypad");
                joyPad[0] = new ThreeGlassesWand(InputType.LeftWand);
                joyPad[1] = new ThreeGlassesWand(InputType.RightWand);
            }

            StopAllCoroutines();
            StartCoroutine(CallPluginAtEndOfFrames());
        }

        void VRCameraInit ()
        {
            // get maincamera's nearClip and farClip
            thisCam = GetComponent<Camera>();
            near = thisCam.nearClipPlane;
            far = thisCam.farClipPlane;
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
                        MemberInfo meth = t.GetMethod("OnRenderImage",
                            BindingFlags.Instance |
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
                subCameraCam[i] = subCamera[i].AddComponent<Camera>();
                subCameraCam[i].fieldOfView = fieldOfView;
                subCameraCam[i].nearClipPlane = near;
                subCameraCam[i].farClipPlane = far;
                subCameraCam[i].cullingMask = layerMask;
                subCameraCam[i].depth = thisCam.depth;
                subCamera[i].transform.SetParent(this.transform);

                // add the components
                foreach(var item in needAdd)
                {
                    ThreeGlassesUtils.CopyComponent((Component)item, subCamera[i]);
                }

                // add subCamera script after add all component
                subCameraScript[i] = subCamera[i].AddComponent<ThreeGlassesSubCamera>();
                subCameraScript[i].type = (ThreeGlassesSubCamera.CameraType)i;
                
                subCameraScript[i].Flip = !flipDisplay;
                
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

            thisCam.enabled = !onlyHeadDisplay;
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
                var hmd = new float[] { 0, 0, 0, 0, 0, 0, 1};
                float[] wand_left = new float[] { 0, 0, 0, 0, 0, 0, 1 };
                float[] wand_right = new float[] { 0, 0, 0, 0, 0, 0, 1 };
                ThreeGlassesDllInterface.GetTrackedPost(hmd, wand_left, wand_right);
                
                headDisplayPosition = new Vector3(-hmd[0] / 1000.0f, hmd[1] / 1000.0f, -hmd[2] / 1000.0f);
                headDisplayRotation = new Quaternion(hmd[3], hmd[4], -hmd[5], -hmd[6]);

                thisCam.transform.localPosition = headDisplayPosition;
                thisCam.transform.localRotation = headDisplayRotation;


                // update wand info
                if (!enableJoypad) continue;
                joyPad[0].pack.position = new Vector3(
                    wand_left[0],
                    wand_left[1],
                    wand_left[2]) / 700.0f;
                joyPad[0].pack.rotation = new Quaternion(wand_left[3], wand_left[5], -wand_left[4], -wand_left[6]);
                joyPad[1].pack.position = new Vector3(
                    wand_right[0],
                    wand_right[1],
                    wand_right[2]) / 700.0f;
                joyPad[1].pack.rotation = new Quaternion(wand_right[3], wand_right[5],-wand_right[4], -wand_right[6]);
                for (var i = 0; i < JOYPAD_NUM; i++)
                {
                    joyPad[i].Update();
                }
            }
        }

        void Update()
        {
            for (int i = 0; i < CAMERA_NUM; i++)
            {
                subCameraScript[i].Flip = !flipDisplay;
                subCameraCam[i].cullingMask = layerMask;
            }
            
            // update eyedistance
            var eyeDis = new Vector3(eyeDistance / 2, 0, 0);
            subCamera[0].transform.localPosition = -eyeDis;
            subCamera[1].transform.localPosition = eyeDis;

            thisCam.enabled = !onlyHeadDisplay;
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
