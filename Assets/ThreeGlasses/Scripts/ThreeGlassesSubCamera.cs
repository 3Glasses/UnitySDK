using System.Collections;
using UnityEngine;
namespace ThreeGlasses
{
    [RequireComponent(typeof(Camera))]
    public class ThreeGlassesSubCamera : MonoBehaviour
    {
        private Material material;
        private Matrix4x4 projection;
        private Camera camera;

        public enum CameraType
        {
            LeftEye = 0,
            RightEye = 1,
            Screen = 3
        };
        public CameraType type = CameraType.Screen;


        void Start()
        {
            material = new Material(Shader.Find("ThreeGlasses/DepthComposite"));
            camera = GetComponent<Camera>();

            projection = Matrix4x4.zero;
            var proj = new float[16];
            ThreeGlassesDllInterface.SZVRPluginProjection(proj);

            projection[0, 0] = proj[0];
            projection[1, 1] = proj[5];
            projection[0, 2] = proj[2];
            projection[1, 2] = proj[6];
            projection[2, 2] = proj[10];
            projection[2, 3] = proj[11];
            projection[3, 2] = proj[14];

            var nearClipPlane = camera.nearClipPlane;
            var farClipPlane = camera.farClipPlane;

            projection[2, 2] = (nearClipPlane + farClipPlane) / (nearClipPlane - farClipPlane);
            projection[2, 3] = 2 * nearClipPlane * farClipPlane / (nearClipPlane - farClipPlane);
        }

        void OnPreCull()
        {
            if (camera != null)
            {
                camera.projectionMatrix = projection;
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, material);
        }
    }
}
