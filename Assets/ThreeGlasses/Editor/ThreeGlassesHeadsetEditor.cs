using UnityEditor;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

namespace ThreeGlasses
{
    [CustomEditor(typeof(ThreeGlassesHeadset))]
    public class ThreeGlassesHeadsetEditor : Editor
    {
        private SerializedProperty EnableHeadRotTracking;
        private SerializedProperty EnableHeadPosTracking;
        private SerializedProperty FixUV;
        private SerializedProperty Near;
        private SerializedProperty Far;

        private SerializedProperty EyeDistance;
        private SerializedProperty FieldOfView;

        private SerializedProperty leftCamera;
        private SerializedProperty rightCamera;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EnableHeadRotTracking = serializedObject.FindProperty("EnableHeadRotTracking");
            EnableHeadPosTracking = serializedObject.FindProperty("EnableHeadPosTracking");

            FixUV = serializedObject.FindProperty("FixUV");

            Near = serializedObject.FindProperty("Near");
            Far = serializedObject.FindProperty("Far");

            EyeDistance = serializedObject.FindProperty("EyeDistance");
            FieldOfView = serializedObject.FindProperty("FieldOfView");

            leftCamera = serializedObject.FindProperty("leftCamera");
            rightCamera = serializedObject.FindProperty("rightCamera");

            // Draw icon
            var r = EditorGUILayout.BeginVertical(GUILayout.Height(128));
            EditorGUILayout.Space();

            GUI.DrawTexture(r, ThreeGlassesHierarchyIcon.texture_headset, ScaleMode.ScaleToFit);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("SDK Version: " + ThreeGlassesInterfaces.getVersion, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(EnableHeadRotTracking);
            EditorGUILayout.PropertyField(EnableHeadPosTracking);

            EditorGUILayout.PropertyField(Near);
            EditorGUILayout.PropertyField(Far);
            EditorGUILayout.PropertyField(FixUV);

            EyeDistance.floatValue = EditorGUILayout.Slider("Eye Distance", EyeDistance.floatValue, 0.02f, 1.0f);
            FieldOfView.floatValue = EditorGUILayout.Slider("Field Of View", FieldOfView.floatValue, 0.1f, 180f);

            EditorGUILayout.PropertyField(leftCamera);
            EditorGUILayout.PropertyField(rightCamera);

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            if(target.GetType() == typeof(ThreeGlassesHeadset))
            {
                ((ThreeGlassesHeadset)target).SetCameraPos();
            }
        }
    }
}