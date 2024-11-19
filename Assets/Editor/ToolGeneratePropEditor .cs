using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolGenerateProps))]
public class ToolGeneratePropEditor : Editor
{
    private ToolGenerateProps toolGenerateProps;

    private void OnEnable()
    {
        toolGenerateProps = (ToolGenerateProps)target;
    }

    private void OnSceneGUI()
    {
        // Chỉ thực hiện nếu shapeType là Custom
        if (toolGenerateProps.ShapeType != PropShape.Custom) return;

        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = new Plane(Vector3.back, toolGenerateProps.transform.position);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                Vector3 localPoint = hitPoint - toolGenerateProps.transform.position;

                int x = Mathf.FloorToInt(localPoint.x / toolGenerateProps.Size);
                int y = Mathf.FloorToInt(localPoint.y / toolGenerateProps.Size);

                Vector2Int cell = new Vector2Int(x, y);

                // Thêm hoặc xóa ô trong danh sách selectedCells
                if (toolGenerateProps.SelectedCells.Contains(cell))
                {
                    toolGenerateProps.SelectedCells.Remove(cell);
                }
                else
                {
                    toolGenerateProps.SelectedCells.Add(cell);
                }

                // Đánh dấu đối tượng là đã thay đổi để lưu trạng thái
                EditorUtility.SetDirty(toolGenerateProps);

                e.Use();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Props"))
        {
            toolGenerateProps.GenerateProps();
        }

        if (GUILayout.Button("Clear Custom Grid"))
        {
            toolGenerateProps.SelectedCells.Clear();
            EditorUtility.SetDirty(toolGenerateProps);
        }
    }
}
