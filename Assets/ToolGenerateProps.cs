using System.Collections.Generic;
using UnityEngine;

public enum PropShape
{
    Circle,
    Square,
    Triangle,
    Hexagon,
    Star,
    Line,
    Cross,
    Spiral,
    Grid,
    Heart,
    Custom // Thêm tùy chọn Custom
}

[ExecuteInEditMode]
public class ToolGenerateProps : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private GameObject propPrefab; // Prefab được dùng để sinh prop
    [SerializeField] private PropShape shapeType; // Hình dạng cần sắp xếp
    [SerializeField] private int propCount = 10; // Số lượng prop
    [SerializeField] private float size = 1f; // Kích thước hoặc khoảng cách giữa các prop
    [SerializeField] private Color gizmoColor = Color.green; // Màu của Gizmo trong Scene View

    [Header("Custom Grid Settings")]
    [SerializeField] private int gridWidth = 5; // Chiều rộng của lưới
    [SerializeField] private int gridHeight = 5; // Chiều cao của lưới
    [SerializeField] private List<Vector2Int> selectedCells = new List<Vector2Int>(); // Các ô được chọn trong lưới
    [SerializeField] private Color customCellColor = Color.yellow; // Màu của các ô trong custom grid

    private List<GameObject> spawnedProps = new List<GameObject>();

    // Getters for Editor
    public PropShape ShapeType => shapeType;
    public float Size => size;
    public List<Vector2Int> SelectedCells => selectedCells;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    public void GenerateProps()
    {
        // Xóa các prop đã spawn trước đó
        ClearProps();

        // Sinh prop dựa trên hình dạng
        switch (shapeType)
        {
            case PropShape.Circle:
                GenerateCircle();
                break;
            case PropShape.Square:
                GenerateSquare();
                break;
            case PropShape.Triangle:
                GenerateTriangle();
                break;
            case PropShape.Hexagon:
                GenerateHexagon();
                break;
            case PropShape.Star:
                GenerateStar();
                break;
            case PropShape.Line:
                GenerateLine();
                break;
            case PropShape.Cross:
                GenerateCross();
                break;
            case PropShape.Spiral:
                GenerateSpiral();
                break;
            case PropShape.Grid:
                GenerateGrid();
                break;
            case PropShape.Heart:
                GenerateHeart();
                break;
            case PropShape.Custom:
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
        foreach (Vector2Int cell in selectedCells)
        {
            Vector3 position = new Vector3(cell.x * size, cell.y * size, 0) + transform.position;
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
            case PropShape.Circle:
                DrawCirclePreview();
                break;
            case PropShape.Square:
                DrawSquarePreview();
                break;
            case PropShape.Triangle:
                DrawTrianglePreview();
                break;
            case PropShape.Hexagon:
                DrawHexagonPreview();
                break;
            case PropShape.Star:
                DrawStarPreview();
                break;
            case PropShape.Line:
                DrawLinePreview();
                break;
            case PropShape.Cross:
                DrawCrossPreview();
                break;
            case PropShape.Spiral:
                DrawSpiralPreview();
                break;
            case PropShape.Grid:
                DrawGridPreview();
                break;
            case PropShape.Heart:
                DrawHeartPreview();
                break;
            case PropShape.Custom:
                DrawCustomGridPreview();
                break;
        }
    }

    private void DrawCustomGridPreview()
    {
        Gizmos.color = gizmoColor;

        // Vẽ grid
        for (int x = 0; x <= gridWidth; x++)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(x * size, 0, 0),
                transform.position + new Vector3(x * size, gridHeight * size, 0)
            );
        }
        for (int y = 0; y <= gridHeight; y++)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(0, y * size, 0),
                transform.position + new Vector3(gridWidth * size, y * size, 0)
            );
        }

        // Vẽ các ô đã chọn
        Gizmos.color = customCellColor;
        foreach (Vector2Int cell in selectedCells)
        {
            Vector3 center = transform.position + new Vector3(cell.x * size + size / 2, cell.y * size + size / 2, 0);
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

    private void DrawGridPreview()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(propCount));
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (x * gridSize + y >= propCount) return;
                Vector3 position = new Vector3(x, y, 0) * size;
                Gizmos.DrawWireCube(transform.position + position, Vector3.one * 0.2f);
            }
        }
    }

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

}
