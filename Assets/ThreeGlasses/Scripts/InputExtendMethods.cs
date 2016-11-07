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
        Fire,               // 扳机键
        Menu,               // 菜单键
    };

    public static class TGInput
    {
        
        public static bool GetKey(InputType type, InputKey key)
        {
            return ThreeGlassesCamera.getJoypadKey(type, key);
        }
    }
}
