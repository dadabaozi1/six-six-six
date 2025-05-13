using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 回合管理器，负责控制玩家与敌人的回合流程
public class TurnManager : MonoBehaviour
{
    // 单例模式，方便其他脚本调用 TurnManager.Instance
    public static TurnManager Instance { get; private set; }

    // 游戏状态枚举：玩家回合、敌人回合、游戏结束
    public enum GameState { PlayerTurn, EnemyTurn, GameOver }

    [Header("Settings")]
    public int darkTilesExpandMin = 1; // 每回合黑暗地块扩展最小数量
    public int darkTilesExpandMax = 2; // 每回合黑暗地块扩展最大数量

    private GameState currentState;    // 当前游戏状态
    private int cardsPlayedThisTurn;   // 本回合已打出的卡牌数
    private const int maxCardsPerTurn = 3; // 每回合最多可打出的卡牌数

    public int CurrentTurn { get; private set; } // 当前回合数
    public int CardsPlayedThisTurn => cardsPlayedThisTurn; // 外部只读访问已打出的卡牌数

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 场景中已有实例则销毁重复的
        }
    }

    private void Start()
    {
        // 游戏开始时，初始化玩家回合
        StartPlayerTurn();
    }

    // 开始玩家回合
    public void StartPlayerTurn()
    {
        CurrentTurn++; // 回合数+1
        currentState = GameState.PlayerTurn; // 设置为玩家回合
        cardsPlayedThisTurn = 0; // 重置本回合已打出的卡牌数

        // 抽卡逻辑，交由卡牌管理器实现
        CardManager.Instance.DrawCardsForTurn();

        // 更新UI回合信息
        GameUIController.Instance.UpdateTurnInfo();

        Debug.Log("Player Turn Started"); // 调试信息
    }

    // 结束玩家回合
    public void EndPlayerTurn()
    {
        if (currentState != GameState.PlayerTurn) return; // 非玩家回合时不执行

        // 黑暗地块扩展逻辑
        DarkTileSystem.Instance.ExpandDarkTiles(
            Random.Range(darkTilesExpandMin, darkTilesExpandMax + 1)
        );

        // 结束后立即进入下一玩家回合
        StartPlayerTurn();
    }

    // （未被使用的私有方法）扩展黑暗地块
    private void ExpandDarkTiles()
    {
        int expandCount = Random.Range(darkTilesExpandMin, darkTilesExpandMax + 1);
        DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
    }

    // 判断是否还能继续打卡牌
    public bool CanPlayCard()
    {
        return currentState == GameState.PlayerTurn &&
               cardsPlayedThisTurn < maxCardsPerTurn;
    }

    // 注册已打出的卡牌数（每打出一张卡调用一次）
    public void RegisterCardPlayed()
    {
        cardsPlayedThisTurn++; // 已打出卡牌数+1

        // 更新UI回合信息
        GameUIController.Instance.UpdateTurnInfo();
    }
}