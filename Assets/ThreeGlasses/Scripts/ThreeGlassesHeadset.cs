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

        private static RenderTexture OutRenderTexture;
        private static RenderTexture leftTexture;
        private static RenderTexture righTexture;

        private static bool[] eyeStatus = {false, false};

        private static Material material;

        private static Mesh mergeMesh;

        void Start()
        {
            SetCameraPos();

            if (material != null) return;
            var shader = Resources.Load<Shader>(ThreeGlassesConst.MergeShaderPath);
            material = new Material(shader);

            if (mergeMesh == null)
            {
                mergeMesh = ThreeGlassesUtils.CreateMesh(4, 4);
            }
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

            if (OutRenderTexture == null)
            {
                OutRenderTexture = new RenderTexture(RenderWidth, RenderHeight, 24,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);
            }

            if (leftTexture == null)
            {
                leftTexture = new RenderTexture(RenderWidth/2,
                    RenderHeight, 24, RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Default);

                leftCamera.SetRenderTarget(leftTexture);
            }

            if (righTexture != null) return;
            righTexture = new RenderTexture(RenderWidth/2,
                RenderHeight, 24, RenderTextureFormat.ARGBFloat,
                RenderTextureReadWrite.Default);

            rightCamera.SetRenderTarget(righTexture);
        }

        void OnDisable()
        {
            ThreeGlassesEvents.HeadPosEvent -= UpdatePos;
            ThreeGlassesEvents.HeadRotEvent -= UpdateRot;

            if (OutRenderTexture != null)
            {
                OutRenderTexture.Release();
            }

            if (leftTexture != null)
            {
                leftTexture.Release();
            }

            if (righTexture != null)
            {
                righTexture.Release();
            }

            OutRenderTexture = null;
            leftTexture = null;
            righTexture = null;
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
            if (!leftTexture.Create() || !righTexture.Create() || !OutRenderTexture.Create()) return;

#if UNITY_5_4_OR_NEWER
            Graphics.CopyTexture(
                leftTexture, 0, 0, 0, 0, leftTexture.width, leftTexture.height,
                OutRenderTexture, 0, 0, 0, 0);
            Graphics.CopyTexture(
                righTexture, 0, 0, 0, 0, righTexture.width, righTexture.height,
                OutRenderTexture, 0, 0, leftTexture.width, 0);
#else
            material.SetTexture(0, leftTexture);
            material.SetTexture(1, righTexture);

            Graphics.SetRenderTarget(OutRenderTexture);

            GL.PushMatrix();
            GL.LoadOrtho();

            material.SetPass(0);
            Graphics.DrawMeshNow(mergeMesh, Vector3.zero, Quaternion.identity);

            GL.PopMatrix();
#endif

            UpdateTextureFromUnity(OutRenderTexture.GetNativeTexturePtr());
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);

            eyeStatus[0] = eyeStatus[1] = false;
        }
    }
}