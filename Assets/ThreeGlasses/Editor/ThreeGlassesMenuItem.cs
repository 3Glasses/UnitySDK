#define TGDEBUG
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ThreeGlasses
{
#if UNITY_EDITOR

    public class ThreeGlassesMenuItem
    {
        const string kEnableHeadDisplay = "3Glasses/EnableHeadDisplay";

        [UnityEditor.MenuItem(kEnableHeadDisplay, true, 1)]
        public static bool ToggleSimulationModeValidate()
        {
            Menu.SetChecked(kEnableHeadDisplay, GameObject.FindObjectOfType(typeof(ThreeGlassesManager)) != null);
            return true;
        }

        [UnityEditor.MenuItem(kEnableHeadDisplay, false, 1)]
		public static void ToggleSimulationMode ()
		{
            if(GameObject.FindObjectOfType(typeof(ThreeGlassesManager)) != null)
            {
                clear3Glasses();
            }
            else
            {
                clear3Glasses();
                add3Glasses();
            }
        }

        public static void add3Glasses()
        {
            // bind camera
            Camera[] cams = GameObject.FindObjectsOfType<Camera>();
            float depth = 0;
            Camera cam;
            if(cams.Length > 0)
            {
                depth = cams[0].depth;
                cam = cams[0];
                foreach (var item in cams)
                {
                    if(item.depth < depth)
                    {
                        depth = item.depth;
                        cam = item;
                    }
                }
            }
            else
            {
                GameObject mainCamera = new GameObject("MainCamera");
                Undo.RegisterCreatedObjectUndo(mainCamera, "create camera");
                cam = Undo.AddComponent<Camera>(mainCamera);
            }

            // bind script
            var tgm = Undo.AddComponent<ThreeGlassesManager>(cam.gameObject);
            tgm.cloneTargetCamera = cam;

            Selection.activeGameObject = cam.gameObject;
        }
        public static void clear3Glasses()
        {
            // remove ThreeGlassesCamera form maincamera
            Component[] objects = GameObject.FindObjectsOfType<ThreeGlassesManager>();
            foreach (var item in objects)
            {
                Undo.DestroyObjectImmediate(item);
            }
            // remove ThreeGlassesSubCamera
            objects = GameObject.FindObjectsOfType<ThreeGlassesSubCamera>();
            foreach (var item in objects)
            {
                Undo.DestroyObjectImmediate(item);
            }
        }
    }
#endif
}
