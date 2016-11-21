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

        public ThreeGlassesSubCamera()
        {
            Flip = false;
        }

        void Start()
        {
            material = new Material(Shader.Find("Hidden/ThreeGlasses/ReverseUV"));
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if(Flip)
            {
                Graphics.Blit(src, dst, material);
            }
            else
            {
                Graphics.Blit(src, dst);
            }
            
        }

        public bool Flip { get; set; }
    }
}
