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
            material = new Material(Shader.Find("Hidden/ThreeGlasses/DepthComposite"));
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            RenderTexture rt = RenderTexture.GetTemporary(src.width, dst.height, 0, RenderTextureFormat.ARGB32);

            Graphics.Blit(rt, rt, material, 0); // Clear
            Graphics.Blit(src, rt, material, 1);
            Graphics.Blit(rt, dst);

            RenderTexture.ReleaseTemporary(rt);
       }
    }
}
