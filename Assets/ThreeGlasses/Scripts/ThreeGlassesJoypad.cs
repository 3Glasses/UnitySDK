using UnityEngine;
using System.Collections;
namespace ThreeGlasses
{
    public class ThreeGlassesJoypad {
  
        InputType type = InputType.LeftJoyPad;

        // 手柄本身的位置和旋转
        public Vector3 position;
        public Quaternion rotation;
        // 手柄按键状态
        public uint keyStatus;
        // 手柄的摇杆状态
        public Vector2 stick;

        public ThreeGlassesJoypad(InputType type)
        {
            this.type = type;
        }
        // 掩码
        // todo 需要拿实物弄清楚状态对应的掩码
        private static class ButtonMask
        {
            public const uint MENU_BUTTON_MASK = 0x000001;
            public const uint B_BUTTON_MASK = 0x000001 << 2;
            public const uint LEFT_HANDLE_MASK = 0x000001 << 3;
            public const uint RIGHT_HANDLE_MASK = 0x000001 << 4;
            public const uint TRIGGER_MASK = 0x000001 << 5;
            public const uint TRIGGER_PRESSEND_MASK = 0x000001 << 6;
        }

        // 更新位置信息并将其缓存
        public void Update()
        {
            var quaternion_array = new float[4];
            var position_array = new float[3]; // 0.x 1.y 2.z
            var key_status = new uint[1];
            var trigger_value = new byte[] { 255 };// min:0, max:255
            var stick = new byte[2]; // 0.x 1.y

            
            // 转换信息并缓存
            position.x = checkFloat(position_array[0]) ? -position_array[0] : position.x;
            position.y = checkFloat(position_array[1]) ? -position_array[1] : position.y;
            position.z = checkFloat(position_array[2]) ? -position_array[2] : position.z;

            rotation.x = checkFloat(quaternion_array[2]) ? quaternion_array[2] : 0;
            rotation.y = -(checkFloat(quaternion_array[0]) ? quaternion_array[0] : 0);
            rotation.z = checkFloat(quaternion_array[1]) ? quaternion_array[1] : 0;
            rotation.w = -(checkFloat(quaternion_array[3]) ? quaternion_array[3] : 0);

            keyStatus = key_status[0];
            // todo trigger是否也属于按键类属于的话将其转换到keyStatus状态中并添加相应掩码

            // todo 测试stick的范围并归一化
            this.stick = new Vector2(stick[0], stick[1]);
        }

        public bool getKey(InputKey key)
        {
            // todo 根据掩码算得出键状态
            return false;
        }

        private static bool checkFloat(float v)
        {
            var status = !float.IsNaN(v);

            if (v > 500.0f || v < -500.0f)
            {
                status = false;
            }
            return status;
        }
    }
}
