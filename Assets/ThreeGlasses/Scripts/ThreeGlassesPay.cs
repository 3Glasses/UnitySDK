using UnityEngine;
using UnityEngine.Events;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

/*
* Pay Module 
*/

namespace ThreeGlasses
{
    [AddComponentMenu("3Glasses/Pay")]
    public class ThreeGlassesPay : MonoBehaviour
    {
        public string AppKey = "";

        public bool EnablePayment = true;
        public bool EnableQuit = true;

        public UnityEvent PayFailedEvent
        {
            get
            {
                return payFailedEvent;
            }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UnityEvent payFailedEvent = new UnityEvent();

        void Start()
        {
#if !UNITY_EDITOR
        if (EnablePayment && !ThreeGlassesInterfaces.PayApp(AppKey))
        {
            if (EnableQuit)
            {
                Application.Quit();
            }
            else
            {
                PayFailedEvent.Invoke();
            }
        }
#endif
        }
    }
}