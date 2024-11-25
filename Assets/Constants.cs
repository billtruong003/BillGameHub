using UnityEngine;
namespace BillUtils
{
    public static class Constants
    {
        // Màu sắc sử dụng trong lưới
        public static readonly Color DefaultGizmoColor = Color.green;
        public static readonly Color CustomCellColor = Color.yellow;
        public static readonly Color RewardColor = Color.yellow;
        public static readonly Color ObstacleColor = Color.red;

        // Phạm vi giới hạn cho kích thước hoặc số lượng
        public const int PropCountMin = 0;
        public const int PropCountMax = 10;
        public const float SizeMin = -20f;
        public const float SizeMax = 20f;

        // Định nghĩa các giá trị enum
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
            Custom
        }

        public enum PropGen
        {
            Reward,
            Obstacle,
            None
        }
    }
}