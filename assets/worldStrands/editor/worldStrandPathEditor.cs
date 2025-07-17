using UnityEditor;
using UnityEngine;

namespace worldStrands.editor
{
    [CustomEditor(typeof(worldStrandPath))]
    public class worldStrandPathEditor : UnityEditor.Editor
    {
        SerializedProperty pointsProp;
        SerializedProperty profileProp;
        SerializedProperty scaleProp;

        void OnEnable()
        {
            pointsProp = serializedObject.FindProperty("points");
            profileProp = serializedObject.FindProperty("profile");
            scaleProp = serializedObject.FindProperty("profileScale");
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

            EditorGUILayout.PropertyField(scaleProp);
            DrawProfileCurve(profileProp);

            EditorGUILayout.PropertyField(pointsProp, true);

            if (GUILayout.Button("Update Mesh"))
            {
                ((worldStrandPath)target).UpdateMesh();
            }

            serializedObject.ApplyModifiedProperties();
        }

        void DrawProfileCurve(SerializedProperty list)
        {
            Rect rect = GUILayoutUtility.GetRect(200, 150);
            GUI.Box(rect, GUIContent.none);

            if (list.arraySize < 2)
                return;

            float minY = float.MaxValue;
            float maxY = float.MinValue;
            for (int i = 0; i < list.arraySize; i++)
            {
                float y = list.GetArrayElementAtIndex(i).FindPropertyRelative("y").floatValue;
                minY = Mathf.Min(minY, y);
                maxY = Mathf.Max(maxY, y);
            }
            if (Mathf.Approximately(minY, maxY))
            {
                maxY = minY + 1f;
            }

            Handles.BeginGUI();
            Vector3 prev = Vector3.zero;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty p = list.GetArrayElementAtIndex(i);
                float y = p.FindPropertyRelative("y").floatValue;
                Color c = p.FindPropertyRelative("color").colorValue;

                float t = (float)i / (list.arraySize - 1);
                Vector2 pos = new Vector2(Mathf.Lerp(rect.xMin, rect.xMax, t),
                    Mathf.Lerp(rect.yMax, rect.yMin, Mathf.InverseLerp(minY, maxY, y)));

                if (i > 0)
                {
                    Handles.color = Color.gray;
                    Handles.DrawLine(prev, pos);
                }

                EditorGUI.BeginChangeCheck();
                Handles.color = c;
                // Use the 3D FreeMoveHandle variant to avoid a null reference
                // exception that occurs with the 2D overload in some Unity
                // versions when drawing inside the inspector.
                Vector3 newPos3 = Handles.FreeMoveHandle(
                    new Vector3(pos.x, pos.y, 0f),
                    Quaternion.identity,
                    4f,
                    Vector3.zero,
                    Handles.DotHandleCap);
                Vector2 newPos = new Vector2(newPos3.x, newPos3.y);
                if (EditorGUI.EndChangeCheck())
                {
                    float newY = Mathf.Lerp(minY, maxY, Mathf.InverseLerp(rect.yMax, rect.yMin, newPos.y));
                    p.FindPropertyRelative("y").floatValue = newY;
                }

                if (Event.current.type == EventType.ContextClick && Vector2.Distance(Event.current.mousePosition, pos) < 5f)
                {
                    list.DeleteArrayElementAtIndex(i);
                    Event.current.Use();
                    break;
                }

                prev = pos;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition))
            {
                list.arraySize++;
                SerializedProperty np = list.GetArrayElementAtIndex(list.arraySize - 1);
                float y = Mathf.Lerp(minY, maxY, Mathf.InverseLerp(rect.yMax, rect.yMin, Event.current.mousePosition.y));
                np.FindPropertyRelative("y").floatValue = y;
                np.FindPropertyRelative("color").colorValue = Color.white;
                Event.current.Use();
            }

            Handles.EndGUI();
        }
    }
}
