using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ToolGeneratePropsGrid2D : MonoBehaviour
{
    [Header("Grid Settings")]
    [Range(1, 20)] public int GridWidth = 5;  // Chiều rộng của lưới
    [Range(1, 20)] public int GridHeight = 5; // Chiều cao của lưới
    [Range(0.1f, 5f)] public float CellSize = 1f; // Kích thước mỗi ô trong lưới

    [Header("Display Settings")]
    public bool ShowGrid = true; // Hiển thị lưới
    public List<PropCustom> PropCustoms = new List<PropCustom>(); // Danh sách các prop tùy chỉnh
    public List<GridCell> GridCells = new List<GridCell>(); // Danh sách các ô đã chọn

    [Serializable]
    public class PropCustom
    {
        public string Name; // Tên của loại prop
        public GameObject Prefab; // Prefab cho loại prop
        public Color ColorRect; // Màu để hiển thị trong Scene View
    }

    [Serializable]
    public class GridCell
    {
        public Vector2Int Position; // Vị trí của ô trong lưới
        public int PropCustomIndex = -1; // Chỉ số của PropCustom trong danh sách
    }

    public void GenerateProps()
    {
        // Xóa tất cả các đối tượng hiện có
        ClearProps();

        foreach (GridCell cell in GridCells)
        {
            if (cell.PropCustomIndex >= 0 && cell.PropCustomIndex < PropCustoms.Count)
            {
                var propCustom = PropCustoms[cell.PropCustomIndex];
                Vector3 worldPosition = GridToWorldPosition(cell.Position);
                SpawnProp(propCustom.Prefab, worldPosition);
            }
        }
    }

    private void SpawnProp(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject instance = Instantiate(prefab, position, Quaternion.identity, transform);
        instance.name = $"{prefab.name}_{position.x}_{position.y}";
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        float offsetX = (GridWidth * CellSize) / 2f - CellSize / 2f;
        float offsetY = (GridHeight * CellSize) / 2f - CellSize / 2f;

        return new Vector3(gridPosition.x * CellSize - offsetX, gridPosition.y * CellSize - offsetY, 0) + transform.position;
    }

    public void ClearProps()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void SaveProps(string savePath)
    {
        StartCoroutine(Cor_SaveProps(savePath));
    }

    private IEnumerator Cor_SaveProps(string savePath)
    {
    #if UNITY_EDITOR
        if (transform.childCount == 0)
        {
            Debug.LogWarning("Không có đối tượng nào để lưu.");
            yield break;
        }

        // Tạo container tạm
        string prefabName = System.IO.Path.GetFileNameWithoutExtension(savePath);
        GameObject parentContainer = new GameObject(prefabName);

        int originalChildCount = transform.childCount;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.transform.SetParent(parentContainer.transform);
            Debug.Log($"Đã chuyển {child.name} vào container tạm.");
        }

        Debug.Log($"Tổng số object trong container tạm: {parentContainer.transform.childCount}/{originalChildCount}");

        // Kiểm tra và lưu prefab
        string uniquePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(savePath);
        GameObject savedPrefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(parentContainer, uniquePath);
        if (savedPrefab != null)
        {
            Debug.Log($"Prefab đã được lưu thành công tại: {uniquePath}");
        }
        else
        {
            Debug.LogError("Lưu prefab thất bại.");
        }

        // Xóa container tạm
        DestroyImmediate(parentContainer);
        UnityEditor.AssetDatabase.Refresh();

        yield return null;
    #endif
    }

    public void ClearGrid()
    {
        GridCells.Clear();
    }

    public GridCell GetCell(Vector2Int position)
    {
        return GridCells.Find(cell => cell.Position == position);
    }

    public void ToggleCell(Vector2Int position)
    {
        GridCell existingCell = GetCell(position);
        if (existingCell != null)
        {
            existingCell.PropCustomIndex++;
            if (existingCell.PropCustomIndex >= PropCustoms.Count)
            {
                GridCells.Remove(existingCell);
            }
        }
        else
        {
            GridCells.Add(new GridCell { Position = position, PropCustomIndex = 0 });
        }
    }
}
