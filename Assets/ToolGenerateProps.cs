using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BillUtils;
[ExecuteInEditMode]
public class ToolGenerateProps : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameObject propPrefab; // Prefab được dùng để sinh prop
    [SerializeField] private Constants.PropShape shapeType; // Hình dạng cần sắp xếp
    [SerializeField, Range(0,10)] private int propCount = 10; // Số lượng prop
    [SerializeField, Range(-20,20)] private float size = 1f; // Kích thước hoặc khoảng cách giữa các prop
    [SerializeField] private Color gizmoColor = Color.green; // Màu của Gizmo trong Scene View

    [Header("Custom Grid Settings")]
    [SerializeField] private int gridWidth = 5; // Chiều rộng của lưới
    [SerializeField] private int gridHeight = 5; // Chiều cao của lưới
    [SerializeField] private List<Vector2Int> selectedCells = new List<Vector2Int>(); // Các ô được chọn trong lưới
    [SerializeField] private Color customCellColor = Color.yellow; // Màu của các ô trong custom grid

    [SerializeField] private List<GameObject> spawnedProps = new List<GameObject>();
    public string propName;

    // Getters for Editor
    public Constants.PropShape ShapeType => shapeType;
    public float Size => size;
    public List<Vector2Int> SelectedCells => selectedCells;
    // Sử dụng enum từ Constants
    
    [SerializeField] public List<Prop> props = new List<Prop>();

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    [Serializable]
    public class Prop
    {
        public Constants.PropGen PropGen = Constants.PropGen.None; // Loại prop
        public Vector3Int Position; // Vị trí trong grid (x, y, z nếu cần 3D)
    }
    
    [Serializable]
    public enum PropGen 
    {
        Reward,
        Obstacle,
        None,
    }

    public void GenerateProps()
    {
        ClearProps();

        switch (shapeType)
        {
            case Constants.PropShape.Circle:
                GenerateCircle();
                break;
            case Constants.PropShape.Square:
                GenerateSquare();
                break;
            case Constants.PropShape.Triangle:
                GenerateTriangle();
                break;
            case Constants.PropShape.Hexagon:
                GenerateHexagon();
                break;
            case Constants.PropShape.Star:
                GenerateStar();
                break;
            case Constants.PropShape.Line:
                GenerateLine();
                break;
            case Constants.PropShape.Cross:
                GenerateCross();
                break;
            case Constants.PropShape.Spiral:
                GenerateSpiral();
                break;
            case Constants.PropShape.Grid:
                GenerateGrid();
                break;
            case Constants.PropShape.Heart:
                GenerateHeart();
                break;
            case Constants.PropShape.Custom:
                GenerateCustomGrid();
                break;
        }
    }

    [Header("Circle Settings")]
    [SerializeField] private int resolution = 10; 
    float radius;  // Kích thước radius của hình tròn
    private void GenerateCircle()
    {
        radius = size; // Bán kính của hình tròn

        for (int x = -resolution; x <= resolution; x++)
        {
            for (int y = -resolution; y <= resolution; y++)
            {
                Vector3 position = new Vector3(x, y, 0) * (radius / resolution);

                // Chỉ đặt prop nếu điểm nằm trong hình tròn
                if (position.magnitude <= radius)
                {
                    SpawnProp(transform.position + position);
                }
            }
        }
    }


    private void GenerateSquare()
    {
        float halfSize = size / 2f; // Kích thước nửa cạnh của hình vuông
        float step = size / propCount; // Khoảng cách giữa các prop

        for (float x = -halfSize; x <= halfSize; x += step)
        {
            for (float y = -halfSize; y <= halfSize; y += step)
            {
                Vector3 position = new Vector3(x, y, 0);
                SpawnProp(transform.position + position);
            }
        }
    }

    private void GenerateTriangle()
    {
        float height = size; // Chiều cao của tam giác
        float baseWidth = size; // Chiều rộng đáy của tam giác

        for (int row = 0; row < propCount; row++)
        {
            float rowHeight = row * (height / propCount);
            int propsInRow = propCount - row;

            for (int i = 0; i < propsInRow; i++)
            {
                float xOffset = (baseWidth / propsInRow) * i - (baseWidth / 2f);
                Vector3 position = new Vector3(xOffset, rowHeight, 0);
                SpawnProp(transform.position + position);
            }
        }
    }


    private void GenerateHexagon()
    {
        float radius = size; // Bán kính của hình lục giác
        int layers = propCount / 6; // Số lớp của lục giác

        for (int layer = 0; layer <= layers; layer++)
        {
            float layerRadius = (radius / layers) * layer;
            int pointsInLayer = 6 * layer;

            for (int i = 0; i < pointsInLayer; i++)
            {
                float angle = i * Mathf.PI * 2 / pointsInLayer;
                Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * layerRadius;
                SpawnProp(transform.position + position);
            }
        }
    }

    private void GenerateStar()
    {
        float outerRadius = size; // Bán kính ngoài
        float innerRadius = size / 2f; // Bán kính trong
        int resolution = propCount;

        for (int i = 0; i < resolution; i++)
        {
            float angleOuter = i * Mathf.PI * 2 / resolution;
            float angleInner = angleOuter + (Mathf.PI / resolution);

            Vector3 outerPoint = new Vector3(Mathf.Cos(angleOuter), Mathf.Sin(angleOuter), 0) * outerRadius;
            Vector3 innerPoint = new Vector3(Mathf.Cos(angleInner), Mathf.Sin(angleInner), 0) * innerRadius;

            SpawnProp(transform.position + outerPoint);
            SpawnProp(transform.position + innerPoint);
        }

        // Lấp đầy khoảng bên trong
        float step = size / (resolution / 2);
        for (float x = -outerRadius; x <= outerRadius; x += step)
        {
            for (float y = -outerRadius; y <= outerRadius; y += step)
            {
                Vector3 position = new Vector3(x, y, 0);
                if (position.magnitude <= outerRadius)
                {
                    SpawnProp(transform.position + position);
                }
            }
        }
    }

    public void ClearAllProps()
    {
        ClearProps();
    }

    private void GenerateLine()
    {
        for (int i = 0; i < propCount; i++)
        {
            Vector3 position = new Vector3(i * size, 0, 0);
            SpawnProp(position);
        }
    }

    private void GenerateCross()
    {
        for (int i = 0; i < propCount / 2; i++)
        {
            Vector3 position = new Vector3(i * size, 0, 0);
            SpawnProp(position);
            position = new Vector3(0, i * size, 0);
            SpawnProp(position);
        }
    }

    private void GenerateSpiral()
    {
        for (int i = 0; i < propCount; i++)
        {
            float angle = i * Mathf.PI * 2 / propCount;
            float radius = size * i / propCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            SpawnProp(position);
        }
    }

    private void GenerateGrid()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(propCount));
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (spawnedProps.Count >= propCount) return;
                Vector3 position = new Vector3(x, y, 0) * size;
                SpawnProp(position);
            }
        }
    }

    private void GenerateHeart()
    {
        for (int i = 0; i < propCount; i++)
        {
            float t = i / (float)propCount * Mathf.PI * 2;
            float x = 16 * Mathf.Sin(t) * Mathf.Sin(t) * Mathf.Sin(t);
            float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);
            Vector3 position = new Vector3(x, y, 0) * size * 0.1f;
            SpawnProp(position);
        }
    }

    private void GenerateCustomGrid()
    {
        // Tính toán offset để căn giữa
        float offsetX = (gridWidth * size) / 2f - size / 2f;
        float offsetY = (gridHeight * size) / 2f - size / 2f;

        foreach (Vector2Int cell in selectedCells)
        {
            // Điều chỉnh vị trí theo offset
            Vector3 position = new Vector3(cell.x * size - offsetX, cell.y * size - offsetY, 0) + transform.position;
            SpawnProp(position);
        }
    }


    private void SpawnProp(Vector3 position)
    {
        if (propPrefab == null) return;
        GameObject prop = Instantiate(propPrefab, position, Quaternion.identity, transform);
        spawnedProps.Add(prop);
    }

    private void ClearProps()
    {
        foreach (GameObject prop in spawnedProps)
        {
            if (prop != null) DestroyImmediate(prop);
        }
        spawnedProps.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isEditor) return;

        Gizmos.color = gizmoColor;

        switch (shapeType)
        {
            case Constants.PropShape.Circle:
                DrawCirclePreview();
                break;
            case Constants.PropShape.Square:
                DrawSquarePreview();
                break;
            case Constants.PropShape.Triangle:
                DrawTrianglePreview();
                break;
            case Constants.PropShape.Hexagon:
                DrawHexagonPreview();
                break;
            case Constants.PropShape.Star:
                DrawStarPreview();
                break;
            case Constants.PropShape.Line:
                DrawLinePreview();
                break;
            case Constants.PropShape.Cross:
                DrawCrossPreview();
                break;
            case Constants.PropShape.Spiral:
                DrawSpiralPreview();
                break;
            case Constants.PropShape.Grid:
                // DrawGridPreview();
                break;
            case Constants.PropShape.Heart:
                DrawHeartPreview();
                break;
            case Constants.PropShape.Custom:
                DrawCustomGridPreview();
                break;
        }
    }

    private void DrawCustomGridPreview()
    {
        Gizmos.color = gizmoColor;

        // Tính toán offset để căn giữa quanh transform.position
        float offsetX = (gridWidth * size) / 2f;
        float offsetY = (gridHeight * size) / 2f;

        // Vẽ grid
        for (int x = 0; x <= gridWidth; x++)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(x * size - offsetX, -offsetY, 0),
                transform.position + new Vector3(x * size - offsetX, gridHeight * size - offsetY, 0)
            );
        }
        for (int y = 0; y <= gridHeight; y++)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(-offsetX, y * size - offsetY, 0),
                transform.position + new Vector3(gridWidth * size - offsetX, y * size - offsetY, 0)
            );
        }

        // Vẽ các ô đã chọn
        Gizmos.color = customCellColor;
        foreach (Vector2Int cell in selectedCells)
        {
            Vector3 center = transform.position + new Vector3(cell.x * size - offsetX + size / 2, cell.y * size - offsetY + size / 2, 0);
            Gizmos.DrawCube(center, new Vector3(size * 0.8f, size * 0.8f, 0.1f));
        }
    }



    private void DrawCirclePreview()
    {
        radius = size; // Bán kính của hình tròn

        for (int x = -resolution; x <= resolution; x++)
        {
            for (int y = -resolution; y <= resolution; y++)
            {
                Vector3 position = new Vector3(x, y, 0) * (radius / resolution);

                // Chỉ hiển thị Gizmos nếu điểm nằm trong hình tròn
                if (position.magnitude <= radius)
                {
                    Gizmos.DrawSphere(transform.position + position, 0.1f);
                }
            }
        }
    }

    private void DrawSquarePreview()
    {
        float halfSize = size / 2f; // Kích thước nửa cạnh của hình vuông
        float step = size / propCount; // Khoảng cách giữa các điểm

        for (float x = -halfSize; x <= halfSize; x += step)
        {
            for (float y = -halfSize; y <= halfSize; y += step)
            {
                Vector3 position = new Vector3(x, y, 0);
                Gizmos.DrawCube(transform.position + position, Vector3.one * 0.2f);
            }
        }
    }


    private void DrawTrianglePreview()
    {
        float height = size; // Chiều cao của tam giác
        float baseWidth = size; // Chiều rộng đáy của tam giác

        for (int row = 0; row < propCount; row++)
        {
            float rowHeight = row * (height / propCount);
            int propsInRow = propCount - row;

            for (int i = 0; i < propsInRow; i++)
            {
                float xOffset = (baseWidth / propsInRow) * i - (baseWidth / 2f);
                Vector3 position = new Vector3(xOffset, rowHeight, 0);
                Gizmos.DrawSphere(transform.position + position, 0.1f);
            }
        }
    }

    private void DrawHexagonPreview()
    {
        float radius = size; // Bán kính của hình lục giác
        int layers = propCount / 6; // Số lớp của lục giác

        for (int layer = 0; layer <= layers; layer++)
        {
            float layerRadius = (radius / layers) * layer;
            int pointsInLayer = 6 * layer;

            for (int i = 0; i < pointsInLayer; i++)
            {
                float angle = i * Mathf.PI * 2 / pointsInLayer;
                Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * layerRadius;
                Gizmos.DrawSphere(transform.position + position, 0.1f);
            }
        }
    }

    private void DrawStarPreview()
    {
        float outerRadius = size; // Bán kính ngoài
        float innerRadius = size / 2f; // Bán kính trong
        int resolution = propCount;

        for (int i = 0; i < resolution; i++)
        {
            float angleOuter = i * Mathf.PI * 2 / resolution;
            float angleInner = angleOuter + (Mathf.PI / resolution);

            Vector3 outerPoint = new Vector3(Mathf.Cos(angleOuter), Mathf.Sin(angleOuter), 0) * outerRadius;
            Vector3 innerPoint = new Vector3(Mathf.Cos(angleInner), Mathf.Sin(angleInner), 0) * innerRadius;

            Gizmos.DrawSphere(transform.position + outerPoint, 0.1f);
            Gizmos.DrawSphere(transform.position + innerPoint, 0.1f);
        }

        // Lấp đầy khoảng bên trong
        float step = size / (resolution / 2);
        for (float x = -outerRadius; x <= outerRadius; x += step)
        {
            for (float y = -outerRadius; y <= outerRadius; y += step)
            {
                Vector3 position = new Vector3(x, y, 0);
                if (position.magnitude <= outerRadius)
                {
                    Gizmos.DrawSphere(transform.position + position, 0.1f);
                }
            }
        }
    }

    private void DrawLinePreview()
    {
        for (int i = 0; i < propCount; i++)
        {
            Vector3 position = new Vector3(i * size, 0, 0);
            Gizmos.DrawWireCube(transform.position + position, Vector3.one * 0.2f);
        }
    }

    private void DrawCrossPreview()
    {
        for (int i = 0; i < propCount / 2; i++)
        {
            Vector3 position = new Vector3(i * size, 0, 0);
            Gizmos.DrawWireCube(transform.position + position, Vector3.one * 0.2f);
            position = new Vector3(0, i * size, 0);
            Gizmos.DrawWireCube(transform.position + position, Vector3.one * 0.2f);
        }
    }

    private void DrawSpiralPreview()
    {
        for (int i = 0; i < propCount; i++)
        {
            float angle = i * Mathf.PI * 2 / propCount;
            float radius = size * i / propCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            Gizmos.DrawWireSphere(transform.position + position, 0.1f);
        }
    }

    // private void DrawGridPreview()
    // {
    //     int gridSize = Mathf.CeilToInt(Mathf.Sqrt(propCount));
    //     for (int x = 0; x < gridSize; x++)
    //     {
    //         for (int y = 0; y < gridSize; y++)
    //         {
    //             if (x * gridSize + y >= propCount) return;
    //             Vector3 position = new Vector3(x, y, 0) * size;
    //             Gizmos.DrawWireCube(transform.position + position, Vector3.one * 0.2f);
    //         }
    //     }
    // }

    private void DrawHeartPreview()
    {
        for (int i = 0; i < propCount; i++)
        {
            float t = i / (float)propCount * Mathf.PI * 2;
            float x = 16 * Mathf.Sin(t) * Mathf.Sin(t) * Mathf.Sin(t);
            float y = 13 * Mathf.Cos(t) - 5 * Mathf.Cos(2 * t) - 2 * Mathf.Cos(3 * t) - Mathf.Cos(4 * t);
            Vector3 position = new Vector3(x, y, 0) * size * 0.05f;
            Gizmos.DrawWireSphere(transform.position + position, 0.1f);
        }
    }

    public void SavePropsAsPrefab(string savePath)
    {
        if (spawnedProps.Count == 0)
        {
            Debug.LogWarning("Không có prop nào được sinh ra để lưu!");
            return;
        }

#if UNITY_EDITOR
        // Tạo một GameObject container để lưu tất cả các props
        GameObject container = new GameObject("PropsContainer");

        // Di chuyển tất cả các props thành con của container
        foreach (GameObject prop in spawnedProps)
        {
            if (prop != null)
                prop.transform.SetParent(container.transform);
        }

        // Lưu container thành prefab
        string prefabPath = AssetDatabase.GenerateUniqueAssetPath(savePath);
        PrefabUtility.SaveAsPrefabAsset(container, prefabPath);

        // Xóa container sau khi lưu
        DestroyImmediate(container);

        Debug.Log($"Đã lưu prefab tại: {prefabPath}");
#else
        Debug.LogError("Hàm này chỉ hoạt động trong chế độ Editor!");
#endif
    }
}
