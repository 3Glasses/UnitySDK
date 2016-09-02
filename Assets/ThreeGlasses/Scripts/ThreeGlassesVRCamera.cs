using System.Collections;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local


/*
* Distortion Module
*/

namespace ThreeGlasses
{

    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("3Glasses/VR Camera")]
    public class ThreeGlassesVRCamera : MonoBehaviour
    {
        public bool LeftEye;
        public Camera cam;

        private RenderTexture render;
        private static Material material;

        public void SetRenderTarget(RenderTexture r)
        {
            render = r;
        }

        void Awake()
        {
            if (material != null) return;
            var shader = Resources.Load<Shader>(ThreeGlassesConst.FixShaderPath);
            material = new Material(shader);
        }

        void Start()
        {
            cam = GetComponent<Camera>();
            cam.rect = new Rect(0, 0, 1.0f, 1.0f);

            StartCoroutine(ThreeGlassesUtils.DelayedRun(
                () =>
                {
                    var cams = gameObject.GetComponentsInChildren<Camera>();
                    if (cams == null) return;
                    foreach (var l_cam in cams)
                    {
                        l_cam.rect = cam.rect;
                        l_cam.fieldOfView = cam.fieldOfView;
                        l_cam.nearClipPlane = cam.nearClipPlane;
                        l_cam.farClipPlane = cam.farClipPlane;
                    }
                },
                new WaitForFixedUpdate()
                ));

            cam.targetTexture = render;
        }

        void OnDisable()
        {
            cam.targetTexture = null;
            if (render == null) return;
            render.Release();
            render = null;
        }

        void LateUpdate()
        {
            StartCoroutine(UpdateTexture());
        }

        IEnumerator UpdateTexture()
        {
            yield return new WaitForEndOfFrame();

            if (render == null) yield break;
            ThreeGlassesHeadset.Submit(LeftEye);
        }
    }
}
