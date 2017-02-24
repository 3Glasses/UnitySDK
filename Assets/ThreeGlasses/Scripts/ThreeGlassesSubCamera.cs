using System.Collections;
using UnityEngine;
namespace ThreeGlasses
{
    [RequireComponent(typeof(Camera))]
    public class ThreeGlassesSubCamera : MonoBehaviour
    {
        private Material material;

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
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, material);
        }
    }
}
