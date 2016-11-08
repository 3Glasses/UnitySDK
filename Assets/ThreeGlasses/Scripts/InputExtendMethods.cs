using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{
    public enum InputType
    {
        LeftJoyPad = 0,
        RightJoyPad = 1
    }

    public enum InputKey
    {
        WandMenu = 0,
        WandBack,
        WandLeftSide,
        WandRightSide,
        WandTriggerWeak,
        WandTriggerStrong,
        InputNum
    };

    public static class TGInput
    {
        public static bool GetKey(InputType type, InputKey key)
        {
            return ThreeGlassesCamera.joyPad[(int)type].GetKey(key);
        }
        
        // get trigger process rang=0-1.0
        public static float GetTriggerProcess(InputType type)
        {
            return ThreeGlassesCamera.joyPad[(int)type].GetTriggerProcess();
        }

        // get stick rang=0-1.0
        public static Vector2 GetStick(InputType type)
        {
            return ThreeGlassesCamera.joyPad[(int)type].GetStick();
        }
    }
}
