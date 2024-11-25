using UnityEditor;
using UnityEngine;
using BillUtils;

[CustomEditor(typeof(ToolGenerateProps))]
public class ToolGeneratePropEditor : Editor
{
    private ToolGenerateProps toolGenerateProps;
    private ToolGeneratePropsGrid2D toolGeneratePropsGrid;

    private void OnEnable()
    {
        toolGenerateProps = (ToolGenerateProps)target;
    }

    private void OnSceneGUI()
    {
        // Chỉ thực hiện nếu shapeType là Custom
        if (toolGenerateProps.ShapeType != Constants.PropShape.Custom) return;

        Event e = Event.current;

        // Tính toán offset để căn giữa quanh transform.position
        float offsetX = (toolGenerateProps.GridWidth * toolGenerateProps.Size) / 2f;
        float offsetY = (toolGenerateProps.GridHeight * toolGenerateProps.Size) / 2f;

        // Vẽ grid với tickrate
        for (int x = 0; x < toolGenerateProps.GridWidth; x++)
        {
            for (int y = 0; y < toolGenerateProps.GridHeight; y++)
            {
                Vector3 cellCenter = toolGenerateProps.transform.position + 
                                    new Vector3(x * toolGenerateProps.Size - offsetX + toolGenerateProps.Size / 2, 
                                                y * toolGenerateProps.Size - offsetY + toolGenerateProps.Size / 2, 0);

                // Vẽ ô nổi bật nếu ô đó được chọn
                if (toolGenerateProps.SelectedCells.Contains(new Vector2Int(x, y)))
                {
                    Handles.color = Color.yellow;
                    Handles.DrawSolidRectangleWithOutline(
                        new Rect(cellCenter - new Vector3(toolGenerateProps.Size / 2, toolGenerateProps.Size / 2, 0),
                                new Vector2(toolGenerateProps.Size, toolGenerateProps.Size)),
                        new Color(1f, 1f, 0f, 0.2f), // Màu nền
                        Color.yellow                 // Màu viền
                    );
                }

                // Vẽ "tick" tại tâm của mỗi ô
                Handles.color = Color.green;
                Handles.DrawLine(
                    cellCenter + new Vector3(-toolGenerateProps.Size * 0.1f, 0, 0),
                    cellCenter + new Vector3(toolGenerateProps.Size * 0.1f, 0, 0)
                );
                Handles.DrawLine(
                    cellCenter + new Vector3(0, -toolGenerateProps.Size * 0.1f, 0),
                    cellCenter + new Vector3(0, toolGenerateProps.Size * 0.1f, 0)
                );
            }
        }

        // Xử lý chuột để chọn/bỏ chọn các ô trong grid
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = new Plane(Vector3.back, toolGenerateProps.transform.position);
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                Vector3 localPoint = hitPoint - toolGenerateProps.transform.position;

                int x = Mathf.FloorToInt((localPoint.x + offsetX) / toolGenerateProps.Size);
                int y = Mathf.FloorToInt((localPoint.y + offsetY) / toolGenerateProps.Size);

                if (x >= 0 && x < toolGenerateProps.GridWidth && y >= 0 && y < toolGenerateProps.GridHeight)
                {
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

        if (GUILayout.Button("Clear All Props"))
        {
            toolGenerateProps.ClearAllProps();
        }

        if (GUILayout.Button("Save Props"))
        {
            toolGenerateProps.SavePropsAsPrefab($"Assets/Model/Prefab/Level/{toolGenerateProps.propName}.prefab");
        }

    }
}
