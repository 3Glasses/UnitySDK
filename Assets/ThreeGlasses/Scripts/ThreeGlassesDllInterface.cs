using System.Runtime.InteropServices;

namespace ThreeGlasses
{
    public static class ThreeGlassesDllInterface
    {
        private const string Dllname = "SZVRUnityPlugin";

		[DllImport(Dllname)]
        public static extern void SZVRPluginInit();

        [DllImport(Dllname)]
        public static extern void SZVRPluginDestroy();

        [DllImport(Dllname)]
        public static extern uint SZVRPluginGetFOV();

        [DllImport(Dllname)]
        public static extern void SZVRPluginEnableATW();

        [DllImport(Dllname)]
        public static extern void SZVRPluginDiasbleATW();

        [DllImport(Dllname, EntryPoint = "GetHMDPresent")]
        public static extern void GetHMDPresent(uint[] status);

        // [DllImport(Dllname)]
        // public static extern void GetTrackedPost(float[] hmd, float[] controllerLeft, float[] controllerRight);

        // hmd
        // [DllImport(Dllname)]
        // public static extern void szvrGetHmdSerialNumer(char* szSerialNumber);
        // [DllImport(Dllname)]
        // public static extern void szvrGetHmdProductName(char* szProductName);
        // [DllImport(Dllname)]
        // public static extern unsigned short szvrGetDeviceFirmwareVersion();
        [DllImport(Dllname)]
        public static extern int szvrGetHmdDongleCheck();
        [DllImport(Dllname)]
        public static extern void szvrSetHmdDonglePower(bool bOn);
        [DllImport(Dllname)]
        public static extern uint szvrGetHmdLightSensorStatus();


        [DllImport(Dllname)]
        public static extern uint szvrGetHmdMenuButtonStatus();

        [DllImport(Dllname)]
        public static extern uint szvrGetHmdPowerButtonStatus();

        [DllImport(Dllname)]
        public static extern uint szvrGetHmdConnectStatus();
        
        [DllImport(Dllname)]
        public static extern uint szvrGetHmdOrientationWithQuat(ref float x, ref float y, ref float z, ref float w);
        [DllImport(Dllname)]
        public static extern uint szvrGetHmdPostionWithVector(ref float x, ref float y, ref float z);

        [DllImport(Dllname)]
        public static extern uint szvrGetHmdTouchPadValue(ref float x, ref float y);
        [DllImport(Dllname)]
        public static extern uint szvrResetHmdOrientationData();
        
        // wand
        [DllImport(Dllname)]
        public static extern uint szvrGetWandsButtonState(int iHand, ref int btn_mask);

        [DllImport(Dllname)]
        public static extern uint szvrGetWandsOrientationWithQuat(int iHand, ref float x, ref float y, ref float z, ref float w);
        [DllImport(Dllname)]
        public static extern uint szvrGetWandsPositonWithVector(int iHand, ref float x, ref float y, ref float z);
        
        [DllImport(Dllname)]
        public static extern uint szvrGetWandsTriggerValue(int iHand, ref int trigger_value); 

        [DllImport(Dllname)]
        public static extern uint szvrGetWandsStickValue(int iHand, ref int stick_x, ref int stick_y);

        /* 
         * Return Value(uint):
		 *   0: success to get the value
		 *   1: fail to get the value
         * 
         * side (uint):
         *   0 for the first recognized controller, 1 for the second recognized controller
         * buttoms ( uint array, length: 6) 
         *   0: up, 1: down
         *   buttoms[0] = Menu button;
		 *   buttoms[1] = Back button;
		 *   buttoms[2] = Left handle button;
		 *   buttoms[3] = Right handle button;
		 *   buttoms[4] = Trigger press down;
		 *   buttoms[5] = Trigger press all the way down;
         * value ( byte array, length: 3 )
         *   value[0] = value pointer for output trigger press measuring value, range in (0 - 255)
         *   value[1] = x coordinate;
         *   value[2] = y coordinate;
         */
        // [DllImport(Dllname)]
        // public static extern uint GetWandInput(uint side, uint[] buttoms, byte[] value);

        [DllImport(Dllname)]
        public static extern void GetRenderSize(uint[] bufferSize);

        [DllImport(Dllname)]
        public static extern void StereoRenderBegin();

        [DllImport(Dllname)]
        public static extern void UpdateTextureFromUnity(
            System.IntPtr leftIntPtr,
            System.IntPtr rigthIntPtr);

        [DllImport(Dllname)]
        public static extern System.IntPtr GetRenderEventFunc();

        // no vrshow
        [DllImport(Dllname)]
		public static extern uint szvrInitDevices();


		[DllImport(Dllname)]
		public static extern uint szvrShutdownDevices();
    }
}
