using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

namespace ThreeGlasses
{
    public class ThreeGlassesDllInterface
    {
        // 注意缺少x86的版本
        // -----------------------------------------------------------相机------------SZVRUnityPlugin.dll
        [DllImport("SZVRUnityPlugin")]
        public static extern void SZVRPluginInit();

        [DllImport("SZVRUnityPlugin")]
        public static extern void SZVRPluginDestroy();

        [DllImport("SZVRUnityPlugin")]
        public static extern void SZVRPluginEnableATW();

        [DllImport("SZVRUnityPlugin")]
        public static extern void SZVRPluginDiasbleATW();

        // 新接口替换原来的GetHMDQuaternion
        [DllImport("SZVRUnityPlugin")]
        public static extern void GetTrackedPost(float[] hmd, float[] controller_left, float[] Controller_right);

        [DllImport("SZVRUnityPlugin")]
        public static extern void UpdateTextureFromUnity(System.IntPtr leftIntPtr, System.IntPtr rigthIntPtr);

        [DllImport("SZVRUnityPlugin")]
        public static extern System.IntPtr GetRenderEventFunc();

                
        // ------------------------------------------------------------手柄-----------SZVRPWandPlugin.dll
        // 默认不就是cdecl的么？
        [DllImport("SZVRWandPlugin", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SZVR_GetWandData(float[] quaternion, float[] position,
                                                    uint[] keyStatus, byte[] TriggerValue,
                                                    byte[] Stick, bool right);


    }

}
