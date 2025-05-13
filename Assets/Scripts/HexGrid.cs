using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 六边形网格系统 - 处理六边形网格的坐标转换和邻居查找
/// </summary>
public class HexGrid : MonoBehaviour
{
    [Header("地图组件")]
    public Tilemap tilemap;  // Unity的Tilemap组件引用

    // 六边形邻居方向偏移量（平顶六边形布局）
    private static readonly Vector3Int[] neighborDirections =
    {
        // 轴向坐标系下的六个方向偏移
        new Vector3Int(1, 0, 0),    // 右 (0°)
        new Vector3Int(0, 1, 0),    // 右上 (60°)
        new Vector3Int(-1, 1, 0),   // 左上 (120°)
        new Vector3Int(-1, 0, 0),   // 左 (180°)
        new Vector3Int(0, -1, 0),   // 左下 (240°)
        new Vector3Int(1, -1, 0)    // 右下 (300°)
    };

    /// <summary>
    /// 获取指定位置的所有有效邻居格子
    /// </summary>
    /// <param name="position">中心格子坐标</param>
    /// <returns>有效邻居格子列表</returns>
    public List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // 检查所有六个方向
        foreach (var dir in neighborDirections)
        {
            Vector3Int neighborPos = position + dir;

            // 只添加存在tile的邻居（防止越界）
            if (tilemap.GetTile(neighborPos) != null)
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// 世界坐标转网格坐标
    /// </summary>
    /// <param name="worldPosition">世界空间坐标</param>
    /// <returns>网格坐标</returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// 网格坐标转世界坐标
    /// </summary>
    /// <param name="cellPosition">网格坐标</param>
    /// <returns>世界空间中心坐标</returns>
    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        // 获取网格的中心点世界坐标
        return tilemap.GetCellCenterWorld(cellPosition);
    }

    /// <summary>
    /// 检查指定网格位置是否有效（是否在地图范围内）
    /// </summary>
    public bool IsValidCell(Vector3Int cellPosition)
    {
        return tilemap.GetTile(cellPosition) != null;
    }

    /// <summary>
    /// 计算两个六边形格子之间的距离
    /// </summary>
    public int GetDistance(Vector3Int a, Vector3Int b)
    {
        // 六边形网格距离计算公式
        return (Mathf.Abs(a.x - b.x) +
               Mathf.Abs(a.y - b.y) +
               Mathf.Abs(a.z - b.z)) / 2;
    }
}
