// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Loca
// ReSharper disable ConvertToAutoProperty


namespace ThreeGlasses
{
    public class ThreeGlassesWandButtonEvent
    {
        private static class ButtonMask
        {
            public const uint MenuButton = 0x000001;
            public const uint BButton = 0x000001 << 2;
            public const uint LeftHandle = 0x000001 << 3;
            public const uint RightHandle = 0x000001 << 4;
            public const uint Trigger = 0x000001 << 5;
            public const uint TriggerPressEnd = 0x000001 << 6;
        }

        private static class TriggerConst
        {
            public const byte MAX_VALUE = 255;
        }

        public class ButtonEvent
        {
            public static ButtonEvent NoneEvent = new ButtonEvent(0, 255, new byte[] { 0, 0 }, ThreeGlassesInterfaces.LeftOrRight.Right);

            private static uint L_OLD_KEY_STATUS;
            private static uint R_OLD_KEY_STATUS;

            internal uint KeyStatus
            {
                get { return keyStatus; }
                private set { keyStatus = value; }
            }

            public ThreeGlassesInterfaces.LeftOrRight LeftOrRight
            {
                get { return lr; }
                private set { lr = value; }
            }

            public bool MenuButton
            {
                get { return menuButton; }
                private set { menuButton = value; }
            }

            public bool BButton
            {
                get { return bButton; }
                private set { bButton = value; }
            }

            public bool LeftHandle
            {
                get { return leftHandle; }
                private set { leftHandle = value; }
            }

            public bool RightHandle
            {
                get { return rightHandle; }
                private set { rightHandle = value; }
            }

            public bool Trigger
            {
                get { return trigger; }
                private set { trigger = value; }
            }

            public bool TriggerPressEnd
            {
                get { return triggerPressEnd; }
                private set { triggerPressEnd = value; }
            }

            public float TriggerValue
            {
                get { return trigger_value; }
                private set { trigger_value = value; }
            }

            public float StickXValue
            {
                get { return stickX_value; }
                private set { stickX_value = value; }
            }

            public float StickYValue
            {
                get { return stickY_value; }
                private set { stickY_value = value; }
            }

            public bool OnMenuButtonDown
            {
                get { return onMenuButtonDown; }

                private set { onMenuButtonDown = value; }
            }

            public bool OnBButtonDown
            {
                get { return onBButtonDown; }

                private set { onBButtonDown = value; }
            }

            public bool OnLeftHandleDown
            {
                get { return onLeftHandleDown; }

                private set { onLeftHandleDown = value; }
            }

            public bool OnRightHandleDown
            {
                get { return onRightHandleDown; }

                private set { onRightHandleDown = value; }
            }

            public bool OnTriggerDown
            {
                get { return onTriggerDown; }

                private set { onTriggerDown = value; }
            }

            public bool OnTriggerPressEndDown
            {
                get { return onTriggerPressEndDown; }

                private set { onTriggerPressEndDown = value; }
            }

            public bool OnMenuButtonUp
            {
                get { return onMenuButtonUp; }

                private set { onMenuButtonUp = value; }
            }

            public bool OnBButtonUp
            {
                get { return onBButtonUp; }

                private set { onBButtonUp = value; }
            }

            public bool OnLeftHandleUp
            {
                get { return onLeftHandleUp; }

                private set { onLeftHandleUp = value; }
            }

            public bool OnRightHandleUp
            {
                get { return onRightHandleUp; }

                private set { onRightHandleUp = value; }
            }

            public bool OnTriggerUp
            {
                get { return onTriggerUp; }

                private set { onTriggerUp = value; }
            }

            public bool OnTriggerPressEndUp
            {
                get { return onTriggerPressEndUp; }

                private set { onTriggerPressEndUp = value; }
            }

            public float TriggerRawValue
            {
                get { return trigger_raw_value; }

                private set { trigger_raw_value = value; }
            }

            public float StickXRawValue
            {
                get { return stickX_raw_value; }

                private set { stickX_raw_value = value; }
            }

            public float StickYRawValue
            {
                get { return stickY_raw_value; }

                private set { stickY_raw_value = value; }
            }

            private uint keyStatus;
            private ThreeGlassesInterfaces.LeftOrRight lr;
            private bool menuButton;
            private bool bButton;
            private bool leftHandle;
            private bool rightHandle;
            private bool trigger;
            private bool triggerPressEnd;

            private bool onMenuButtonDown;
            private bool onBButtonDown;
            private bool onLeftHandleDown;
            private bool onRightHandleDown;
            private bool onTriggerDown;
            private bool onTriggerPressEndDown;

            private bool onMenuButtonUp;
            private bool onBButtonUp;
            private bool onLeftHandleUp;
            private bool onRightHandleUp;
            private bool onTriggerUp;
            private bool onTriggerPressEndUp;

            private float trigger_value;
            private float stickX_value;
            private float stickY_value;

            private float trigger_raw_value;
            private float stickX_raw_value;
            private float stickY_raw_value;

            public ButtonEvent(uint status, byte trigger_value, byte[] stick, ThreeGlassesInterfaces.LeftOrRight LR)
            {
                KeyStatus = status;
                LeftOrRight = LR;

                MenuButton = (status & ButtonMask.MenuButton) == ButtonMask.MenuButton;
                BButton = (status & ButtonMask.BButton) == ButtonMask.BButton;
                LeftHandle = (status & ButtonMask.LeftHandle) == ButtonMask.LeftHandle;
                RightHandle = (status & ButtonMask.RightHandle) == ButtonMask.RightHandle;
                Trigger = (status & ButtonMask.Trigger) == ButtonMask.Trigger;
                TriggerPressEnd = (status & ButtonMask.TriggerPressEnd) == ButtonMask.TriggerPressEnd;

                StickXValue = 0.5f;
                StickYValue = 0.5f;

                if (LR == ThreeGlassesInterfaces.LeftOrRight.Left)
                {
                    RefreshOnEvents(L_OLD_KEY_STATUS);
                    L_OLD_KEY_STATUS = status;
                }
                else
                {
                    RefreshOnEvents(R_OLD_KEY_STATUS);
                    R_OLD_KEY_STATUS = status;
                }

                TriggerValue = trigger_value / (float)TriggerConst.MAX_VALUE;

                StickXValue = ThreeGlassesUtils.Lerp(0, 1.0f, stick[0] / (float)TriggerConst.MAX_VALUE);
                StickYValue = ThreeGlassesUtils.Lerp(0, 1.0f, stick[1] / (float)TriggerConst.MAX_VALUE);

                TriggerRawValue = trigger_value;
                StickXRawValue = stick[0];
                StickYRawValue = stick[1];
            }

            private void RefreshOnEvents(uint oldStatus)
            {
                OnMenuButtonDown = MenuButton && (oldStatus & ButtonMask.MenuButton) != ButtonMask.MenuButton;
                OnBButtonDown = BButton && (oldStatus & ButtonMask.BButton) != ButtonMask.BButton;
                OnLeftHandleDown = LeftHandle && (oldStatus & ButtonMask.LeftHandle) != ButtonMask.LeftHandle;
                OnRightHandleDown = RightHandle && (oldStatus & ButtonMask.RightHandle) != ButtonMask.RightHandle;
                OnTriggerDown = Trigger && (oldStatus & ButtonMask.Trigger) != ButtonMask.Trigger;
                OnTriggerPressEndDown = TriggerPressEnd && (L_OLD_KEY_STATUS & ButtonMask.TriggerPressEnd) != ButtonMask.TriggerPressEnd;

                OnMenuButtonUp = (!MenuButton) && (oldStatus & ButtonMask.MenuButton) == ButtonMask.MenuButton;
                OnBButtonUp = (!BButton) && (oldStatus & ButtonMask.BButton) == ButtonMask.BButton;
                OnLeftHandleUp = (!LeftHandle) && (oldStatus & ButtonMask.LeftHandle) == ButtonMask.LeftHandle;
                OnRightHandleUp = (!RightHandle) && (oldStatus & ButtonMask.RightHandle) == ButtonMask.RightHandle;
                OnTriggerUp = (!Trigger) && (oldStatus & ButtonMask.Trigger) == ButtonMask.Trigger;
                OnTriggerPressEndUp = (!TriggerPressEnd) && (oldStatus & ButtonMask.TriggerPressEnd) == ButtonMask.TriggerPressEnd;
            }
        }
    }
}