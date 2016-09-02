using System.Collections;
using UnityEngine;
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

        public static Mesh CreateMesh(float width, float height)
        {
            var m = new Mesh
            {
                vertices = new[]
                {
                    new Vector3(-width, -height, 0.01f),
                    new Vector3(width, -height, 0.01f),
                    new Vector3(width, height, 0.01f),
                    new Vector3(-width, height, 0.01f)
                },
                uv = new[]
                {
                    new Vector2(0, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                triangles = new[] {0, 1, 2, 0, 2, 3}
            };
            m.RecalculateNormals();

            return m;
        }

    }
}