using ThreeGlasses;
using UnityEngine;
// ReSharper disable CheckNamespace

public class JoystickTest : MonoBehaviour {
	
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
        if (buttonEvent.BButton)
        {
            Debug.Log("BButton");
        }
        if (buttonEvent.MenuButton)
        {
            Debug.Log("MenuButton");
        }
        if (buttonEvent.LeftHandle)
        {
            Debug.Log("LeftHandle");
        }
        if (buttonEvent.RightHandle)
        {
            Debug.Log("RightHandle");
        }
        if (buttonEvent.Trigger)
        {
            Debug.Log("Trigger");
        }
        if (buttonEvent.TriggerPressEnd)
        {
            Debug.Log("TriggerPressEnd");
        }
        Debug.Log("Trigger Value:" + buttonEvent.TriggerValue);
        Debug.Log("StickXValue Value:" + buttonEvent.StickXValue);
        Debug.Log("StickYValue Value:" + buttonEvent.StickYValue);
    }
}
