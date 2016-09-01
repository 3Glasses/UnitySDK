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

        public bool FixUV = true;

        public float Near = 0.01f;
        public float Far = 1000f;

        public float EyeDistance = 0.1f;
        public float FieldOfView = 90f;

        public ThreeGlassesVRCamera leftCamera;
        public ThreeGlassesVRCamera rightCamera;

        internal static RenderTexture OutRenderTexture;  

        private static bool[] eyeStatus = {false, false};

        void Start()
        {
            SetCameraPos();
        }

        public void SetCameraPos()
        {
            var eyeDistance = new Vector3(EyeDistance / 2, 0, 0);

            if (leftCamera == null || rightCamera == null) return;

            leftCamera.transform.localPosition = -eyeDistance;
            rightCamera.transform.localPosition = eyeDistance;

            leftCamera.FixUV = FixUV;
            rightCamera.FixUV = FixUV;

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

            if (OutRenderTexture != null) return;

                OutRenderTexture = new RenderTexture(RenderWidth, RenderHeight, 24,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);

                leftCamera.SetRenderTarget(new RenderTexture(RenderWidth / 2,
                    RenderHeight, 24, RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default));

                rightCamera.SetRenderTarget(new RenderTexture(RenderWidth / 2,
                    RenderHeight, 24, RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default));
        }

        void OnDisable()
        {
            ThreeGlassesEvents.HeadPosEvent -= UpdatePos;
            ThreeGlassesEvents.HeadRotEvent -= UpdateRot;

            if (OutRenderTexture == null) return;
            OutRenderTexture.Release();
            OutRenderTexture = null;
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

        internal static void Submit(bool lefteye)
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
            UpdateTextureFromUnity(OutRenderTexture.GetNativeTexturePtr());
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);
            eyeStatus[0] = eyeStatus[1] = false;
        }
    }
}