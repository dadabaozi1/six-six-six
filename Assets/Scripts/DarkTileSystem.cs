using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 黑暗地块系统 - 管理游戏中黑暗地块的扩展和效果
/// </summary>
public class DarkTileSystem : MonoBehaviour
{
    [Header("地图设置")]
    public Tilemap baseMap;       // 基础地图Tilemap组件
    public TileBase darkTile;     // 黑暗地块的Tile引用
    public TileBase normalTile;   // 普通地块的Tile引用

    public static DarkTileSystem Instance;  // 单例实例

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null) Instance = this;
        else Destroy(gameObject);  // 防止重复实例
    }

    /// <summary>
    /// 扩展黑暗地块
    /// </summary>
    /// <param name="expandCount">最大扩展数量</param>
    public void ExpandDarkTiles(int expandCount)
    {
        List<Vector3Int> newDarkTiles = new List<Vector3Int>();  // 存储将要变黑暗的新地块

        // 遍历地图所有单元格
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            // 如果当前地块是黑暗地块
            if (baseMap.GetTile(pos) == darkTile)
            {
                // 检查六个相邻方向
                for (int i = 0; i < 6; i++)
                {
                    Vector3Int neighbor = GetHexNeighbor(pos, i);  // 获取相邻地块坐标

                    // 如果相邻地块是普通地块且未在待处理列表中，且未达到最大扩展数量
                    if (baseMap.GetTile(neighbor) == normalTile &&
                        !newDarkTiles.Contains(neighbor) &&
                        newDarkTiles.Count < expandCount)
                    {
                        newDarkTiles.Add(neighbor);  // 加入待处理列表
                    }
                }
            }
        }

        // 应用新的黑暗地块
        foreach (var tilePos in newDarkTiles)
        {
            baseMap.SetTile(tilePos, darkTile);  // 将地块设置为黑暗状态
        }

        CheckPlayerCaught();  // 检查玩家是否被黑暗吞噬
    }

    /// <summary>
    /// 获取六边形网格中指定方向的相邻地块坐标
    /// </summary>
    /// <param name="position">中心位置</param>
    /// <param name="direction">方向索引(0-5)</param>
    /// <returns>相邻地块坐标</returns>
    private Vector3Int GetHexNeighbor(Vector3Int position, int direction)
    {
        // 六边形网格的六个方向偏移量（轴向坐标系）
        Vector3Int[] hexDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // 右 (0°)
            new Vector3Int(1, -1, 0),  // 右上 (60°)
            new Vector3Int(0, -1, 0),  // 左上 (120°)
            new Vector3Int(-1, 0, 0),  // 左 (180°)
            new Vector3Int(-1, 1, 0),  // 左下 (240°)
            new Vector3Int(0, 1, 0)    // 右下 (300°)
        };

        // 返回相邻位置坐标
        return position + hexDirections[direction];
    }

    /// <summary>
    /// 检查玩家是否被黑暗地块捕获
    /// </summary>
    private void CheckPlayerCaught()
    {
        // 获取玩家所在的六边形网格坐标
        Vector3Int playerHex = baseMap.WorldToCell(PlayerController.Instance.transform.position);

        // 如果玩家所在位置是黑暗地块
        if (baseMap.GetTile(playerHex) == darkTile)
        {
            // 触发游戏结束（失败）
            GameManager.Instance.GameOver(false);
        }
    }
}