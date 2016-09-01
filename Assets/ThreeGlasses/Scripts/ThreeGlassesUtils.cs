using System.Collections;
using UnityEngine;
using System.Collections.Generic;
// ReSharper disable CheckNamespace

namespace ThreeGlasses
{
    public class ThreeGlassesUtils : MonoBehaviour
    {
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        public static IEnumerator DelayedRun(System.Action action, YieldInstruction wait)
        {
            yield return wait;
            if (action != null)
            {
                action();
            }
        }
    }
}