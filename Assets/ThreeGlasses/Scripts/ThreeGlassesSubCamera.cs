using System.Collections;
using UnityEngine;
namespace ThreeGlasses
{
    [RequireComponent(typeof(Camera))]
    public class ThreeGlassesSubCamera : MonoBehaviour
    {
        private Material material;
        private bool flip = false;
        public enum CameraType
        {
            LeftEye = 0,
            RightEye = 1,
            Screen = 3
        };
        public CameraType type = CameraType.Screen;
        void Awake()
        {
            material = new Material(Shader.Find("Hidden/ThreeGlasses/ReverseUV"));
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if(flip)
            {
                Graphics.Blit(src, dst, material);
            }
            else
            {
                Graphics.Blit(src, dst);
            }
            
        }

        public bool FLIP
        {
            get { return flip; }
            set { flip = value; }
        }

    }
}
