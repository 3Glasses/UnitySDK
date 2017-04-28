using System;
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
                }
                return false;
            }

            public void SetMotor(ushort level)
            {
                ThreeGlassesDllInterface.SZVR_SetVibrator_V2((uint)type, level);
            }
        }

        public Wand pack = new Wand();

        public ThreeGlassesWand(InputType type)
        {
            pack.type = type;
        }

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
            pack.stick[1] = ((stick[offset + 1] / (float)255.0) - 0.5f)*2.0f;
        }
        public void UpdateTrigger(byte[] trigger)
        {
            int offset = (int)pack.type;
            pack.triggerProcess = trigger[offset] / (float)255.0;

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

        public void SetMotor(ushort level)
        {
            pack.SetMotor(level);
        }
    }
}
