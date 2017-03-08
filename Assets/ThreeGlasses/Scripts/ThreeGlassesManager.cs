using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
// ReSharper disable IteratorNeverReturns
// ReSharper disable InconsistentNaming

namespace ThreeGlasses
{
    public class ThreeGlassesManager : MonoBehaviour {
        // camera
        const int CAMERA_NUM = 2;
        private GameObject[] subCamera = new GameObject[CAMERA_NUM];
        private float near, far;
        private string[] cameraName = new string[]{"leftCamera", "rightCamera"};
        private Camera thisCam;
        private Camera[] subCameraCam = new Camera[CAMERA_NUM];
        private ThreeGlassesSubCamera[] subCameraScript = new ThreeGlassesSubCamera[CAMERA_NUM];
        public static Vector3 hmdPosition = new Vector3();
        public static Quaternion hmdRotation = new Quaternion();
		public bool freezePosition = false;
		public bool freezeRotation = false;

        [Range(1.0f, 4.0f)]
        public float scaleRenderResolution = 1.3f;
        public bool AsynchronousProjection = false;

        // RenderTexture
        private static RenderTexture[] renderTexture = new RenderTexture[CAMERA_NUM];
        public enum AntiAliasingLevel
        {
            Disabled = 1,
            Sample_2x = 2,
            Sample_4x = 4,
            Sample_8x = 8
        };
        public AntiAliasingLevel hmdAntiAliasingLevel = AntiAliasingLevel.Sample_2x;


        // eye's distance
        public float eyeDistance = 0.1f;
       
        public LayerMask layerMask = -1;

        // whether to active the wand
        public bool enableJoypad = true;

        const int JOYPAD_NUM = 2;
        public static ThreeGlassesWand[] joyPad = new ThreeGlassesWand[JOYPAD_NUM] { null, null};

        // maincamera can displayer
        public bool onlyHeadDisplay = false;

        // hmd button
        public const int HMD_BUTTON_MASK_MENU = 0x01;
        public const int HMD_BUTTON_MASK_EXIT = 0x02;

        private static int hmdKeyStatus = 0;
        //hmd touchpad
        private static Vector2 hmdTouchPad = Vector2.zero;

        static public string hmdName = "no name";
        System.IntPtr strPtr;


        void Awake()
        {
            ThreeGlassesHeadDisplayLife.scaleRenderSize = scaleRenderResolution;
            ThreeGlassesHeadDisplayLife.AsynchronousProjection =
                AsynchronousProjection;

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

            // check hmd status
            bool result = false;
            if (0 != ThreeGlassesDllInterface.SZVR_GetHMDConnectionStatus(ref result) || !result)
            {
                Debug.LogWarning("The Helmet Mounted Display is not connect");
            }

            // get hmd name
            strPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(64);
            if (0 != ThreeGlassesDllInterface.SZVR_GetHMDDevName(strPtr))
            {
                hmdName = Marshal.PtrToStringAnsi(strPtr, 64);
            }

            if (hmdName == null || hmdName.Length <= 0)
            {
                hmdName = "no name";
                Debug.LogWarning("can not get HMD's name");
            }
            
            // init RenderTexture
            if (renderTexture[0] == null && renderTexture[1] == null)
            {
                for (var i = 0; i < CAMERA_NUM; i++)
                {
                    renderTexture[i] = new RenderTexture(
                        (int)ThreeGlassesHeadDisplayLife.renderWidth / 2,
                        (int)ThreeGlassesHeadDisplayLife.renderHeight,
                        24,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Default);
                    renderTexture[i].antiAliasing = (int)hmdAntiAliasingLevel;
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
                subCameraScript[i].CameraType = (ThreeGlassesSubCamera.CameraTypes)i;
            }

            var eyeDis = new Vector3(eyeDistance/2, 0, 0);
            subCamera[0].transform.localPosition = -eyeDis;
            subCamera[1].transform.localPosition = eyeDis;

            // set the camera who bind ThreeGlassesSubCamera
            ThreeGlassesSubCamera[] cams = GameObject.FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            foreach(var cam in cams)
            {
                Camera tempCamera = cam.gameObject.GetComponent<Camera>();
                if (ThreeGlassesSubCamera.CameraTypes.Screen == cam.CameraType)
                {
                    tempCamera.targetTexture = null;
                    continue;
                }

                tempCamera.targetTexture = renderTexture[(int) cam.CameraType];
            }

            thisCam.enabled = !onlyHeadDisplay;
        }
        public void Pasue()
        {
            // stop render HMD
            StopAllCoroutines();

            // all subcamera render to screen not rendertexture
            ThreeGlassesSubCamera[] cams = GameObject.FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            foreach(var cam in cams)
            {
                Camera tempCamera = cam.gameObject.GetComponent<Camera>();
                if (ThreeGlassesSubCamera.CameraTypes.Screen != cam.CameraType)
                {
                    tempCamera.targetTexture = null;
                }
            }

            // destroy plugin
            ThreeGlassesDllInterface.SZVRPluginDestroy();

            // release rendtexture
            for (var i = 0; i < CAMERA_NUM; i++)
            {
                renderTexture[i].Release();
            }
        }

        public void Resume()
        {
            // init plugin
            ThreeGlassesDllInterface.SZVRPluginInit(
                (uint)(ThreeGlassesHeadDisplayLife.AsynchronousProjection ? 0 : 1),
                ThreeGlassesHeadDisplayLife.renderWidth,
                ThreeGlassesHeadDisplayLife.renderHeight);

            // create rendertexture
            for (var i = 0; i < CAMERA_NUM; i++)
            {
//                renderTexture[i] = new RenderTexture(
//                    (int)renderWidth / 2,
//                    (int)renderHeight,
//                    24,
//                    RenderTextureFormat.Default,
//                    RenderTextureReadWrite.Default);
//                renderTexture[i].antiAliasing = (int)hmdAntiAliasingLevel;
                renderTexture[i].Create();
            }

            // bind rendertexure
            ThreeGlassesSubCamera[] cams = GameObject.FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            foreach(var cam in cams)
            {
                Camera tempCamera = cam.gameObject.GetComponent<Camera>();
                if (ThreeGlassesSubCamera.CameraTypes.Screen == cam.CameraType)
                {
                    tempCamera.targetTexture = null;
                    continue;
                }
                tempCamera.targetTexture = renderTexture[(int) cam.CameraType];
            }

            // resume render HMD
            StopAllCoroutines();
            StartCoroutine(CallPluginAtEndOfFrames());
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

                UpdateHMD();

                UpdateWand();
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

        void UpdateHMD()
        {
            // update hmd
            float[] pos = {0, 0, 0};
            ThreeGlassesDllInterface.SZVR_GetHMDPos(pos);
            var hmdPosition = new Vector3(pos[0], pos[1], -pos[2])/1000f;
            if (!freezePosition && ThreeGlassesUtils.CheckNaN(hmdPosition))
            {
                thisCam.transform.localPosition = hmdPosition;
            }

            float[] rotate = { 0, 0, 0, 1 };
            ThreeGlassesDllInterface.SZVR_GetHMDRotate(rotate);
            hmdRotation = new Quaternion(rotate[0], rotate[1], -rotate[2], -rotate[3]);
            if (!freezeRotation)
            {
                thisCam.transform.localRotation = hmdRotation;
            }
            ThreeGlassesDllInterface.StereoRenderBegin();

            bool[] button = { false, false };
            ThreeGlassesDllInterface.SZVR_GetHMDMenuButton(ref button[0]);
            ThreeGlassesDllInterface.SZVR_GetHMDExitButton(ref button[1]);
            for (int i = 0; i < 2; i++)
            {
                if (button[i])
                {
                    hmdKeyStatus |= 1 << i;
                }
            }

            // touchpad
            byte[] touchPos = {0, 0};
            ThreeGlassesDllInterface.SZVR_GetHMDTouchpad(touchPos);
            hmdTouchPad[0] = ((touchPos[0] / (float)255.0) - 0.5f)*2.0f; 
            hmdTouchPad[1] = (-(touchPos[1] / (float)255.0) + 0.5f)*2.0f;
        }

        void UpdateWand()
        {
            // update wand info
            if (!enableJoypad) return;

            byte[] connect = { 0, 0 };
            if (0 == ThreeGlassesDllInterface.SZVR_GetWandConnectionStatus(connect))
            {
                if (connect[0] != 0 || connect[1] != 0)
                {
                    bool getRotate = false, getPos = false, getTrigger = false, getStick = false, getButton = false;
                    float[] wandRotate = { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f };
                    float[] wandPos = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
                    byte[] trigger = { 0, 0 };
                    byte[] stick = { 0, 0, 0, 0 };
                    byte[] wandButton = new byte[12];
                    if (0 == ThreeGlassesDllInterface.SZVR_GetWandRotate(wandRotate))
                    {
                        getRotate = true;
                    }
                    if (0 == ThreeGlassesDllInterface.SZVR_GetWandPos(wandPos))
                    {
                        getPos = true;
                    }
                    if (0 == ThreeGlassesDllInterface.SZVR_GetWandTriggerProcess(trigger))
                    {
                        getTrigger = true;
                    }
                    if (0 == ThreeGlassesDllInterface.SZVR_GetWandStick(stick))
                    {
                        ThreeGlassesUtils.Log("lwand="+ stick[0]+ "    "+stick[1]);
                        ThreeGlassesUtils.Log("rwand="+ stick[2]+ "    "+stick[3]);
                        getStick = true;
                    }
                    if (0 == ThreeGlassesDllInterface.SZVR_GetWandButton(wandButton))
                    {
                        getButton = true;
                    }

                    for (var i = 0; i < JOYPAD_NUM; i++)
                    {
                        if (connect[i] != 0)
                        {
                            if (getRotate)
                            {
                                joyPad[i].UpdateRotate(wandRotate);
                            }
                            if (getPos)
                            {
                                joyPad[i].UpdatePos(wandPos);
                            }
                            if (getTrigger)
                            {
                                joyPad[i].UpdateTrigger(trigger);
                            }
                            if (getStick)
                            {
                                joyPad[i].UpdateStick(stick);
                            }
                            if (getButton)
                            {
                                joyPad[i].UpdateButton(wandButton);
                            }
                        }
                    }
                }
            }
        }

        static public bool GetHmdKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.HmdMenu:
                    return (hmdKeyStatus & HMD_BUTTON_MASK_MENU) != 0;
                case InputKey.HmdExit:
                    return (hmdKeyStatus & HMD_BUTTON_MASK_EXIT) != 0;
            }
            return false;
        }
        static public Vector2 GetHmdTouchPad()
        {
            return hmdTouchPad;
        }
    
        void OnDestroy()
        {
            for (var i = 0; i < CAMERA_NUM; i++)
            {
                if (renderTexture[i] != null)
                {
                    renderTexture[i].Release();
                    renderTexture[i] = null;   
                }
            }
            System.Runtime.InteropServices.Marshal.FreeHGlobal(strPtr);
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
			
		// no wear headdisplay
        public static bool GetHMDPresent()
        {
            bool status = false;
            if (0 != ThreeGlassesDllInterface.SZVR_GetHMDPresent(ref status))
            {
                status = false;
            }
            return status;
        }
			
    }
}
