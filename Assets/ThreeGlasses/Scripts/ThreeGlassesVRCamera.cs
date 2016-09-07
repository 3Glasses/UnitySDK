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

        public void SetRenderTarget(RenderTexture r)
        {
            render = r;
            cam.targetTexture = render;

            var cams = gameObject.GetComponentsInChildren<Camera>();
            if (cams == null) return;
            foreach (var l_cam in cams)
            {
                l_cam.targetTexture = render;
            }
        }

        void Awake()
        {
            cam = GetComponent<Camera>();
        }

        void Start()
        {
            cam.rect = new Rect(0, 0, 1.0f, 1.0f);
            var cams = gameObject.GetComponentsInChildren<Camera>();
            if (cams == null) return;
            foreach (var l_cam in cams)
            {
                l_cam.rect = cam.rect;
                l_cam.fieldOfView = cam.fieldOfView;
                l_cam.nearClipPlane = cam.nearClipPlane;
                l_cam.farClipPlane = cam.farClipPlane;
            }
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
