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
        void Awake()
        {
            material = new Material(Shader.Find("Hidden/ThreeGlasses/FlipUV"));
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, material);
        }

    }
}
