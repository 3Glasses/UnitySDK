using System.Collections;
using UnityEngine;
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

        private static ThreeGlassesWandButtonEvent.ButtonEvent _wand_button_event;
        private static Vector3 _wand_position;
        private static Quaternion _wand_quaternion;

        private static Vector3 _hmd_position;
        private static Quaternion _hmd_quaternion;

        //no gc
        private static ThreeGlassesInterfaces.LeftOrRight left = ThreeGlassesInterfaces.LeftOrRight.Left;
        private static ThreeGlassesInterfaces.LeftOrRight right = ThreeGlassesInterfaces.LeftOrRight.Right;

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

            StopAllCoroutines();
            StartCoroutine(RefreshOrientationData());
        }

        void Update()
        {
            if ( self != this ) return;
            GetWandPosAndRotd(ref left, ref LeftWandEvent);
            GetWandPosAndRotd(ref right, ref RightWandEvent);
        }

        static void GetWandPosAndRotd(ref ThreeGlassesInterfaces.LeftOrRight lr, ref System.Action<Quaternion, Vector3> callback)
        {
            ThreeGlassesInterfaces.GetWandPosAndRot(lr, ref _wand_position, ref _wand_quaternion, out _wand_button_event);

            if (callback != null)
            {
                callback(_wand_quaternion, _wand_position);
            }
            if (WandButtonEvent != null)
            {
                WandButtonEvent(_wand_button_event);
            }
        }

        static IEnumerator RefreshOrientationData()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (HeadRotEvent != null)
                {
                    _hmd_quaternion = ThreeGlassesInterfaces.GetCameraOrientation();
                    if (_hmd_quaternion.x.Equals(float.NaN) ||
                        _hmd_quaternion.y.Equals(float.NaN) ||
                        _hmd_quaternion.z.Equals(float.NaN) ||
                        _hmd_quaternion.w.Equals(float.NaN))
                    {
                        _hmd_quaternion = Quaternion.identity;
                    }
                    HeadRotEvent(_hmd_quaternion);
                }

                if (HeadPosEvent == null) continue;
                _hmd_position = ThreeGlassesInterfaces.GetCameraPosition();
                HeadPosEvent(_hmd_position);
            }
            // ReSharper disable once IteratorNeverReturns
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