using UnityEditor;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers


namespace ThreeGlasses
{
    [CustomEditor(typeof(ThreeGlassesVRCamera))]
    public class ThreeGlassesVRCameraEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw icon
            var r = EditorGUILayout.BeginVertical(GUILayout.Height(64));
            EditorGUILayout.Space();

            GUI.DrawTexture(r, ThreeGlassesHierarchyIcon.texture_eye, ScaleMode.ScaleToFit);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }
}