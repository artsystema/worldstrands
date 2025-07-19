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
        // Make draggingIndex a field so it persists between events
        private int draggingIndex = -1;
        private int selectedProfileIndex = -1;

        void OnEnable()
        {
            pointsProp = serializedObject.FindProperty("points");
            profileProp = serializedObject.FindProperty("profile");
            scaleProp = serializedObject.FindProperty("profileScale");

            // Ensure profile has at least 2 valid points with normalized x/y and color
            if (profileProp != null && profileProp.isArray)
            {
                if (profileProp.arraySize < 2)
                {
                    profileProp.arraySize = 2;
                    for (int i = 0; i < 2; i++)
                    {
                        SerializedProperty p = profileProp.GetArrayElementAtIndex(i);
                        SerializedProperty xProp = p.FindPropertyRelative("x");
                        SerializedProperty yProp = p.FindPropertyRelative("y");
                        SerializedProperty colorProp = p.FindPropertyRelative("color");
                        if (xProp != null) xProp.floatValue = (i == 0) ? 0f : 1f;
                        if (yProp != null) yProp.floatValue = (i == 0) ? 0f : 1f;
                        if (colorProp != null) colorProp.colorValue = Color.white;
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        void OnSceneGUI()
        {
            worldStrandPath path = (worldStrandPath)target;
            for (int i = 0; i < pointsProp.arraySize; i++)
            {
                SerializedProperty pointProp = pointsProp.GetArrayElementAtIndex(i);
                SerializedProperty posProp = pointProp.FindPropertyRelative("position");
                SerializedProperty rotProp = pointProp.FindPropertyRelative("rotation");
                Vector3 world = path.transform.TransformPoint(posProp.vector3Value);
                Quaternion worldRot = path.transform.rotation * rotProp.quaternionValue;
                EditorGUI.BeginChangeCheck();
                world = Handles.PositionHandle(world, worldRot);
                worldRot = Handles.RotationHandle(worldRot, world);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(path, "Move/Rotate Point");
                    posProp.vector3Value = path.transform.InverseTransformPoint(world);
                    rotProp.quaternionValue = Quaternion.Inverse(path.transform.rotation) * worldRot;
                    serializedObject.ApplyModifiedProperties();
                    path.UpdateMesh();
                }
            }
        }

        void DrawProfileCurve(SerializedProperty list)
        {
            // Use fixed preview window size
            Rect rect = GUILayoutUtility.GetRect(100, 250);
            GUI.Box(rect, GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.xMin, rect.yMin - 20, rect.width, 18), "Profile Preview", EditorStyles.boldLabel);

            // Display total count of vertices in top-right corner
            int vertexCount = 0;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty xProp = list.GetArrayElementAtIndex(i).FindPropertyRelative("x");
                SerializedProperty yProp = list.GetArrayElementAtIndex(i).FindPropertyRelative("y");
                SerializedProperty colorProp = list.GetArrayElementAtIndex(i).FindPropertyRelative("color");
                if (xProp != null && yProp != null && colorProp != null)
                    vertexCount++;
            }
            EditorGUI.LabelField(new Rect(rect.xMax - 60, rect.yMin + 4, 60, 18), $"Count: {vertexCount}", EditorStyles.miniBoldLabel);

            int validPoints = 0;
            float minX = 0f, maxX = 1f;
            float minY = 0f, maxY = 1f;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty point = list.GetArrayElementAtIndex(i);
                SerializedProperty xProp = point.FindPropertyRelative("x");
                SerializedProperty yProp = point.FindPropertyRelative("y");
                SerializedProperty colorProp = point.FindPropertyRelative("color");
                if (xProp == null || yProp == null || colorProp == null)
                    continue;
                float x = Mathf.Clamp01(xProp.floatValue);
                float y = Mathf.Clamp01(yProp.floatValue);
                validPoints++;
            }
            if (validPoints < 2)
            {
                EditorGUI.LabelField(new Rect(rect.xMin, rect.yMin + 60, rect.width, 18), "Not enough valid profile points.", EditorStyles.boldLabel);
                return;
            }

            Handles.BeginGUI();
            Vector3 prev = Vector3.zero;
            for (int i = 0; i < list.arraySize; i++)
            {
                SerializedProperty p = list.GetArrayElementAtIndex(i);
                SerializedProperty xProp = p.FindPropertyRelative("x");
                SerializedProperty yProp = p.FindPropertyRelative("y");
                SerializedProperty colorProp = p.FindPropertyRelative("color");
                if (xProp == null || yProp == null || colorProp == null)
                    continue;
                float x = Mathf.Clamp01(xProp.floatValue);
                float y = Mathf.Clamp01(yProp.floatValue);
                Color c = colorProp.colorValue;
                Vector2 pos = new Vector2(
                    Mathf.Lerp(rect.xMin, rect.xMax, x),
                    Mathf.Lerp(rect.yMax, rect.yMin, y));

                if (i > 0)
                {
                    Handles.color = Color.gray;
                    Handles.DrawLine(prev, pos);
                }

                Handles.color = new Color(c.r, c.g, c.b, 1f); // Ignore alpha for display
                float handleSize = 8f;
                Rect handleRect = new Rect(pos.x - handleSize * 0.5f, pos.y - handleSize * 0.5f, handleSize, handleSize);
                Handles.DrawSolidDisc(pos, Vector3.forward, handleSize * 0.5f);

                // Draw index next to handle
                Rect indexRect = new Rect(pos.x - 10, pos.y - 18, 20, 16);
                EditorGUI.LabelField(indexRect, i.ToString(), EditorStyles.miniLabel);

                // Show both x and y below the handle
                var displayCoordinates = ((worldStrandPath)target).displayCoordinates;
                if (displayCoordinates)
                {
                    Rect labelRect = new Rect(pos.x - 40, pos.y + handleSize * 0.5f, 80, 18);
                    EditorGUI.LabelField(labelRect, $"x:{xProp.floatValue:F2} y:{yProp.floatValue:F2}", EditorStyles.miniLabel);
                }

                if (Event.current.type == EventType.MouseDown && handleRect.Contains(Event.current.mousePosition))
                {
                    // Only start dragging if left mouse button
                    if (Event.current.button == 0)
                    {
                        draggingIndex = i;
                        selectedProfileIndex = i;
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                }
                if (draggingIndex == i && Event.current.type == EventType.MouseDrag)
                {
                    float newX = Mathf.Clamp01(Mathf.InverseLerp(rect.xMin, rect.xMax, Event.current.mousePosition.x));
                    float newY = Mathf.Clamp01(Mathf.InverseLerp(rect.yMax, rect.yMin, Event.current.mousePosition.y));
                    xProp.floatValue = newX;
                    yProp.floatValue = newY;
                    colorProp.colorValue = c;
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    GUI.changed = true;
                }
                if (draggingIndex == i && Event.current.type == EventType.MouseUp)
                {
                    serializedObject.ApplyModifiedProperties();
                    draggingIndex = -1;
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.ContextClick && handleRect.Contains(Event.current.mousePosition))
                {
                    // Only allow deletion if there are more than 2 points
                    if (list.arraySize > 2)
                    {
                        list.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                    }
                    Event.current.Use();
                    break;
                }

                prev = pos;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && rect.Contains(Event.current.mousePosition))
            {
                list.arraySize++;
                SerializedProperty np = list.GetArrayElementAtIndex(list.arraySize - 1);
                SerializedProperty xProp = np.FindPropertyRelative("x");
                SerializedProperty yProp = np.FindPropertyRelative("y");
                SerializedProperty colorProp = np.FindPropertyRelative("color");
                if (xProp != null && yProp != null && colorProp != null)
                {
                    float x = Mathf.Clamp01(Mathf.InverseLerp(rect.xMin, rect.xMax, Event.current.mousePosition.x));
                    float y = Mathf.Clamp01(Mathf.InverseLerp(rect.yMax, rect.yMin, Event.current.mousePosition.y));
                    xProp.floatValue = x;
                    yProp.floatValue = y;
                    colorProp.colorValue = Color.white;
                    serializedObject.ApplyModifiedProperties();
                }
                Event.current.Use();
            }

            Handles.EndGUI();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("displayCoordinates"), new GUIContent("Display Coordinates"));
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(scaleProp);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("profileStep"), new GUIContent("Path Detail"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sagging"), new GUIContent("Sagging"));
            GUILayout.Space(20);
            DrawProfileCurve(profileProp);



            if (selectedProfileIndex >= 0 && selectedProfileIndex < profileProp.arraySize)
            {
                SerializedProperty p = profileProp.GetArrayElementAtIndex(selectedProfileIndex);
                SerializedProperty colorProp = p.FindPropertyRelative("color");
                if (colorProp != null)
                {
                    Color newColor = EditorGUILayout.ColorField($"Point [{selectedProfileIndex}] Color", colorProp.colorValue);
                    if (newColor != colorProp.colorValue)
                    {
                        colorProp.colorValue = newColor;
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(pointsProp, true);

            if (GUILayout.Button("Update Mesh"))
            {
                ((worldStrandPath)target).UpdateMesh();
            }
            if (GUILayout.Button("Bake Mesh"))
            {
                string path = UnityEditor.EditorUtility.SaveFilePanel("Export OBJ", "Assets", "StrandMesh.obj", "obj");
                if (!string.IsNullOrEmpty(path))
                {
                    ((worldStrandPath)target).BakeMeshToObj(path);
                }
            }
            if (GUILayout.Button("Bake Mesh (FBX)"))
            {
                string path = UnityEditor.EditorUtility.SaveFilePanel("Export FBX", "Assets", "StrandMesh.fbx", "fbx");
                if (!string.IsNullOrEmpty(path))
                {
                    ((worldStrandPath)target).BakeMeshToFbx(path);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
