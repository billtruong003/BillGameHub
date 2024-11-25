using UnityEngine;
using System;
using System.Collections.Generic;

public class ToolGeneratePropsGrid3D : MonoBehaviour
{
    [Header("Grid Settings")]
    public int GridWidth = 10;
    public int GridHeight = 10;
    public int GridDepth = 10;
    public float CellSize = 1f;
    
    [Header("Display Settings")]
    public bool ShowGrid = true;
    public Color GridColor = Color.gray; // Màu sắc của grid
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
        public Vector3Int Position; // Vị trí của ô trong lưới
        public int PropCustomIndex = -1; // Chỉ số của PropCustom trong danh sách
    }

    public Vector3 GetGridOrigin()
    {
        return transform.position - new Vector3(
            (GridWidth * CellSize) / 2f - CellSize / 2f,
            (GridHeight * CellSize) / 2f - CellSize / 2f,
            (GridDepth * CellSize) / 2f - CellSize / 2f
        );
    }

    public Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return GetGridOrigin() + new Vector3(
            gridPosition.x * CellSize,
            gridPosition.y * CellSize,
            gridPosition.z * CellSize
        );
    }

    public Vector3Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - GetGridOrigin();
        return new Vector3Int(
            Mathf.FloorToInt((localPosition.x) / CellSize),
            Mathf.FloorToInt((localPosition.y) / CellSize),
            Mathf.FloorToInt((localPosition.z) / CellSize)
        );
    }

    public void ToggleCell(Vector3Int position)
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

    public GridCell GetCell(Vector3Int position)
    {
        return GridCells.Find(cell => cell.Position == position);
    }

    public void GenerateProps()
    {
        // Xóa các prop hiện có
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
        instance.name = $"{prefab.name}_{position.x}_{position.y}_{position.z}";
    }

    public void ClearProps()
    {
#if UNITY_EDITOR
        // Xóa tất cả các GameObject con
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
#endif
    }

    public void ClearGrid()
    {
        GridCells.Clear();
    }

    // Kiểm tra xem vị trí grid có nằm trong phạm vi không
    public bool IsWithinGridBounds(Vector3Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < GridWidth &&
               gridPosition.y >= 0 && gridPosition.y < GridHeight &&
               gridPosition.z >= 0 && gridPosition.z < GridDepth;
    }

    // Lấy Bounds của grid
    public Bounds GetGridBounds()
    {
        Vector3 origin = GetGridOrigin();
        Vector3 size = new Vector3(GridWidth * CellSize, GridHeight * CellSize, GridDepth * CellSize);
        return new Bounds(origin + size / 2f, size);
    }
}
