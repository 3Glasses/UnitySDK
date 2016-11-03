using System.Runtime.InteropServices;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable UseStringInterpolation

/*
 * Head Module
 */

namespace ThreeGlasses
{
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("3Glasses/Headset")]
    public class ThreeGlassesHeadset : MonoBehaviour
    {
        private const int RenderWidth = 2880;
        private const int RenderHeight = 1440;

        [DllImport("SZVRUnityPlugin")]
        private static extern void SZVRPluginInit();

        [DllImport("SZVRUnityPlugin")]
        private static extern void SZVRPluginDestroy();

        [DllImport("SZVRUnityPlugin")]
        private static extern void SZVRPluginEnableATW();

        [DllImport("SZVRUnityPlugin")]
        private static extern void SZVRPluginDiasbleATW();

        [DllImport("SZVRUnityPlugin")]
        private static extern void GetHMDQuaternion(float[] input);

        [DllImport("SZVRUnityPlugin")]
        private static extern void UpdateTextureFromUnity(System.IntPtr leftIntPtr, System.IntPtr rigthIntPtr);

        [DllImport("SZVRUnityPlugin")]
        private static extern System.IntPtr GetRenderEventFunc();

        public bool EnableHeadRotTracking = true;
        public bool EnableHeadPosTracking = false;

        public float Near = 0.3f;
        public float Far = 1000f;

        public float EyeDistance = 0.1f;
        private const float FieldOfView = 90;

        public ThreeGlassesVRCamera leftCamera;
        public ThreeGlassesVRCamera rightCamera;

        private static RenderTexture _leftRenderTexture;
        private static RenderTexture _rightRenderTexture;

        private static bool[] eyeStatus = {false, false};
        private static bool upTexture;

        void Awake()
        {
            SZVRPluginInit();
        }

        void Start()
        {
            //Application.targetFrameRate = 60;

            SetCameraPos();
            StartCoroutine(ThreeGlassesUtils.DelayedRun(() =>
            {
                if (_leftRenderTexture != null && _rightRenderTexture != null) return;

                _leftRenderTexture = new RenderTexture(RenderWidth/2, RenderHeight, 24,
                    RenderTextureFormat.BGRA32,
                    RenderTextureReadWrite.Default);

                _rightRenderTexture = new RenderTexture(RenderWidth/2, RenderHeight, 24,
                    RenderTextureFormat.BGRA32,
                    RenderTextureReadWrite.Default);

                leftCamera.SetRenderTarget(_leftRenderTexture);
                rightCamera.SetRenderTarget(_rightRenderTexture);

                upTexture = false;
                
            }, new WaitForEndOfFrame()));
        }

        public void SetCameraPos()
        {
            var eyeDistance = new Vector3(EyeDistance/2, 0, 0);

            if (leftCamera == null || rightCamera == null) return;

            leftCamera.transform.localPosition = -eyeDistance;
            rightCamera.transform.localPosition = eyeDistance;

            if (leftCamera.cam == null || rightCamera.cam == null) return;

            leftCamera.cam.fieldOfView = FieldOfView;
            rightCamera.cam.fieldOfView = FieldOfView;

            leftCamera.LeftEye = true;
            rightCamera.LeftEye = false;

            leftCamera.cam.nearClipPlane = Near;
            leftCamera.cam.farClipPlane = Far;

            rightCamera.cam.nearClipPlane = Near;
            rightCamera.cam.farClipPlane = Far;
        }

        void OnEnable()
        {
            ThreeGlassesEvents.HeadPosEvent += UpdatePos;
        }

        void OnDisable()
        {
            ThreeGlassesEvents.HeadPosEvent -= UpdatePos;
        }

        void OnDestroy()
        {
            SZVRPluginDestroy();
        }

        void UpdatePos(Vector3 pos)
        {
            if (EnableHeadPosTracking)
            {
                transform.localPosition = pos;
            }
        }

        public void Update()
        {
            if (!EnableHeadRotTracking) return;
            var input = new float[] {0, 0, 0, 0};
            GetHMDQuaternion(input);
            input[0] = input[0];
            input[1] = input[1];
            input[2] = input[2];
            input[3] = input[3];
            transform.localRotation = new Quaternion(input[0], input[1], input[2], input[3]);
        }

        public void EnableATW()
        {
            SZVRPluginEnableATW();
        }

        public void DisableATW()
        {
            SZVRPluginDiasbleATW();
        }

        public static void Submit(bool lefteye)
        {
            if (lefteye)
            {
                eyeStatus[0] = true;
            }
            else
            {
                eyeStatus[1] = true;
            }

            if (!eyeStatus[0] || !eyeStatus[1]) return;
            if (!_leftRenderTexture.Create() ||
                !_rightRenderTexture.Create())
            {
                return;
            }

            if (!upTexture)
            {
                upTexture = true;
                UpdateTextureFromUnity(
                    _leftRenderTexture.GetNativeTexturePtr(),
                    _rightRenderTexture.GetNativeTexturePtr());
            }
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);

            eyeStatus[0] = eyeStatus[1] = false;
        }
    }
}
