using UnityEngine;
using ThreeGlasses;
// ReSharper disable CheckNamespace

public class JoystickButtonDownTest : MonoBehaviour {
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
        if (buttonEvent.OnBButtonDown)
        {
            Debug.Log("OnBButtonDown");
        }
        if (buttonEvent.OnMenuButtonDown)
        {
            Debug.Log("OnMenuButtonDown");
        }
        if (buttonEvent.OnLeftHandleDown)
        {
            Debug.Log("OnLeftHandleDown");
        }
        if (buttonEvent.OnRightHandleDown)
        {
            Debug.Log("OnRightHandleDown");
        }
        if (buttonEvent.OnTriggerDown)
        {
            Debug.Log("OnTriggerDown");
        }
        if (buttonEvent.OnTriggerPressEndDown)
        {
            Debug.Log("OnTriggerPressEndDown");
        }
    }
}
