using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{
    public enum InputType
    {
        LeftWand = 0,
        RightWand = 1
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
            if (ThreeGlassesCamera.joyPad[(int)type] == null)
                return false;
            
            return ThreeGlassesCamera.joyPad[(int)type].GetKey(key);
        }
        
        // get trigger process rang=0-1.0
        public static float GetTriggerProcess(InputType type)
        {
            if (ThreeGlassesCamera.joyPad[(int)type] == null)
                return 0.0f;
            
            return ThreeGlassesCamera.joyPad[(int)type].GetTriggerProcess();
        }

        // get stick rang=0-1.0
        public static Vector2 GetStick(InputType type)
        {
            return ThreeGlassesCamera.joyPad[(int)type] == null ? Vector2.zero : ThreeGlassesCamera.joyPad[(int)type].GetStick();
        }

        public static Vector3 GetPosition(InputType type)
        {
            return ThreeGlassesCamera.joyPad[(int) type] == null ? Vector3.zero : ThreeGlassesCamera.joyPad[(int)type].pack.position;
        }

        public static Quaternion GetRotation(InputType type)
        {
            return ThreeGlassesCamera.joyPad[(int)type] == null ? Quaternion.identity : ThreeGlassesCamera.joyPad[(int) type].pack.rotation;
        }

        public static Vector3 GetHeadDisplayPosition()
        {
            return ThreeGlassesCamera.headDisplayPosition;
        }

        public static Quaternion GetHeadDisplayRotation()
        {
            return ThreeGlassesCamera.headDisplayRotation;
        }
        

    }
}
