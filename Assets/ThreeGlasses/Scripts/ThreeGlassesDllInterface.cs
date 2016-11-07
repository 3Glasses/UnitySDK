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

        [DllImport("SZVRUnityPlugin")]
        public static extern void GetHMDQuaternion(float[] input);

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


//         // 下面的都可以不用看都是暂时没有用到的
// 
//         // -------------------------------------------------------屏幕设置相关-----------SZVRPlugin.dll
//         // 获取头显的位置，已被注释了，为什么和旋转数据放在一起？？
//         [DllImport("SZVRPlugin")]
//         private static extern bool SZVR_GetData(float[] input, float[] output);
//         //Pay app  SZVR_PayApp不知道是用于什么
//         [DllImport("SZVRPlugin", CallingConvention = CallingConvention.Cdecl)]
//         private static extern bool SZVR_PayApp(string appKey);
// 
//         // -------------------------------------------------------屏幕设置相关-----------user32.dll
//         // 设置窗口风格
//         [DllImport("user32.dll")]
//         private static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
// 
//         // 设置窗口z序，位置，大小等
//         [DllImport("user32.dll")]
//         private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
// 
//         // 获取当前应用窗口
//         [DllImport("user32.dll")]
//         static extern IntPtr GetActiveWindow();
// 
//         // 获取窗口位置和大小
//         [DllImport("user32.dll")]
//         [return: MarshalAs(UnmanagedType.Bool)]
//         static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
// 
//         #region InternalDataStructure
//         //Window Rect
//         [StructLayout(LayoutKind.Sequential)]
//         private struct RECT
//         {
//             public int Left;
//             public int Top;
//             public int Right;
//             public int Bottom;
//         }
//         #endregion
// 
//         #region windows
//         // 唯一调用的函数没被任何其他函数调用，也就是说没有在任何地方有使用
//         //Get the position and resolution of the headset under MS windows OS
//         internal static Vector4 GetScreenPosRes()
//         {
//             var inputs = new float[1];
//             inputs[0] = 0;
//             var result = new float[4];
//             var res = new Vector4();
//             if (!SZVR_GetData(inputs, result)) return res;
//             res.x = result[0];
//             res.y = result[1];
//             res.z = result[2];
//             res.w = result[3];
//             return res;
//         }
// 
//         #endregion
// 
//         
//         // 使用的地方被注释掉了，也就是说没有在任何地方有使用
//         //Get Headset rotation Quaternion
//         public static Quaternion GetCameraOrientation()
//         {
//             var rotation = Quaternion.identity;
//             var inputs = new[] { 1.0f, Time.deltaTime * 1000 };
// 
//             var result = new float[4];
//             if (!SZVR_GetData(inputs, result)) return rotation;
//             rotation.x = -result[0];
//             rotation.y = -result[1];
//             rotation.z = result[2];
//             rotation.w = result[3];
//             return rotation;
//         }
//         // 使用的地方被注释掉了，也就是说没有在任何地方有使用
//         //Get Headset position
//         public static Vector3 GetCameraPosition()
//         {
//             var position = Vector3.zero;
// 
//             var inputs = new float[1];
//             inputs[0] = 6;
//             var result = new float[3];
// 
//             if (!SZVR_GetData(inputs, result)) return position;
// 
//             position.x = checkFloat(result[0]) ? result[0] : 0;
//             position.y = checkFloat(result[1]) ? result[1] : 0;
//             position.z = checkFloat(result[2]) ? result[2] : 0;
// 
//             return position;
//         }
//         /// <summary>
//         /// Set current application window to headset display
//         /// </summary>
//         // 无任何地方调用
//         public static void SetPositionAndResolution()
//         {
//             var posRes = GetScreenPosRes();
//             Screen.SetResolution((int)posRes[2], (int)posRes[3], false);
//             // 设置一个新的窗口风格
//             SetWindowLong(GetActiveWindow(), GWL_STYLE, WS_BORDER);
//             // 设置窗体位置和大小以及z序
//             SetWindowPos(GetActiveWindow(), 0, (int)posRes[0], (int)posRes[1], (int)posRes[2], (int)posRes[3], SWP_SHOWWINDOW);
//         }
// 
// 
//         /// <summary>
//         /// Pay app interfaces.
//         /// </summary>
//         /// <param name="appKey">app id</param>
//         /// <returns></returns>
//         public static bool PayApp(string appKey)
//         {
//             var status = false;
//             try
//             {
//                 status = SZVR_PayApp(appKey);
//             }
//             catch (Exception)
//             {
//                 status = false;
//             }
// 
//             return status;
//         }
    }

}
