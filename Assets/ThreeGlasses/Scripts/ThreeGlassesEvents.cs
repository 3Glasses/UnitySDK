using UnityEngine;
using System.Collections;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

/*
 * Main Event Module 
 */

namespace ThreeGlasses
{
    [AddComponentMenu("3Glasses/Event System")]
    public class ThreeGlassesEvents : MonoBehaviour
    {
        public static event System.Action<Quaternion> HeadRotEvent;
        public static event System.Action<Vector3> HeadPosEvent;
        public static event System.Action<Quaternion, Vector3> LeftWandEvent;
        public static event System.Action<Quaternion, Vector3> RightWandEvent;
        public static event System.Action<ThreeGlassesWandButtonEvent.ButtonEvent> WandButtonEvent;

        private static ThreeGlassesEvents self;

        void Awake()
        {
            if ( self != null )
            {
                if ( self != this )
                {
                    Debug.LogError("multiple events object");
                }
            }
            self = this;
        }

        void Update()
        {
            if ( self != this ) return;
            GetWandPosAndRotd(ThreeGlassesInterfaces.LeftOrRight.Left,  LeftWandEvent);
            GetWandPosAndRotd(ThreeGlassesInterfaces.LeftOrRight.Right, RightWandEvent);

            StartCoroutine(ThreeGlassesUtils.DelayedRun(RefreshOrientationData, new WaitForEndOfFrame()));
        }

        static void GetWandPosAndRotd(ThreeGlassesInterfaces.LeftOrRight lr, System.Action<Quaternion, Vector3> callback)
        {
            var position = Vector3.zero;
            var rotation = Quaternion.identity;
            ThreeGlassesWandButtonEvent.ButtonEvent buttonEvent;
            ThreeGlassesInterfaces.GetWandPosAndRot(lr, ref position, ref rotation, out buttonEvent);

            if (callback != null)
            {
                callback(rotation, position);
            }
            if (WandButtonEvent != null)
            {
                WandButtonEvent(buttonEvent);
            }
        }

        static void RefreshOrientationData()
        {
            if( HeadRotEvent != null)
            {
                var rotation = ThreeGlassesInterfaces.GetCameraOrientation();
                if( rotation.x.Equals(float.NaN) ||
                    rotation.y.Equals(float.NaN) ||
                    rotation.z.Equals(float.NaN) ||
                    rotation.w.Equals(float.NaN))
                {
                    rotation = Quaternion.identity;
                }
                HeadRotEvent(rotation);
            }

            if (HeadPosEvent == null)  return;
            var vec = ThreeGlassesInterfaces.GetCameraPosition();
            HeadPosEvent(vec);
        }

        void OnDestroy()
        {
            if(self == this)
            {
                self = null;
            }
        }
    }
}