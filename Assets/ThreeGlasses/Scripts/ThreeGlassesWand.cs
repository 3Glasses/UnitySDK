using UnityEngine;
using System.Collections;
namespace ThreeGlasses
{
    public class ThreeGlassesWand {
        private const int KEY_NUM = 6;
        private const uint KEY_DOWN = 1;

        public const int WANDS_BUTTON_MASK_MENU = 0x01;
        public const int WANDS_BUTTON_MASK_BACK = 0x02;
        public const int WANDS_BUTTON_MASK_LEFT_HANDLE = 0x04;
        public const int WANDS_BUTTON_MASK_RIGHT_HANDLE = 0x08;
        public const int WANDS_BUTTON_MASK_TRIGGER_PRESSED = 0x10;
        public const int WANDS_BUTTON_MASK_TRIGGER_PRESS_END = 0x20;


        public class Wand
        {
            public InputType type = InputType.LeftWand;
            // wand postion & rotation
            public Vector3 position;
            public Quaternion rotation;
            // key status
            public int keyStatus;
            // stick status
            public Vector2 stick;
            public float triggerProcess;


            public Wand(Wand wand)
            {
                type = wand.type;
                position = wand.position;
                rotation = wand.rotation;
                stick = new Vector2(wand.stick.x, wand.stick.y);
                triggerProcess = wand.triggerProcess;
                keyStatus = wand.keyStatus;
            }
            public Wand()
            {
                type = InputType.LeftWand;
                position = Vector3.zero;
                rotation = Quaternion.identity;
                keyStatus = 0;
                stick = Vector2.zero;
                triggerProcess = 0;
            }
        }

        public Wand pack = new Wand();

        public ThreeGlassesWand(InputType type)
        {
            pack.type = type;
        }

        // update wand info
        public void Update()
        {
            // update position
            float x = 0, y = 0, z = 0, w = 1;
            if (0 == ThreeGlassesDllInterface.szvrGetWandsPositonWithVector((int)pack.type, ref x, ref y, ref z))
            {
                Vector3 vec = new Vector3(x, y, z)/-1000f;
                if (ThreeGlassesUtils.CheckNaN(vec))
                {
                    pack.position = vec;
                    // Debug.Log("type=" + pack.type + "    vec=" + vec);
                }
            }
            // update rotation
            if (0 == ThreeGlassesDllInterface.szvrGetWandsOrientationWithQuat((int)pack.type, ref x, ref y, ref z, ref w))
            {
                Quaternion quat = new Quaternion(x, -y, z, -w);
                if (ThreeGlassesUtils.CheckNaN(quat))
                {
                    pack.rotation = quat;
                }
            }

            // update key
            int trigger_value = 0;
            if (0 == ThreeGlassesDllInterface.szvrGetWandsTriggerValue((int)pack.type, ref trigger_value))
            {
                pack.triggerProcess = 1.0f - (trigger_value / (float)255.0);
            }
            
            int stick_x = 128, stick_y = 128;
            if (0 == ThreeGlassesDllInterface.szvrGetWandsStickValue((int)pack.type, ref stick_x, ref stick_y))
            {
                //                 int left = (int)Mathf.Clamp(((stickTemp[1] - 127) * 1.2f), -128, 128);
                //                 int right = (int)Mathf.Clamp(((stickTemp[2] - 127) * 1.2f), -128, 128);
                //                 left /= 16;//-8~8
                //                 right /= 16;
                // 
                //                 pack.stick[0] = left / 8.0f;
                //                 pack.stick[1] = -right / 8.0f;
                pack.stick[0] = ((stick_x / (float)255.0) - 0.5f)*2.0f;
                pack.stick[1] = (-(stick_y / (float)255.0) + 0.5f)*2.0f;
            }

            int keyStatus = 0;
            if (0 == ThreeGlassesDllInterface.szvrGetWandsButtonState((int)pack.type, ref keyStatus))
            {
                pack.keyStatus = keyStatus;
            }
        }
        
        // get key status up=false  down=true
        public bool GetKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.WandMenu:
                    return (pack.keyStatus & WANDS_BUTTON_MASK_MENU) != 0;
                case InputKey.WandBack:
                    return (pack.keyStatus & WANDS_BUTTON_MASK_BACK) != 0;
                case InputKey.WandLeftSide:
                    return (pack.keyStatus & WANDS_BUTTON_MASK_LEFT_HANDLE) != 0;
                case InputKey.WandRightSide:
                    return (pack.keyStatus & WANDS_BUTTON_MASK_RIGHT_HANDLE) != 0;
                case InputKey.WandTriggerWeak:
                    return (pack.keyStatus & WANDS_BUTTON_MASK_TRIGGER_PRESSED) != 0;
                case InputKey.WandTriggerStrong:
                    return (pack.keyStatus & WANDS_BUTTON_MASK_TRIGGER_PRESS_END) != 0;
            }
            return false;
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
