﻿using UnityEngine;
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

        public static Vector3 GetPosition(InputType type)
        {
            Vector3 temp = ThreeGlassesCamera.joyPad[(int)type].pack.position;
            return new Vector3(temp.x, temp.y, temp.z);
        }

        public static Quaternion GetRotation(InputType type)
        {
            Quaternion temp = ThreeGlassesCamera.joyPad[(int)type].pack.rotation;
            return new Quaternion(temp.x, temp.y, temp.z, temp.w);
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
