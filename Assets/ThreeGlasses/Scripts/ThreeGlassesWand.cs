using UnityEngine;
using System.Collections;
namespace ThreeGlasses
{
    public class ThreeGlassesWand {
        private const int KEY_NUM = 6;
        private const int KEY_DOWN = 1;
        public class Wand
        {
            public InputType type = InputType.LeftJoyPad;
            // wand postion & rotation
            public Vector3 position;
            public Quaternion rotation;
            // key status
            public uint[] keyStatus = new uint[KEY_NUM];
            // stick status
            public Vector2 stick;
            public float triggerProcess;
            
        }

        public Wand pack = new Wand();
        
        // temp save
        private uint[] keyStatusTemp = new uint[KEY_NUM];
        private byte[] stickTemp = new byte[3];

        public ThreeGlassesWand(InputType type)
        {
            pack.type = type;
        }

        // 更新位置信息并将其缓存
        public void Update()
        {
            if (0 == ThreeGlassesDllInterface.GetWandInput((uint)pack.type, keyStatusTemp, stickTemp))
            {
                pack.stick[0] = stickTemp[1] / (float)255.0;
                pack.stick[1] = stickTemp[2] / (float)255.0;
                pack.triggerProcess = stickTemp[0] / (float)255.0;

                for (int i = 0; i < KEY_NUM; i++)
                {
                    pack.keyStatus[i] = keyStatusTemp[i];
                }
            }            
        }
        
        // get key status up=false  down=true
        public bool GetKey(InputKey key)
        {
            return pack.keyStatus[(int)key] == KEY_DOWN;
        }
        
        // get trigger process rang=0-1.0
        public float GetTriggerProcess()
        {
            return pack.triggerProcess;
        }

        // get stick rang=0-1.0
        public Vector2 GetStick()
        {
            return new Vector2(pack.stick.x, pack.stick.y);
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
