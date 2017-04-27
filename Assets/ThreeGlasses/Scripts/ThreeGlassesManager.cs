using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable IteratorNeverReturns
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable UnassignedField.Global

namespace ThreeGlasses
{
    public class ThreeGlassesManager : MonoBehaviour {
        // camera
        const int CAMERA_NUM = 2;
        private GameObject[] subCamera = new GameObject[CAMERA_NUM];
        private float near, far;
        private string[] cameraName = {"leftCamera", "rightCamera"};
        private Camera[] subCameraCam = new Camera[CAMERA_NUM];
        private ThreeGlassesSubCamera[] subCameraScript = new ThreeGlassesSubCamera[CAMERA_NUM];
        public static Vector3 hmdPosition;
        public static Quaternion hmdRotation;

        public Camera cloneTargetCamera;
        public bool bindTargetCamera = true;
        private bool _bindTargetCamera = true;

        public bool freezePosition = false;
		public bool freezeRotation = false;

        [Range(1.0f, 4.0f)]
        public float scaleRenderResolution = 1.3f;
        public bool AsynchronousProjection = false;
        public bool useUnityStereoRendering = false;

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

        public bool CopyOnRenderImageComponent = true;

        public bool CopyOnPreRenderComponent = true;

        public bool CopyOnPostRenderComponent = true;

        // whether to active the wand
        public bool enableJoypad = true;

        const int JOYPAD_NUM = 2;
        public static ThreeGlassesWand[] joyPad = { null, null};

        // hmd button
        public const int HMD_BUTTON_MASK_MENU = 0x01;
        public const int HMD_BUTTON_MASK_EXIT = 0x02;

        private static int hmdKeyStatus;
        //hmd touchpad
        private static Vector2 hmdTouchPad = Vector2.zero;

        public static string hmdName = "no name";
        IntPtr strPtr;


        void Awake()
        {
            ThreeGlassesHeadDisplayLife.scaleRenderSize = scaleRenderResolution;
            ThreeGlassesHeadDisplayLife.AsynchronousProjection =
                AsynchronousProjection;

            // create life manager object
            if (FindObjectOfType(typeof(ThreeGlassesHeadDisplayLife)) == null)
            {
                var life = new GameObject("ThreeGlassesHeadDisplayLife");
                life.AddComponent<ThreeGlassesHeadDisplayLife>();
                DontDestroyOnLoad(life);
            }

            // lock cursor
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // block runtime settings
            _bindTargetCamera = bindTargetCamera;
        }

        void Start ()
        {
            ThreeGlassesUtils.Log("MainCamera init");

            // check hmd status
            var result = false;
            if (0 != ThreeGlassesDllInterface.SZVR_GetHMDConnectionStatus(ref result) || !result)
            {
                Debug.LogWarning("The Helmet Mounted Display is not connect");
            }

            // get hmd name
            strPtr = Marshal.AllocHGlobal(64);
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
            //Check if the camera is a parent object
            var isParent = false;
            var parentCamera = gameObject.GetComponentsInParent<Camera>();
            foreach (var p_camera in parentCamera)
            {
                if (p_camera.GetInstanceID()
                    != cloneTargetCamera.GetInstanceID()) continue;
                isParent = true;
                break;
            }

            if (!isParent && _bindTargetCamera)
            {
                Debug.LogError("Clone target is not parent object, VR Camera can't rotate");
            }

            // get maincamera's nearClip and farClip
            near = cloneTargetCamera.nearClipPlane;
            far = cloneTargetCamera.farClipPlane;

            // get components
            var needAdd = new ArrayList();
            var needAddTypes = new[] { typeof(GUILayer), typeof(FlareLayer)};
            var coms = gameObject.GetComponents<Component>();
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
                if (!(com is MonoBehaviour)) continue;
                if (com == this) continue;
                var t = com.GetType();
                MemberInfo methORI = t.GetMethod("OnRenderImage",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.Public);
                MemberInfo methOPreR = t.GetMethod("OnPreRender",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.Public);
                MemberInfo methOPostR = t.GetMethod("OnPostRender",
                    BindingFlags.Instance |
                    BindingFlags.NonPublic | BindingFlags.Public);

                if ((CopyOnRenderImageComponent && methORI != null) ||
                    (CopyOnPreRenderComponent && methOPreR != null) ||
                    (CopyOnPostRenderComponent && methOPostR != null))
                {
                    needAdd.Add(com);
                }
            }

            // create and set camera
            for (var i=0; i<CAMERA_NUM; i++)
            {
                // create camera
                subCamera[i] = new GameObject();

                // rename，add ThreeGlassesSubCamera
                subCamera[i].name = cameraName[i];
                subCameraCam[i] = subCamera[i].AddComponent<Camera>();
                subCameraCam[i].nearClipPlane = near;
                subCameraCam[i].farClipPlane = far;
                subCameraCam[i].cullingMask = layerMask;
                subCameraCam[i].depth = cloneTargetCamera.depth;

#if UNITY_5_6_OR_NEWER
                subCameraCam[i].allowHDR = cloneTargetCamera.allowHDR;
                subCameraCam[i].allowMSAA = cloneTargetCamera.allowMSAA;
#endif
                subCamera[i].transform.SetParent(transform);
                subCamera[i].transform.rotation = Quaternion.identity;
                subCamera[i].transform.localRotation = Quaternion.identity;

                // add the components
                foreach (var item in needAdd)
                {
                    ThreeGlassesUtils.CopyComponent((Component)item, subCamera[i]);
                }

                // add subCamera script after add all component
                subCameraScript[i] = subCamera[i].AddComponent<ThreeGlassesSubCamera>();
                subCameraScript[i].CameraType = (ThreeGlassesSubCamera.CameraTypes)i;
            }


            if (useUnityStereoRendering)
            {
                subCameraCam[0].stereoSeparation = cloneTargetCamera.stereoSeparation;
                subCameraCam[1].stereoSeparation = cloneTargetCamera.stereoSeparation;

                subCameraCam[0].stereoConvergence = cloneTargetCamera.stereoConvergence;
                subCameraCam[1].stereoConvergence = cloneTargetCamera.stereoConvergence;

                subCameraCam[0].stereoTargetEye = StereoTargetEyeMask.Left;
                subCameraCam[1].stereoTargetEye = StereoTargetEyeMask.Right;

                subCamera[0].transform.localPosition = Vector3.zero;
                subCamera[1].transform.localPosition = Vector3.zero;

#if UNITY_EDITOR
                // Check Player Setting
                if (!PlayerSettings.virtualRealitySupported)
                {
                    Debug.LogError("Don't enable virtual reality supported");
                    if (!PlayerSettings.singlePassStereoRendering)
                    {
                        Debug.LogError("Don't enable single pass");
                    }
                }
#endif
            }
            else
            {
                var eyeDis = new Vector3(eyeDistance / 2, 0, 0);
                subCamera[0].transform.localPosition = -eyeDis;
                subCamera[1].transform.localPosition = eyeDis;
            }

            // set the camera who bind ThreeGlassesSubCamera
            var cams = FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            if (cams == null) return;
            foreach(var cam in cams)
            {
                var tempCamera = cam.gameObject.GetComponent<Camera>();
                if (ThreeGlassesSubCamera.CameraTypes.Screen == cam.CameraType)
                {
                    tempCamera.targetTexture = null;
                    continue;
                }

                tempCamera.targetTexture = renderTexture[(int) cam.CameraType];
            }
        }

        public void Pasue()
        {
            // stop render HMD
            StopAllCoroutines();

            // all subcamera render to screen not rendertexture
            var cams = FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            if (cams != null)
                foreach(var cam in cams)
                {
                    var tempCamera = cam.gameObject.GetComponent<Camera>();
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
                renderTexture[i].Create();
            }

            // bind rendertexure
            var cams = FindObjectsOfType(typeof(ThreeGlassesSubCamera)) as ThreeGlassesSubCamera[];
            if (cams != null)
                foreach(var cam in cams)
                {
                    var tempCamera = cam.gameObject.GetComponent<Camera>();
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
            for (var i = 0; i < CAMERA_NUM; i++)
            {
                subCameraCam[i].cullingMask = layerMask;
            }

            // update eyedistance
            if (useUnityStereoRendering)
            {
                subCamera[0].transform.localPosition = Vector3.zero;
                subCamera[1].transform.localPosition = Vector3.zero;
            }
            else
            {
                var eyeDis = new Vector3(eyeDistance / 2, 0, 0);
                subCamera[0].transform.localPosition = -eyeDis;
                subCamera[1].transform.localPosition = eyeDis;
            }
        }

        void UpdateHMD()
        {
            // update hmd
            float[] pos = {0, 0, 0};
            ThreeGlassesDllInterface.SZVR_GetHMDPos(pos);
            hmdPosition = new Vector3(pos[0], pos[1], -pos[2])/1000f;
            if (!freezePosition && ThreeGlassesUtils.CheckNaN(hmdPosition))
            {
                if (_bindTargetCamera)
                {
                    cloneTargetCamera.transform.localPosition = hmdPosition;
                }
                else
                {
                    transform.localPosition = hmdPosition;
                }
            }

            float[] rotate = { 0, 0, 0, 1 };
            ThreeGlassesDllInterface.SZVR_GetHMDRotate(rotate);
            hmdRotation = new Quaternion(rotate[0], rotate[1], -rotate[2], -rotate[3]);
            if (!freezeRotation)
            {
                if (_bindTargetCamera)
                {
                    cloneTargetCamera.transform.localRotation = hmdRotation;
                }
                else
                {
                    transform.localRotation = hmdRotation;
                }
            }
            ThreeGlassesDllInterface.StereoRenderBegin();

            bool[] button = { false, false };
            ThreeGlassesDllInterface.SZVR_GetHMDMenuButton(ref button[0]);
            ThreeGlassesDllInterface.SZVR_GetHMDExitButton(ref button[1]);
            for (var i = 0; i < 2; i++)
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
                    byte[] trigger = { 128, 128 };
                    byte[] stick = { 128, 128, 128, 128 };
                    var wandButton = new byte[12];
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
                        if (connect[i] == 0) continue;
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

        public static bool GetHmdKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.HmdMenu:
                    return (hmdKeyStatus & HMD_BUTTON_MASK_MENU) != 0;
                case InputKey.HmdExit:
                    return (hmdKeyStatus & HMD_BUTTON_MASK_EXIT) != 0;
                case InputKey.WandMenu:
                    break;
                case InputKey.WandBack:
                    break;
                case InputKey.WandLeftSide:
                    break;
                case InputKey.WandRightSide:
                    break;
                case InputKey.WandTriggerWeak:
                    break;
            }
            return false;
        }
        public static Vector2 GetHmdTouchPad()
        {
            return hmdTouchPad;
        }
    
        void OnDestroy()
        {
            for (var i = 0; i < CAMERA_NUM; i++)
            {
                if (renderTexture[i] == null) continue;
                renderTexture[i].Release();
                renderTexture[i] = null;
            }
            Marshal.FreeHGlobal(strPtr);
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
            var status = false;
            if (0 != ThreeGlassesDllInterface.SZVR_GetHMDPresent(ref status))
            {
                status = false;
            }
            Debug.Log(status);
            return status;
        }
			
    }
}
