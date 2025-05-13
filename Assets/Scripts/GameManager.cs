using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 游戏管理器 - 控制游戏核心流程和状态
/// </summary>
public class GameManager : MonoBehaviour
{
    // ========== 单例实例 ==========
    public static GameManager Instance;

    // ========== 游戏设置 ==========
    [Header("游戏设置")]
    public int initialCardCount = 5;  // 初始手牌数量
    public int cardsPerTurn = 2;      // 每回合抽牌数量
    public int maxHandSize = 7;       // 手牌上限

    // ========== 引用配置 ==========
    [Header("游戏对象引用")]
    public Transform playerPrefab;    // 玩家预制体
    public Transform exitPoint;       // 出口点位置

    [Header("系统引用")]
    public TurnManager turnManager;   // 回合管理器
    public CardManager cardManager;   // 卡牌管理器

    // ========== 生命周期方法 ==========
    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 可选：跨场景保持
        }
        else
        {
            Destroy(gameObject);  // 防止重复实例
        }
    }

    private void Start()
    {
        InitializeGame();  // 游戏初始化
    }

    // ========== 游戏流程控制 ==========
    /// <summary>
    /// 初始化游戏状态
    /// </summary>
    private void InitializeGame()
    {
        SpawnPlayer();               // 生成玩家
        turnManager.StartPlayerTurn(); // 开始玩家回合
        // 可以在这里初始化卡牌系统等其他内容
    }

    /// <summary>
    /// 生成玩家角色
    /// </summary>
    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, GetStartPosition(), Quaternion.identity);
        Debug.Log("玩家已生成");
    }

    /// <summary>
    /// 获取玩家起始位置
    /// </summary>
    /// <returns>起始坐标</returns>
    private Vector3 GetStartPosition()
    {
        // 简单实现 - 实际应该根据地图设计确定
        // 可以考虑从地图数据或配置文件中读取
        return new Vector3(0, 0, 0);
    }

    // ========== 游戏状态控制 ==========
    /// <summary>
    /// 游戏结束处理
    /// </summary>
    /// <param name="win">是否胜利</param>
    public void GameOver(bool win)
    {
        Debug.Log(win ? "游戏胜利！" : "游戏结束");

        // 这里可以添加更多游戏结束逻辑：
        // 1. 显示结算UI
        // 2. 保存游戏数据
        // 3. 播放胜利/失败动画
        // 4. 解锁成就等

        // 示例：暂停游戏
        Time.timeScale = 0;

        // 注意：实际项目中应该用更完善的状态管理系统
    }
}