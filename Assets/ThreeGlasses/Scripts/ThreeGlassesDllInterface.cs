
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
            if (0 != SZVR_GetHMDConnectionStatus(
                    ref hmdConnection))
            {
                InitDevices();
                StartTracking(IntPtr.Zero, IntPtr.Zero,
                    IntPtr.Zero, IntPtr.Zero);
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
        public static extern uint SZVR_GetHMDConnectionStatus(ref bool result);
        [DllImport(Dllname)]// IntPtr must Marshal.AllocHGlobal(64), Marshal.PtrToStringAnsi to string
        public static extern uint SZVR_GetHMDDevName(IntPtr name);
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDDevIPD(ref byte value);
        // if light
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDPresent(ref bool result);

        // hmd rotation & position
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDRotate(float[] rotate);//3
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDPos(float[] pos);//4

        // hmd touchpad
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDTouchpad(byte[] result);//2

        // hmd button
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDMenuButton(ref bool result);
        [DllImport(Dllname)]
        public static extern uint SZVR_GetHMDExitButton(ref bool result);


        // wand --------------------------------------------------------------------------------
        // status
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandConnectionStatus(byte[] status);

        // rotate & position
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandRotate(float[] result);//8
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandPos(float[] result);//6

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
        public static extern uint SZVR_GetWandButton(byte[] result);

        // trigger
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandTriggerProcess(byte[] result); //2
        // stick
        [DllImport(Dllname)]
        public static extern uint SZVR_GetWandStick(byte[] result);//4

        [DllImport(Dllname)]
        public static extern uint SZVR_SetVibrator_V2(uint index, ushort value); // index: 0,1 value: 0~100


        // render ------------------------------------------------------------------------------
        [DllImport(Dllname)]
        public static extern void SZVRPluginProjection(float[] matrix);

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
