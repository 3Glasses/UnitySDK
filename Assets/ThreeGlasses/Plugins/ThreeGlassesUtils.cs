using UnityEngine;
using System.Collections;
using System.Diagnostics; // for ConditionalAttribute

namespace ThreeGlasses
{
    public static class ThreeGlassesUtils
    {
        // 复写log
        [ConditionalAttribute("TGDEBUG")]
        public static void Log(object msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public static Component CopyComponent(Component original, GameObject destination)
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            // Copied fields can be restricted with BindingFlags
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy;

        }
    }

}