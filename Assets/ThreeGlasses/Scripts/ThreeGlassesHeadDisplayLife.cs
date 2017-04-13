//#define VR_SHOW
using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Runtime.InteropServices;


namespace ThreeGlasses
{
    public class ThreeGlassesHeadDisplayLife : MonoBehaviour
    {
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern IntPtr SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern IntPtr ShowWindow(IntPtr hwnd, int cmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);


        private const string UnityWindowClassName = "UnityWndClass";

#if !UNITY_EDITOR
        private IntPtr _windowHandle = IntPtr.Zero;
        private const int SW_SHOW = 5;
        private const int GWL_STYLE = -16;
        private const int WS_BORDER = 0x00800000;
        private const int SW_MAXIMIZE = 3;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_SHOWWINDOW = 0x0040;
#endif

        public static uint renderWidth { get; private set; }
        public static uint renderHeight { get; private set; }
        public static float scaleRenderSize = 1.3f;
        public static bool AsynchronousProjection = false;

        void Awake()
        {
            renderWidth = 2048;
            renderHeight = 1024;

            #if !UNITY_EDITOR
            var threadId = GetCurrentThreadId();
            EnumThreadWindows(threadId, (hWnd, lParam) =>
            {
                var classText = new StringBuilder(UnityWindowClassName.Length + 1);
                GetClassName(hWnd, classText, classText.Capacity);
                if (classText.ToString() != UnityWindowClassName)
                    return true;
                _windowHandle = hWnd;
                return false;
            }, IntPtr.Zero);
            #endif

            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife init");

            uint[] buffsize = { renderWidth, renderHeight };
            ThreeGlassesDllInterface.GetNativeRenderSize(buffsize);
            renderWidth = (uint)(scaleRenderSize * buffsize[0]);
            renderHeight = (uint)(scaleRenderSize * buffsize[1]);

            renderWidth = renderWidth - (renderWidth % 16);
            renderHeight = renderHeight - (renderHeight % 16);

            ThreeGlassesDllInterface.SZVRPluginInit(
                (uint)(AsynchronousProjection ? 0 : 1),
                renderWidth,
                renderHeight);

#if !UNITY_EDITOR
            if (_windowHandle != IntPtr.Zero)
            {
                ShowWindow(_windowHandle, SW_SHOW);
                SetForegroundWindow(_windowHandle);
            }
#endif
        }

        void OnApplicationQuit()
        {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife application quit");
            ThreeGlassesDllInterface.SZVRPluginDestroy();
        }
    }
}
