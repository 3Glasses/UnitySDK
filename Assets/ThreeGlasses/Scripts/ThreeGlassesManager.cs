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
        private static Camera thisCam;
        private Camera[] subCameraCam = new Camera[CAMERA_NUM];
        private ThreeGlassesSubCamera[] subCameraScript = new ThreeGlassesSubCamera[CAMERA_NUM];
        public static Vector3 headDisplayPosition = new Vector3();
        public static Quaternion headDisplayRotation = new Quaternion();
		public bool freezePosition = false;
		public bool freezeRotation = false;

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

        void Awake()
        {
            // create life manager object
            if (GameObject.FindObjectOfType(typeof(ThreeGlassesHeadDisplayLife)) == null)
            {
                GameObject life = new GameObject("ThreeGlassesHeadDisplayLife");
                life.AddComponent<ThreeGlassesHeadDisplayLife>();
                GameObject.DontDestroyOnLoad(life);
            }

            // lock cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
                    renderTexture[i] = new RenderTexture(
                        (int)renderWidth / 2,
                        (int)renderHeight,
                        24,
                        RenderTextureFormat.Default,
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

                tempCamera.targetTexture = renderTexture[(int) cam.type];
            }

            thisCam.enabled = !onlyHeadDisplay;
        }
        
        private IEnumerator CallPluginAtEndOfFrames()
        {
            while (true)
            {
                // after OnRenderImage
                yield return new WaitForEndOfFrame();
                
                ThreeGlassesDllInterface.UpdateTextureFromUnity(
                    renderTexture[0].GetNativeTexturePtr(),
                    renderTexture[1].GetNativeTexturePtr());

                GL.IssuePluginEvent(
                    ThreeGlassesDllInterface.GetRenderEventFunc(), 1);

                // update headdisplay position and rotation
                float x = 0, y = 0, z = 0, w = 1;
                ThreeGlassesDllInterface.szvrGetHmdPostionWithVector(ref x, ref y, ref z);
                var hmdPosition = new Vector3(x, y, -z)/1000f;
				if (!freezePosition && ThreeGlassesUtils.CheckNaN(hmdPosition))
                {
                    thisCam.transform.localPosition = headDisplayPosition = hmdPosition;
                }
                
                ThreeGlassesDllInterface.szvrGetHmdOrientationWithQuat(ref x, ref y, ref z, ref w);
                headDisplayRotation = new Quaternion(x, y, -z, -w);
                if (!freezeRotation)
				{
					thisCam.transform.localRotation = headDisplayRotation;	
				}

                // // update wand info
                if (!enableJoypad) continue;
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
                subCameraCam[i].cullingMask = layerMask;
            }
            
            // update eyedistance
            var eyeDis = new Vector3(eyeDistance / 2, 0, 0);
            subCamera[0].transform.localPosition = -eyeDis;
            subCamera[1].transform.localPosition = eyeDis;

            thisCam.enabled = !onlyHeadDisplay;
        }

        void OnDestroy()
        {
            for (var i = 0; i < CAMERA_NUM; i++)
            {
                renderTexture[i].Release();
                renderTexture[i] = null;
            }
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
			
		public static Transform GetHeadDisplayTransform()
		{
			return thisCam.transform;
		}
		// no wear headdisplay
        public static bool GetHMDPresent()
        {
            uint[] status = { 0 };
            ThreeGlassesDllInterface.GetHMDPresent(status);
            return status[0] != 1;
        }
			
    }
}
