using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace ThreeGlasses
{
    public class ThreeGlassesDllInterface
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

        [DllImport(Dllname)]
        public static extern void GetTrackedPost(float[] hmd, float[] controllerLeft, float[] controllerRight);

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
        [DllImport(Dllname)]
        public static extern uint GetWandInput(uint side, uint[] buttoms, byte[] value);

        [DllImport(Dllname)]
        public static extern void UpdateTextureFromUnity(System.IntPtr leftIntPtr, System.IntPtr rigthIntPtr);

        [DllImport(Dllname)]
        public static extern System.IntPtr GetRenderEventFunc();

    }

}
