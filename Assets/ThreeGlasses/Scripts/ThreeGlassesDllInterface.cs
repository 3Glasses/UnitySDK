
using System.Runtime.InteropServices;
using System;

namespace ThreeGlasses
{
    public static class ThreeGlassesDllInterface
    {
        private const string Dllname = "SZVRUnityPlugin";
        private const string ServerDllname = "3GlassesTracker";

        // Server ---------------------------------------------------------------------------------------
        // init & destroy
        [DllImport(ServerDllname)]
        public static extern int InitDevices();

        [DllImport(ServerDllname)]
        public static extern int StartTracking(IntPtr ptr, IntPtr ptr2, IntPtr ptr3, IntPtr ptr4);

        static ThreeGlassesDllInterface()
        {
            var hmdConnection = false;
            if (0 == SZVR_GetHMDConnectionStatus_V2(
                    ref hmdConnection)) return;

            InitDevices();
            StartTracking(IntPtr.Zero, IntPtr.Zero,
                IntPtr.Zero, IntPtr.Zero);

            uint[] buffsize = {0, 0};
            while (true)
            {
                ThreeGlassesDllInterface.GetNativeRenderSize(buffsize);
                if (buffsize[0] > 0 && buffsize[1] > 0)
                {
                    break;
                }
            }
        }

        // hmd ---------------------------------------------------------------------------------------
        // init & destroy
        [DllImport(Dllname)]
        public static extern void SZVRPluginInit(uint enableATW, uint renderWidth, uint renderHeight);
        [DllImport(Dllname)]
        public static extern void SZVRPluginDestroy();

        // self attr
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDConnectionStatus_V2(ref bool result);
        [DllImport(Dllname)]// IntPtr must Marshal.AllocHGlobal(64), Marshal.PtrToStringAnsi to string
        public static extern uint SZVR_GetHMDDevName_V2(IntPtr name);
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDDevIPD_V2(ref byte value);
        // if light
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDPresent_V2(ref bool result);

        // hmd rotation & position
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDRotate_V2(float[] rotate);//3
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDPos_V2(float[] pos);//4

        // hmd touchpad
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDTouchpad_V2(byte[] result);//2

        // hmd button
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDMenuButton_V2(ref bool result);
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDExitButton_V2(ref bool result);


        // wand --------------------------------------------------------------------------------
        // status
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandConnectionStatus_V2(byte[] status);

        // rotate & position
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandRotate_V2(float[] result);//8
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandPos_V2(float[] result);//6

        // button
        /*
          first wand
          0 Menu button;
          1 Back button;
          2 Left handle button;
          3 Right handle button;
          4 Trigger press down;
          5 Trigger press all the way down;
          second wand
          6 Menu button;
          7 Back button;
          8 Left handle button;
          9 Right handle button;
          10 Trigger press down;
          11 Trigger press all the way down;
        */
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandButton_V2(byte[] result);

        // trigger
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandTriggerProcess_V2(byte[] result); //2
        // stick
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandStick_V2(byte[] result);//4

        [DllImport(Dllname)]
        public static extern uint SZVR_SetVibrator_V2(uint index, ushort value); // index: 0,1 value: 0~100


        // render ------------------------------------------------------------------------------
        [DllImport(Dllname)]
        public static extern void SZVRPluginProjection(float[] leftProjMatrix, float[] rightProjMatrix );

        [DllImport(Dllname)]
        public static extern void GetNativeRenderSize(uint[] bufferSize);

        [DllImport(Dllname)]
        public static extern void UpdateTextureFromUnity(IntPtr leftIntPtr,
                                                         IntPtr rigthIntPtr);
        [DllImport(Dllname)]
        public static extern void StereoRenderBegin();

        [DllImport(Dllname)]
        public static extern IntPtr GetRenderEventFunc();

        // Algorithm
        [DllImport(Dllname, EntryPoint = "SZVR_ALGORITHM_FFT")]
        public static extern void FFT(
            float[] real, float[] imag, uint size);
    }

}
