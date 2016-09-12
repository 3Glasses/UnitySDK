using UnityEngine;
using ThreeGlasses;
using UnityEditor;

// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

#if UNITY_EDITOR

[InitializeOnLoad]
public class ThreeGlassesVRCompositor : MonoBehaviour
{
    static ThreeGlassesVRCompositor()
    {
        EditorApplication.update += Update;
    }

    public static void Update ()
	{
	    ThreeGlassesHeadset.PluginHandleWindowMsg();
	}
}

#endif