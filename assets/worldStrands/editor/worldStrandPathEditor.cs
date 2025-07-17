using UnityEditor;
using UnityEngine;

namespace WorldStrands.Editor
{
    [CustomEditor(typeof(WorldStrandPath))]
    public class WorldStrandPathEditor : UnityEditor.Editor
    {
        SerializedProperty pointsProp;

        void OnEnable()
        {
            pointsProp = serializedObject.FindProperty("points");
        }

        void OnSceneGUI()
        {
            WorldStrandPath path = (WorldStrandPath)target;

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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("profile"));
            EditorGUILayout.PropertyField(pointsProp, true);

            if (GUILayout.Button("Update Mesh"))
            {
                ((WorldStrandPath)target).UpdateMesh();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
