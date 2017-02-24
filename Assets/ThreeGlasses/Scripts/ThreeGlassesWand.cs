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

            // get key status up=false  down=true
            public bool GetKey(InputKey key)
            {
                switch (key)
                {
                    case InputKey.WandMenu:
                        return (keyStatus & WANDS_BUTTON_MASK_MENU) != 0;
                    case InputKey.WandBack:
                        return (keyStatus & WANDS_BUTTON_MASK_BACK) != 0;
                    case InputKey.WandLeftSide:
                        return (keyStatus & WANDS_BUTTON_MASK_LEFT_HANDLE) != 0;
                    case InputKey.WandRightSide:
                        return (keyStatus & WANDS_BUTTON_MASK_RIGHT_HANDLE) != 0;
                    case InputKey.WandTriggerWeak:
                        return (keyStatus & WANDS_BUTTON_MASK_TRIGGER_PRESSED) != 0;
                    case InputKey.WandTriggerStrong:
                        return (keyStatus & WANDS_BUTTON_MASK_TRIGGER_PRESS_END) != 0;
                }
                return false;
            }
        }

        public Wand pack = new Wand();

        public ThreeGlassesWand(InputType type)
        {
            pack.type = type;
        }

        // // update wand info
        // public void Update()
        // {
        //     // update position
        //     float x = 0, y = 0, z = 0, w = 1;
        //     if (0 == ThreeGlassesDllInterface.szvrGetWandsPositonWithVector((int)pack.type, ref x, ref y, ref z))
        //     {
        //         Vector3 vec = new Vector3(x, y, z)/-1000f;
        //         if (ThreeGlassesUtils.CheckNaN(vec))
        //         {
        //             pack.position = vec;
        //             // Debug.Log("type=" + pack.type + "    vec=" + vec);
        //         }
        //     }
        //     // update rotation
        //     if (0 == ThreeGlassesDllInterface.SZVR_GetWandRotate((int)pack.type, ref x, ref y, ref z, ref w))
        //     {
        //         Quaternion quat = new Quaternion(x, -y, z, -w);
        //         if (ThreeGlassesUtils.CheckNaN(quat))
        //         {
        //             pack.rotation = quat;
        //         }
        //     }

        //     // update key
        //     int trigger_value = 0;
        //     if (0 == ThreeGlassesDllInterface.szvrGetWandsTriggerValue((int)pack.type, ref trigger_value))
        //     {
        //         pack.triggerProcess = 1.0f - (trigger_value / (float)255.0);
        //     }
            
        //     int stick_x = 128, stick_y = 128;
        //     if (0 == ThreeGlassesDllInterface.szvrGetWandsStickValue((int)pack.type, ref stick_x, ref stick_y))
        //     {
        //         //                 int left = (int)Mathf.Clamp(((stickTemp[1] - 127) * 1.2f), -128, 128);
        //         //                 int right = (int)Mathf.Clamp(((stickTemp[2] - 127) * 1.2f), -128, 128);
        //         //                 left /= 16;//-8~8
        //         //                 right /= 16;
        //         // 
        //         //                 pack.stick[0] = left / 8.0f;
        //         //                 pack.stick[1] = -right / 8.0f;
        //         pack.stick[0] = ((stick_x / (float)255.0) - 0.5f)*2.0f;
        //         pack.stick[1] = (-(stick_y / (float)255.0) + 0.5f)*2.0f;
        //     }

        //     int keyStatus = 0;
        //     if (0 == ThreeGlassesDllInterface.szvrGetWandsButtonState((int)pack.type, ref keyStatus))
        //     {
        //         pack.keyStatus = keyStatus;
        //     }
        // }

        public void UpdatePos(float[] pos)
        {
            int offset = 3 * (int)pack.type;
            pack.position = new Vector3(pos[offset], pos[offset + 1], pos[offset + 2])/-1000f;
        }
        public void UpdateRotate(float[] rotate)
        {
            int offset = 4 * (int)pack.type;
            pack.rotation = new Quaternion(rotate[offset], -rotate[offset + 1], rotate[offset + 2], -rotate[offset + 3]);
        }
        public void UpdateStick(byte[] stick)
        {
            int offset = 2 * (int)pack.type;
            pack.stick[0] = ((stick[offset] / (float)255.0) - 0.5f)*2.0f; 
            pack.stick[1] = (-(stick[offset + 1] / (float)255.0) + 0.5f)*2.0f;
        }
        public void UpdateTrigger(byte[] trigger)
        {
            int offset = (int)pack.type;
            pack.triggerProcess = 1.0f - (trigger[offset] / (float)255.0);

        }
        public void UpdateButton(byte[] button)
        {
            int offset = 6 * (int)pack.type;
            pack.keyStatus = 0;
            for (int i = 0; i < 6; i++)
            {
                if (button[offset + i] != 0)
                {
                    pack.keyStatus |= 1 << i;
                }
            }
            
        }
        // get key status up=false  down=true
        public bool GetKey(InputKey key)
        {
            return pack.GetKey(key);
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
