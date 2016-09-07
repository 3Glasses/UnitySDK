using System.Runtime.InteropServices;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local

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


        [DllImport("SZVRCompositorPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "UpdateTextureFromUnity")]
        private static extern void UpdateTextureFromUnity(System.IntPtr texture);

        [DllImport("SZVRCompositorPlugin", CallingConvention = CallingConvention.StdCall, EntryPoint = "GetRenderEventFunc")]
        private static extern System.IntPtr GetRenderEventFunc();

        public bool EnableHeadRotTracking = true;
        public bool EnableHeadPosTracking = false;

        public float Near = 0.01f;
        public float Far = 1000f;

        public float EyeDistance = 0.1f;
        public float FieldOfView = 90f;

        public ThreeGlassesVRCamera leftCamera;
        public ThreeGlassesVRCamera rightCamera;

        private static RenderTexture _leftRenderTexture;
        private static RenderTexture _rightRenderTexture;

        private static RenderTexture _outRenderTexture;

        public RenderTexture Out;

        private static bool[] eyeStatus = { false, false };

        void Awake()
        {
            SetCameraPos();
        }

        public void SetCameraPos()
        {
            var eyeDistance = new Vector3(EyeDistance / 2, 0, 0);

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
            ThreeGlassesEvents.HeadRotEvent += UpdateRot;

            if (_outRenderTexture != null) return;

            _leftRenderTexture = new RenderTexture(RenderWidth / 2, RenderHeight, 24,
                RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Default);

            _rightRenderTexture = new RenderTexture(RenderWidth / 2, RenderHeight, 24,
                RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Default);

            _outRenderTexture = new RenderTexture(RenderWidth, RenderHeight, 24,
                RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Default);

            Out = _outRenderTexture;

            leftCamera.SetRenderTarget(_leftRenderTexture);
            rightCamera.SetRenderTarget(_rightRenderTexture);
        }

        void OnDisable()
        {
            ThreeGlassesEvents.HeadPosEvent -= UpdatePos;
            ThreeGlassesEvents.HeadRotEvent -= UpdateRot;

            if (_leftRenderTexture != null)
            {
                _leftRenderTexture.Release();
            }
            _leftRenderTexture = null;

            if (_rightRenderTexture != null)
            {
                _rightRenderTexture.Release();
            }
            _rightRenderTexture = null;

            if (_outRenderTexture != null)
            {
                _outRenderTexture.Release();
            }
            _outRenderTexture = null;
        }

        void UpdatePos(Vector3 pos)
        {
            if (EnableHeadPosTracking)
            {
                transform.localPosition = pos;
            }
        }

        void UpdateRot(Quaternion rotation)
        {
            if (EnableHeadRotTracking)
            {
                transform.localRotation = rotation;
            }
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
                !_rightRenderTexture.Create() ||
                !_outRenderTexture.Create())
            {
                return;
            }

            Graphics.SetRenderTarget(_outRenderTexture);

            GL.PushMatrix();
            GL.LoadOrtho();

            GL.Viewport(new Rect(0, 0, RenderWidth / 2.0f, RenderHeight));
            Graphics.DrawTexture(new Rect(0, 0, 1.0f, 1.0f), _leftRenderTexture);

            GL.Viewport(new Rect(RenderWidth / 2.0f, 0, RenderWidth / 2.0f, RenderHeight));
            Graphics.DrawTexture(new Rect(0, 0, 1.0f, 1.0f), _rightRenderTexture);

            GL.PopMatrix();

            UpdateTextureFromUnity(_outRenderTexture.GetNativeTexturePtr());
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);

            eyeStatus[0] = eyeStatus[1] = false;
        }
    }
}