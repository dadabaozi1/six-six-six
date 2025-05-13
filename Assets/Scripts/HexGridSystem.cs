using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 六边形网格系统 - 管理游戏中的六边形网格逻辑和地块类型判断
/// </summary>
public class HexGridSystem : MonoBehaviour
{
    [Header("地图层级设置")]
    public Tilemap baseMap;         // 基础地图层（地形、功能地块）
    public Tilemap obstacleMap;     // 障碍物层（可选）

    [Header("地块类型配置")]
    public TileBase normalTile;      // 普通可行走地块
    public TileBase darkTile;        // 黑暗地块（危险区域）
    public TileBase cardReplenishTile; // 卡牌补给地块
    public TileBase blockPillarTile;  // 可阻挡的石柱地块
    public TileBase exitTile;        // 出口地块

    // 单例实例
    public static HexGridSystem Instance;

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 如需跨场景可取消注释
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 获取世界坐标对应的六边形网格坐标
    /// </summary>
    /// <param name="worldPosition">世界空间坐标</param>
    /// <returns>六边形网格坐标</returns>
    public Vector3Int GetHexAtPosition(Vector3 worldPosition)
    {
        return baseMap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// 获取六边形网格的中心世界坐标
    /// </summary>
    /// <param name="hexCoords">网格坐标</param>
    /// <returns>世界空间中心坐标</returns>
    public Vector3 GetHexCenterPosition(Vector3Int hexCoords)
    {
        return baseMap.GetCellCenterWorld(hexCoords);
    }

    /// <summary>
    /// 判断六边形是否可通行
    /// </summary>
    /// <param name="hexCoords">网格坐标</param>
    /// <returns>是否可通行</returns>
    public bool IsHexWalkable(Vector3Int hexCoords)
    {
        // 检查基础地图地块类型
        TileBase baseTile = baseMap.GetTile(hexCoords);
        bool isWalkableTerrain = baseTile == normalTile ||
                               baseTile == cardReplenishTile ||
                               baseTile == blockPillarTile;

        // 检查障碍物层（如果有配置）
        bool hasObstacle = obstacleMap != null && obstacleMap.GetTile(hexCoords) != null;

        return isWalkableTerrain && !hasObstacle;
    }

    /// <summary>
    /// 获取六边形地块类型
    /// </summary>
    public TileBase GetHexType(Vector3Int hexCoords)
    {
        return baseMap.GetTile(hexCoords);
    }

    /// <summary>
    /// 判断是否是特殊功能地块
    /// </summary>
    public bool IsSpecialHex(Vector3Int hexCoords)
    {
        TileBase tile = baseMap.GetTile(hexCoords);
        return tile == cardReplenishTile || tile == blockPillarTile || tile == exitTile;
    }

    /// <summary>
    /// 获取指定位置的所有有效邻居格子
    /// </summary>
    public List<Vector3Int> GetNeighbors(Vector3Int centerHex)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),    // 右
            new Vector3Int(0, 1, 0),     // 右上
            new Vector3Int(-1, 1, 0),    // 左上
            new Vector3Int(-1, 0, 0),    // 左
            new Vector3Int(0, -1, 0),    // 左下
            new Vector3Int(1, -1, 0)     // 右下
        };

        foreach (var dir in directions)
        {
            Vector3Int neighbor = centerHex + dir;
            if (baseMap.GetTile(neighbor) != null) // 检查是否存在该地块
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}