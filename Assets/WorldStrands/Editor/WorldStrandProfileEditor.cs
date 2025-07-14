using UnityEditor;
using UnityEngine;

namespace WorldStrands.Editor
{
    [CustomEditor(typeof(WorldStrandProfile))]
    public class WorldStrandProfileEditor : UnityEditor.Editor
    {
        SerializedProperty pointsProp;

        void OnEnable()
        {
            pointsProp = serializedObject.FindProperty("points");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Profile Points", EditorStyles.boldLabel);

            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                SerializedProperty point = pointsProp.GetArrayElementAtIndex(i);
                SerializedProperty yProp = point.FindPropertyRelative("y");
                SerializedProperty colorProp = point.FindPropertyRelative("color");

                EditorGUILayout.BeginHorizontal();
                yProp.floatValue = EditorGUILayout.FloatField(yProp.floatValue);
                colorProp.colorValue = EditorGUILayout.ColorField(colorProp.colorValue);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    pointsProp.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Point"))
            {
                pointsProp.arraySize++;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
