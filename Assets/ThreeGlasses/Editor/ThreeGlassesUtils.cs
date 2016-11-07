using UnityEngine;
using System.Collections;
using System.Diagnostics; // 注意：这是为了使用包含在此名称空间中的ConditionalAttribute特性

namespace ThreeGlasses
{
    static class ThreeGlassesUtils
    {
        // 复写log
        [ConditionalAttribute("TGDEBUG")]
        public static void Log(object msg)
        {
            UnityEngine.Debug.Log(msg);
        }
    }
}
