using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable FieldCanBeMadeReadOnly.Local


namespace ThreeGlasses
{
    [InitializeOnLoad]
    public class ThreeGlassesHierarchyIcon
    {
        public static Texture2D texture_eye;
        public static Texture2D texture_wand;
        public static Texture2D texture_headset;

        static List<int> eyeObjects;
        static List<int> wandObjects;
        static List<int> headsetObjects;

        static ThreeGlassesHierarchyIcon()
        {
            texture_eye     = Resources.Load<Texture2D>(ThreeGlassesConst.ImageResourcesPath + "eye");
            texture_wand    = Resources.Load<Texture2D>(ThreeGlassesConst.ImageResourcesPath + "wand");
            texture_headset = Resources.Load<Texture2D>(ThreeGlassesConst.ImageResourcesPath + "headset");

            eyeObjects = new List<int>();
            wandObjects = new List<int>();
            headsetObjects = new List<int>();

            EditorApplication.update += UpdateCB;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        }

        static void UpdateCB()
        {
            var headObjs = Object.FindObjectsOfType<ThreeGlassesHeadset>();
            var eyeObjs  = Object.FindObjectsOfType<ThreeGlassesVRCamera>();
            var wandObjs = Object.FindObjectsOfType<ThreeGlassesWand>();

            eyeObjects.Clear();
            wandObjects.Clear();
            headsetObjects.Clear();

            if (headObjs != null)
            {
                foreach (var hobj in headObjs)
                {
                    headsetObjects.Add(hobj.gameObject.GetInstanceID());
                }
            }

            if (eyeObjs != null)
            {
                foreach (var eyeObj in eyeObjs)
                {
                    eyeObjects.Add(eyeObj.gameObject.GetInstanceID());
                }
            }

            if (wandObjs == null) return;
            foreach (var wandobj in wandObjs)
            {
                wandObjects.Add(wandobj.gameObject.GetInstanceID());
            }
        }

        static void HierarchyItemCB(int instanceID, Rect selectionRect)
        {
            var r = new Rect(selectionRect);
            r.x = r.width - 15;
            r.width = 25;
            r.height = 20;

            if (eyeObjects.Contains(instanceID))
            {
                GUI.Label(r, texture_eye);
            }

            if (wandObjects.Contains(instanceID))
            {
                GUI.Label(r, texture_wand);
            }

            if (headsetObjects.Contains(instanceID))
            {
                GUI.Label(r, texture_headset);
            }
        }
    }
}