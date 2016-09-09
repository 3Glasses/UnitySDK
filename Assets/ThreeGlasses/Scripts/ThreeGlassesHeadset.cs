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
        private const int RenderWidth = 4096;
        private const int RenderHeight = 2048;


        [DllImport("SZVRCompositorPlugin",
             EntryPoint = "UpdateTextureFromUnity")]
        private static extern void UpdateTextureFromUnity(System.IntPtr leftIntPtr, System.IntPtr rigthIntPtr);

        [DllImport("SZVRCompositorPlugin",
             EntryPoint = "GetRenderEventFunc")]
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
        private static RenderTexture _leftRenderTextureOut;
        private static RenderTexture _rightRenderTextureOut;

        private static bool[] eyeStatus = {false, false};

        private static Material _material;

        void Start()
        {
            if (_material == null)
            {
                _material = new Material(Shader.Find("Hidden/DrawTextureCloseLight"));
            }

            StartCoroutine(ThreeGlassesUtils.DelayedRun(() =>
            {
                SetCameraPos();
                if (_leftRenderTexture != null && _rightRenderTexture != null) return;

                _leftRenderTexture = new RenderTexture(RenderWidth/2, RenderHeight, 24,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);

                _rightRenderTexture = new RenderTexture(RenderWidth/2, RenderHeight, 24,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);

                _leftRenderTextureOut = new RenderTexture(RenderWidth / 2, RenderHeight, 24,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);

                _rightRenderTextureOut = new RenderTexture(RenderWidth / 2, RenderHeight, 24,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);

                leftCamera.SetRenderTarget(_leftRenderTexture);
                rightCamera.SetRenderTarget(_rightRenderTexture);

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
            ThreeGlassesEvents.HeadRotEvent += UpdateRot;
        }

        void OnDisable()
        {
            ThreeGlassesEvents.HeadPosEvent -= UpdatePos;
            ThreeGlassesEvents.HeadRotEvent -= UpdateRot;
        }

        void OnDestroy()
        {
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
                !_rightRenderTexture.Create())
            {
                return;
            }

            GL.Viewport(new Rect(0, 0, RenderWidth/2.0f, RenderHeight));
            GL.PushMatrix();
            GL.LoadOrtho();

            Graphics.SetRenderTarget(_leftRenderTextureOut);
            Graphics.DrawTexture(new Rect(0, 0, 1.0f, 1.0f), _leftRenderTexture, _material);

            Graphics.SetRenderTarget(_rightRenderTextureOut);
            Graphics.DrawTexture(new Rect(0, 0, 1.0f, 1.0f), _rightRenderTexture, _material);

            GL.PopMatrix();

            UpdateTextureFromUnity(_leftRenderTextureOut.GetNativeTexturePtr(), _rightRenderTextureOut.GetNativeTexturePtr());
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);

            eyeStatus[0] = eyeStatus[1] = false;
        }
    }
}
