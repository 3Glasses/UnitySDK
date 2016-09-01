using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

/*
 * Wand Module
 */

namespace ThreeGlasses
{
    [AddComponentMenu("3Glasses/Control Wand")]
    public class ThreeGlassesWand : MonoBehaviour
    {
        public float factor = 1;
        public ThreeGlassesInterfaces.LeftOrRight LeftOrRight;

        private ThreeGlassesInterfaces.LeftOrRight _leftOrRight;

        void Awake()
        {
            _leftOrRight = LeftOrRight;
        }

        void OnEnable()
        {
            if (_leftOrRight == ThreeGlassesInterfaces.LeftOrRight.Left)
            {
                ThreeGlassesEvents.LeftWandEvent += HandleEvent;
            }
            else
            {
                ThreeGlassesEvents.RightWandEvent += HandleEvent;
            }
        }

        void OnDisable()
        {
            if (_leftOrRight == ThreeGlassesInterfaces.LeftOrRight.Left)
            {
                ThreeGlassesEvents.LeftWandEvent -= HandleEvent;
            }
            else
            {
                ThreeGlassesEvents.RightWandEvent -= HandleEvent;
            }
        }

        void HandleEvent(Quaternion rot, Vector3 pos)
        {
            transform.localPosition = pos * factor;
            transform.localRotation = rot;
        }
    }
}