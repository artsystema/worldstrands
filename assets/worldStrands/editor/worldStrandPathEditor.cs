using UnityEditor;
using UnityEngine;

namespace worldStrands.editor
{
    [CustomEditor(typeof(worldStrandPath))]
    public class worldStrandPathEditor : UnityEditor.Editor
    {
        SerializedProperty pointsProp;
        SerializedProperty profileProp;
        UnityEditor.Editor profileEditor;

        void OnEnable()
        {
            pointsProp = serializedObject.FindProperty("points");
            profileProp = serializedObject.FindProperty("profile");
        }

        void OnSceneGUI()
        {
            worldStrandPath path = (worldStrandPath)target;

            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                SerializedProperty pointProp = pointsProp.GetArrayElementAtIndex(i);
                Vector3 world = path.transform.TransformPoint(pointProp.vector3Value);
                EditorGUI.BeginChangeCheck();
                world = Handles.PositionHandle(world, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(path, "Move Point");
                    pointProp.vector3Value = path.transform.InverseTransformPoint(world);
                    serializedObject.ApplyModifiedProperties();
                    path.UpdateMesh();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(profileProp);

            if (profileProp.objectReferenceValue != null)
            {
                Editor.CreateCachedEditor(profileProp.objectReferenceValue, typeof(worldStrandProfileEditor), ref profileEditor);
                if (profileEditor != null)
                {
                    profileEditor.OnInspectorGUI();
                }
            }

            EditorGUILayout.PropertyField(pointsProp, true);

            if (GUILayout.Button("Update Mesh"))
            {
                ((worldStrandPath)target).UpdateMesh();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
