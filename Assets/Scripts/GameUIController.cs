using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏UI控制器 - 管理游戏界面交互和显示
/// </summary>
public class GameUIController : MonoBehaviour
{
    // ========== UI组件引用 ==========
    [Header("UI组件")]
    public Text turnInfoText;        // 回合信息显示文本
    public Button endTurnButton;     // 结束回合按钮

    // ========== 单例实例 ==========
    public static GameUIController Instance;

    // ========== 生命周期方法 ==========
    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 如果需要跨场景可以取消注释
        }
        else
        {
            Destroy(gameObject);    // 防止重复实例
        }
    }

    private void Start()
    {
        // 初始化按钮点击事件
        endTurnButton.onClick.AddListener(OnEndTurnButtonClick);

        // 更新初始回合信息
        UpdateTurnInfo();
    }

    // ========== UI更新方法 ==========
    /// <summary>
    /// 更新回合信息显示
    /// </summary>
    public void UpdateTurnInfo()
    {
        // 格式化显示当前回合数和已使用卡牌数
        turnInfoText.text = $"当前回合: {TurnManager.Instance.CurrentTurn} | " +
                          $"使用卡牌: {TurnManager.Instance.CardsPlayedThisTurn}/3";

        // 可以根据游戏状态改变文本颜色
        if (TurnManager.Instance.CardsPlayedThisTurn >= 3)
        {
            turnInfoText.color = Color.red;  // 达到出牌上限时变红
        }
        else
        {
            turnInfoText.color = Color.white;
        }
    }

    // ========== 按钮事件处理 ==========
    /// <summary>
    /// 结束回合按钮点击事件处理
    /// </summary>
    private void OnEndTurnButtonClick()
    {
        // 调用回合管理器结束当前玩家回合
        TurnManager.Instance.EndPlayerTurn();

        // 更新UI显示
        UpdateTurnInfo();

        // 可以添加按钮点击音效
        // AudioManager.Instance.PlayButtonClickSound();

        // 禁用按钮防止重复点击（可选）
        endTurnButton.interactable = false;
        StartCoroutine(EnableButtonAfterDelay(1f));
    }

    // ========== 辅助方法 ==========
    /// <summary>
    /// 延迟后重新启用按钮
    /// </summary>
    /// <param name="delay">延迟时间（秒）</param>
    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        endTurnButton.interactable = true;
    }
}