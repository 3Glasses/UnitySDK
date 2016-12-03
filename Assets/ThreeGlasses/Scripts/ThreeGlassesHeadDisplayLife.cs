using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Runtime.InteropServices;
// ReSharper disable NotAccessedField.Local

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

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);     [DllImport("user32.dll")]

        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, IntPtr lParam);
        
        private const string UnityWindowClassName = "UnityWndClass";

#if !UNITY_EDITOR
        private IntPtr _windowHandle = IntPtr.Zero;
#endif

	    void Awake ()
        {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife init");
            ThreeGlassesDllInterface.SZVRPluginInit();
        }

        void OnApplicationQuit()
        {
            ThreeGlassesUtils.Log("ThreeGlassesHeadDisplayLife application quit");
            ThreeGlassesDllInterface.SZVRPluginDestroy();
        }

#if !UNITY_EDITOR
        public IEnumerator Start ()
        {
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
            
            yield return new WaitForSeconds(2.0f);

            if (_windowHandle != IntPtr.Zero)
            {
                SetForegroundWindow(_windowHandle);
            }
        }
#endif
    }
}
