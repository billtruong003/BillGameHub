using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToolGeneratePropsGrid2D))]
public class ToolGeneratePropsGrid2DEditor : Editor
{
    private ToolGeneratePropsGrid2D tool;

    private void OnEnable()
    {
        tool = (ToolGeneratePropsGrid2D)target;
    }

    private void OnSceneGUI()
    {
        // Tính toán vị trí gốc của lưới (pivot là transform.position)
        Vector3 gridOrigin = tool.transform.position - new Vector3(
            (tool.GridWidth * tool.CellSize) / 2f - tool.CellSize / 2f,
            (tool.GridHeight * tool.CellSize) / 2f - tool.CellSize / 2f,
            0);

        // Vẽ lưới và xử lý trạng thái của các ô
        for (int x = 0; x < tool.GridWidth; x++)
        {
            for (int y = 0; y < tool.GridHeight; y++)
            {
                Vector3 cellCenter = gridOrigin + new Vector3(
                    x * tool.CellSize,
                    y * tool.CellSize,
                    0);

                // Vẽ grid nhẹ
                Handles.color = Color.green;
                Handles.DrawWireCube(cellCenter, new Vector3(tool.CellSize, tool.CellSize, 0));

                // Vẽ ô nếu nó có trạng thái
                ToolGeneratePropsGrid2D.GridCell cell = tool.GridCells.Find(c => c.Position == new Vector2Int(x, y));
                if (cell != null && cell.PropCustomIndex >= 0 && cell.PropCustomIndex < tool.PropCustoms.Count)
                {
                    var customProp = tool.PropCustoms[cell.PropCustomIndex];
                    Handles.color = customProp.ColorRect;

                    Vector3 rectPosition = cellCenter - new Vector3(tool.CellSize * 0.4f, tool.CellSize * 0.4f, 0);
                    Vector2 rectSize = new Vector2(tool.CellSize * 0.8f, tool.CellSize * 0.8f);

                    Handles.DrawSolidRectangleWithOutline(
                        new Rect(rectPosition, rectSize),
                        new Color(Handles.color.r, Handles.color.g, Handles.color.b, 0.3f), // Màu nền
                        Handles.color // Màu viền
                    );
                }
            }
        }

        // Xử lý chuột để chọn/bỏ chọn các ô trong grid
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Plane plane = new Plane(Vector3.back, tool.transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                Vector3 localPoint = hitPoint - gridOrigin;

                int x = Mathf.FloorToInt((localPoint.x + tool.CellSize / 2) / tool.CellSize);
                int y = Mathf.FloorToInt((localPoint.y + tool.CellSize / 2) / tool.CellSize);

                if (x >= 0 && x < tool.GridWidth && y >= 0 && y < tool.GridHeight)
                {
                    tool.ToggleCell(new Vector2Int(x, y));
                    EditorUtility.SetDirty(tool);
                    e.Use();
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Props"))
        {
            tool.GenerateProps();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            tool.ClearGrid();
            EditorUtility.SetDirty(tool);
        }

        if (GUILayout.Button("Clear Props"))
        {
            tool.ClearProps();
        }

        if (GUILayout.Button("Save Props"))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Props Prefab",
                "NewProps",
                "prefab",
                "Directory prefab"
            );

            if (!string.IsNullOrEmpty(path))
            {
                tool.SaveProps(path);
            }
        }
    }
}
