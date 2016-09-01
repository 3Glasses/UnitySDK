using UnityEngine;
using ThreeGlasses;
// ReSharper disable CheckNamespace

public class JoystickButtonUpTest : MonoBehaviour {

    public void OnEnable()
    {
        ThreeGlassesEvents.WandButtonEvent += ThreeGlassesEventsOnWandButtonEvent;
    }

    public void OnDisable()
    {
        ThreeGlassesEvents.WandButtonEvent -= ThreeGlassesEventsOnWandButtonEvent;
    }

    private static void ThreeGlassesEventsOnWandButtonEvent(ThreeGlassesWandButtonEvent.ButtonEvent buttonEvent)
    {
        if (buttonEvent.OnBButtonUp)
        {
            Debug.Log("OnBButtonUp");
        }
        if (buttonEvent.OnMenuButtonUp)
        {
            Debug.Log("OnMenuButtonUp");
        }
        if (buttonEvent.OnLeftHandleUp)
        {
            Debug.Log("OnLeftHandleUp");
        }
        if (buttonEvent.OnRightHandleUp)
        {
            Debug.Log("OnRightHandleUp");
        }
        if (buttonEvent.OnTriggerUp)
        {
            Debug.Log("OnTriggerUp");
        }
        if (buttonEvent.OnTriggerPressEndUp)
        {
            Debug.Log("OnTriggerPressEndUp");
        }
    }
}
