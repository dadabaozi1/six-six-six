// 导入必要的Unity命名空间
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardManager : MonoBehaviour
{
    // ========== 单例模式 ==========
    [Header("游戏设置")]
    public List<CardData> cardDeck;        // 初始卡牌库（在编辑器中配置）
    public int initialHandSize = 5;        // 初始手牌数量
    public int cardsPerTurn = 2;           // 每回合抽卡数量
    public int maxHandSize = 7;            // 手牌上限

    private List<CardData> currentDeck;    // 当前牌库（运行时使用）
    private List<CardData> currentHand;    // 当前手牌
    private List<CardData> discardPile = new List<CardData>(); // 弃牌堆

    [Header("UI引用")]
    public CardUIManager cardUIManager;    // 卡牌UI管理器引用

    public static CardManager Instance;    // 单例实例

    // ========== 生命周期方法 ==========
    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null) Instance = this;
        else Destroy(gameObject);  // 防止重复实例
    }

    private void Start()
    {
        InitializeDeck();     // 初始化牌库
        DrawInitialHand();    // 抽取初始手牌
    }

    // ========== 牌库管理 ==========
    /// <summary>
    /// 初始化牌库（复制预设牌库并洗牌）
    /// </summary>
    private void InitializeDeck()
    {
        currentDeck = new List<CardData>(cardDeck);  // 深拷贝初始牌库
        ShuffleDeck();  // 洗牌
        Debug.Log($"初始牌库: {currentDeck.Count}张卡");
    }

    /// <summary>
    /// 洗牌算法（Fisher-Yates洗牌法）
    /// </summary>
    private void ShuffleDeck()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            CardData temp = currentDeck[i];
            int randomIndex = Random.Range(i, currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;
        }
    }

    // ========== 抽卡逻辑 ==========
    /// <summary>
    /// 抽取初始手牌
    /// </summary>
    private void DrawInitialHand()
    {
        currentHand = new List<CardData>();
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCardWithReshuffle();  // 自动处理牌库不足的情况
        }
    }

    /// <summary>
    /// 每回合开始时的抽卡逻辑
    /// </summary>
    public void DrawCardsForTurn()
    {
        for (int i = 0; i < cardsPerTurn; i++)
        {
            if (currentHand.Count < maxHandSize)
            {
                DrawCardWithReshuffle();
            }
            else
            {
                Debug.Log("手牌已达上限，不再抽卡");
                break;
            }
        }
    }

    /// <summary>
    /// 智能抽卡（自动检查牌库并洗入弃牌堆）
    /// </summary>
    private void DrawCardWithReshuffle()
    {
        if (currentDeck.Count == 0)
        {
            ReshuffleDiscardPile();  // 牌库空时洗入弃牌堆
        }

        if (currentDeck.Count > 0)
        {
            DrawCard();  // 正常抽卡
        }
        else
        {
            Debug.LogWarning("牌库和弃牌堆均已空，无法抽卡");
        }
    }

    /// <summary>
    /// 将弃牌堆洗回牌库
    /// </summary>
    private void ReshuffleDiscardPile()
    {
        if (discardPile.Count > 0)
        {
            Debug.Log($"将{discardPile.Count}张弃牌洗回牌库");
            currentDeck.AddRange(discardPile);  // 合并弃牌堆
            discardPile.Clear();              // 清空弃牌堆
            ShuffleDeck();                     // 重新洗牌
        }
    }

    /// <summary>
    /// 基础抽卡方法（从牌库顶抽一张牌）
    /// </summary>
    private void DrawCard()
    {
        CardData drawnCard = currentDeck[0];  // 取第一张牌
        currentDeck.RemoveAt(0);              // 从牌库移除
        currentHand.Add(drawnCard);           // 加入手牌
        cardUIManager.UpdateHandUI(currentHand);  // 更新UI

        Debug.Log($"抽卡: {drawnCard.type} (剩余:{currentDeck.Count})");
    }

    // ========== 卡牌使用 ==========
    /// <summary>
    /// 尝试使用卡牌（主入口）
    /// </summary>
    /// <param name="card">要使用的卡牌</param>
    /// <returns>是否使用成功</returns>
    public bool PlayCard(CardData card)
    {
        if (!TurnManager.Instance.CanPlayCard())
        {
            Debug.Log("当前不能使用卡牌：不在玩家回合或已达出牌上限");
            return false;
        }

        Debug.Log($"尝试使用卡牌：{card.type}");

        // 根据卡牌类型调用不同效果
        switch (card.type)
        {
            case CardData.CardType.Move:
                PlayMoveCard(Vector3Int.right); // 示例方向（实际应传入玩家选择的方向）
                break;
                // 其他卡牌类型处理...
        }

        // 将使用后的卡牌移入弃牌堆
        currentHand.Remove(card);
        discardPile.Add(card);
        cardUIManager.UpdateHandUI(currentHand);

        TurnManager.Instance.RegisterCardPlayed();  // 通知回合管理器
        return true;
    }

    // ========== 卡牌效果实现 ==========
    public void PlayMoveCard(Vector3Int direction)
    {
        /* 移动卡效果实现：沿指定方向移动玩家 */
    }

    public void PlayShockCard(Vector3Int direction)
    {
        /* 电击卡效果实现：攻击指定方向敌人 */
    }

    public void PlayEnergyStoneCard(Vector3Int pillarPosition)
    {
        /* 能量石卡效果：激活指定位置的石柱 */
    }

    public void PlayTeleportCard()
    {
        /* 传送卡效果：随机传送玩家 */
    }
}