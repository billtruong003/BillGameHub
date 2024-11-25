using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[CustomEditor(typeof(ToolGeneratePropsGrid3D))]
public class ToolGeneratePropsGrid3DEditor : Editor
{
    private ToolGeneratePropsGrid3D tool;
    private Mesh cubeMesh;
    private Dictionary<Color, Material> materialCache = new Dictionary<Color, Material>();

    private void OnEnable()
    {
        tool = (ToolGeneratePropsGrid3D)target;

        // Tạo một cube tạm thời để lấy mesh
        GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(tempCube);
    }

    private void OnDisable()
    {
        // Xóa các material đã tạo để tránh rò rỉ bộ nhớ
        foreach (var mat in materialCache.Values)
        {
            DestroyImmediate(mat);
        }
        materialCache.Clear();
    }

    private void OnSceneGUI()
    {
        // Vẽ grid để hiển thị
        if (tool.ShowGrid)
        {
            DrawGrid();
        }
    }

    private void DrawGrid()
    {
        // Tính toán gốc của grid
        Vector3 gridOrigin = tool.GetGridOrigin();

        // Vẽ các đường grid
        Handles.color = tool.GridColor; // Sử dụng màu từ Display Settings

        for (int x = 0; x < tool.GridWidth; x++)
        {
            for (int y = 0; y < tool.GridHeight; y++)
            {
                for (int z = 0; z < tool.GridDepth; z++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);
                    Vector3 cellCenter = gridOrigin + new Vector3(
                        x * tool.CellSize,
                        y * tool.CellSize,
                        z * tool.CellSize
                    );

                    // Vẽ ô grid
                    Handles.color = tool.GridColor;
                    Handles.DrawWireCube(cellCenter, Vector3.one * tool.CellSize);
                }
            }
        }

        // Vẽ các ô đã chọn
        DrawSelectedCells();

        // Vẽ các vùng tương tác
        DrawInteractionHandles();
    }

    private void DrawSelectedCells()
    {
        Vector3 gridOrigin = tool.GetGridOrigin();

        foreach (var cell in tool.GridCells)
        {
            if (cell.PropCustomIndex >= 0 && cell.PropCustomIndex < tool.PropCustoms.Count)
            {
                Vector3 cellCenter = gridOrigin + new Vector3(
                    cell.Position.x * tool.CellSize,
                    cell.Position.y * tool.CellSize,
                    cell.Position.z * tool.CellSize
                );

                var customProp = tool.PropCustoms[cell.PropCustomIndex];

                // Lấy hoặc tạo material cho màu sắc này
                Material tempMaterial;
                if (!materialCache.TryGetValue(customProp.ColorRect, out tempMaterial))
                {
                    tempMaterial = new Material(Shader.Find("Standard"));
                    tempMaterial.color = customProp.ColorRect;

                    // Thiết lập chế độ blend để hỗ trợ transparency
                    tempMaterial.SetFloat("_Mode", 3); // 3 = Transparent
                    tempMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    tempMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    tempMaterial.SetInt("_ZWrite", 0); // Tắt ghi vào depth buffer
                    tempMaterial.DisableKeyword("_ALPHATEST_ON");
                    tempMaterial.EnableKeyword("_ALPHABLEND_ON"); // Kích hoạt blending dựa trên alpha
                    tempMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    tempMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent; // Đặt renderQueue là Transparent
                    tempMaterial.hideFlags = HideFlags.HideAndDontSave;

                    // Thiết lập độ bóng và metallic
                    tempMaterial.SetFloat("_Glossiness", 0.5f); // Không bóng
                    tempMaterial.SetFloat("_Metallic", 0.5f); // Không metallic
                    tempMaterial.SetFloat("_Roughness", 0.5f); // Transparent

                    materialCache[customProp.ColorRect] = tempMaterial;
                }

                // Tính toán ma trận biến đổi cho cube
                float selectedSize = tool.CellSize * 0.8f;
                Matrix4x4 matrix = Matrix4x4.TRS(cellCenter, Quaternion.identity, Vector3.one * selectedSize);

                // Vẽ mesh của cube
                tempMaterial.SetPass(0);
                Graphics.DrawMeshNow(cubeMesh, matrix);
            }
        }
    }

    private void DrawInteractionHandles()
    {
        // Lưu trạng thái zTest hiện tại
        var originalZTest = Handles.zTest;

        // Thiết lập zTest để các nút luôn hiển thị lên trên
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        Vector3 gridOrigin = tool.GetGridOrigin();

        for (int x = 0; x < tool.GridWidth; x++)
        {
            for (int y = 0; y < tool.GridHeight; y++)
            {
                for (int z = 0; z < tool.GridDepth; z++)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, z);
                    Vector3 cellCenter = gridOrigin + new Vector3(
                        x * tool.CellSize,
                        y * tool.CellSize,
                        z * tool.CellSize
                    );

                    float interactionSize = tool.CellSize * 0.6f;

                    // Kiểm tra nếu ô đã được chọn
                    if (tool.GetCell(gridPosition) != null)
                    {
                        // Set alpha về 0 để vùng tương tác vô hình nhưng vẫn tương tác được
                        Handles.color = new Color(1f, 1f, 1f, 0f);
                    }
                    else
                    {
                        // Vùng tương tác mờ cho các ô chưa được chọn
                        Handles.color = new Color(1f, 1f, 1f, 0.1f);
                    }

                    if (Handles.Button(cellCenter, Quaternion.identity, interactionSize / 2f, interactionSize / 2f, Handles.CubeHandleCap))
                    {
                        Undo.RegisterCompleteObjectUndo(tool, "Toggle Cell");
                        tool.ToggleCell(gridPosition);
                        EditorUtility.SetDirty(tool);
                    }
                }
            }
        }

        // Khôi phục zTest ban đầu
        Handles.zTest = originalZTest;
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
            Undo.RegisterCompleteObjectUndo(tool, "Clear Grid");
            tool.ClearGrid();
            EditorUtility.SetDirty(tool);
        }

        if (GUILayout.Button("Clear Props"))
        {
            tool.ClearProps();
        }
    }
}
