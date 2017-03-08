using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace ThreeGlasses
{
    [RequireComponent(typeof(Camera))]
    public class ThreeGlassesSubCamera : MonoBehaviour
    {
        private Material _material;
        private Matrix4x4 _projection;
        private Camera _camera;

        public enum CameraTypes
        {
            LeftEye = 0,
            RightEye = 1,
            Screen = 3
        }

        // ReSharper disable once ConvertToConstant.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public CameraTypes CameraType = CameraTypes.Screen;


        public void Start()
        {
            _material = new Material(Shader.Find("ThreeGlasses/DepthComposite"));
            _camera = GetComponent<Camera>();

            _projection = Matrix4x4.zero;
            var proj = new float[16];
            ThreeGlassesDllInterface.SZVRPluginProjection(proj);

            _projection[0, 0] = proj[0];
            _projection[1, 1] = proj[5];
            _projection[0, 2] = proj[2];
            _projection[1, 2] = proj[6];
            _projection[2, 2] = proj[10];
            _projection[2, 3] = proj[11];
            _projection[3, 2] = proj[14];

            var nearClipPlane = _camera.nearClipPlane;
            var farClipPlane = _camera.farClipPlane;

            _projection[2, 2] = (nearClipPlane + farClipPlane) / (nearClipPlane - farClipPlane);
            _projection[2, 3] = 2 * nearClipPlane * farClipPlane / (nearClipPlane - farClipPlane);
        }

        public void OnPreCull()
        {
            if (_camera != null)
            {
                _camera.projectionMatrix = _projection;
            }
        }

        public void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, _material);
        }
    }
}
