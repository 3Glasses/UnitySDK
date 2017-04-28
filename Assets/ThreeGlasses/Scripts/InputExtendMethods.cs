using System;
using UnityEngine;
using System.Collections;

namespace ThreeGlasses
{
    public enum InputType
    {
        LeftWand = 0,
        RightWand = 1,
        HMD
    }

    public enum InputKey
    {
        WandMenu = 0,
        WandBack,
        WandLeftSide,
        WandRightSide,
        WandTriggerWeak,
        HmdMenu = 20,
        HmdExit
    };

    public static class TGInput
    {
        // get HMD or Wand's Position
        public static Vector3 GetPosition(InputType type)
        {
            switch (type)
            {
                case InputType.LeftWand:
                case InputType.RightWand:
                    return ThreeGlassesManager.joyPad[(int) type] == null ? Vector3.zero : ThreeGlassesManager.joyPad[(int)type].pack.position;
                case InputType.HMD:
                    return ThreeGlassesManager.hmdPosition;
            }
            return Vector3.zero;
        }
        // get HMD or Wand's Rotation
        public static Quaternion GetRotation(InputType type)
        {
            switch (type)
            {
                case InputType.LeftWand:
                case InputType.RightWand:            
                    return ThreeGlassesManager.joyPad[(int)type] == null ? Quaternion.identity : ThreeGlassesManager.joyPad[(int) type].pack.rotation;
                case InputType.HMD:
                    return ThreeGlassesManager.hmdRotation;
            }
            return Quaternion.identity;
        }

        public static bool GetKey(InputType type, InputKey key)
        {
            switch (type)
            {
                case InputType.LeftWand:
                case InputType.RightWand:
                    if (ThreeGlassesManager.joyPad[(int) type] == null)
                    {
                        return false;
                    }
                    return ThreeGlassesManager.joyPad[(int)type].GetKey(key);
                 case InputType.HMD:
                    return ThreeGlassesManager.GetHmdKey(key);
            }
            return false;
        }
        
        // get hmd touchpad
        public static Vector2 GetHMDTouchPad()
        {
            return ThreeGlassesManager.GetHmdTouchPad();
        }

        // get hmd name
        public static string GetHMDName()
        {
            return ThreeGlassesManager.hmdName;
        }

        // get trigger process rang=0-1.0
        public static float GetTriggerProcess(InputType type)
        {
            if (type == InputType.LeftWand || type == InputType.RightWand)
            {
                if (ThreeGlassesManager.joyPad[(int)type] != null)
                    return ThreeGlassesManager.joyPad[(int)type].GetTriggerProcess();
            } 
                                
            return 0.0f;           
        }

        // get stick rang=0-1.0
        public static Vector2 GetStick(InputType type)
        {
            if (type == InputType.LeftWand || type == InputType.RightWand)
                return ThreeGlassesManager.joyPad[(int)type] == null ? Vector2.zero : ThreeGlassesManager.joyPad[(int)type].GetStick();
                
            return new Vector2(0,0);
        }

    }
}
