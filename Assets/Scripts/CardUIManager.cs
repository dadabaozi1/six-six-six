using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌UI管理器，负责处理卡牌的显示和拖拽交互逻辑
/// </summary>
public class CardUIManager : MonoBehaviour
{
    // ========== 配置参数 ==========
    [Header("必需引用")]
    public GameObject cardUIPrefab;    // 卡牌UI预制体
    public Transform handPanel;        // 手牌存放的父节点

    [Header("拖拽设置")]
    public float dragAlpha = 0.7f;     // 拖拽时卡牌的透明度
    public Vector2 dragOffset = new Vector2(0, 20f); // 拖拽时鼠标与卡牌的偏移量

    // ========== 运行时状态 ==========
    private List<GameObject> cardUIInstances = new List<GameObject>(); // 当前显示的所有卡牌UI实例
    private GameObject draggedCard;    // 当前被拖拽的卡牌对象
    private CardData currentDraggedCard; // 当前被拖拽的卡牌数据
    private Canvas rootCanvas;         // 根画布
    private GraphicRaycaster raycaster; // UI射线检测器

    // ========== 初始化 ==========
    private void Awake()
    {
        // 获取必要的组件引用
        rootCanvas = GetComponentInParent<Canvas>();
        if (handPanel == null) handPanel = transform;

        // 初始化射线检测器
        raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError("场景中未找到GraphicRaycaster组件！");
            }
        }
    }

    // ========== 手牌UI更新 ==========
    /// <summary>
    /// 根据当前手牌数据更新UI显示
    /// </summary>
    /// <param name="hand">当前手牌数据列表</param>
    public void UpdateHandUI(List<CardData> hand)
    {
        ClearHandUI();  // 先清空现有手牌

        // 为每张卡牌创建UI实例
        foreach (var card in hand)
        {
            var cardUI = Instantiate(cardUIPrefab, handPanel);
            SetupCardUI(cardUI, card);  // 设置卡牌UI
            cardUIInstances.Add(cardUI);
        }
    }

    /// <summary>
    /// 设置单个卡牌UI的显示和交互
    /// </summary>
    private void SetupCardUI(GameObject cardUI, CardData card)
    {
        // 基础显示设置
        var img = cardUI.GetComponent<Image>();
        img.sprite = card.icon;        // 设置卡牌图标
        img.preserveAspect = true;     // 保持图标比例

        // 设置卡牌类型文本（如果有）
        var text = cardUI.GetComponentInChildren<Text>();
        if (text != null) text.text = card.type.ToString();

        // 设置交互事件
        var trigger = cardUI.GetComponent<EventTrigger>() ?? cardUI.AddComponent<EventTrigger>();
        trigger.triggers.Clear();  // 清除旧事件

        // 添加拖拽相关事件
        AddTriggerEvent(trigger, EventTriggerType.BeginDrag, (data) => StartDrag(card, data));
        AddTriggerEvent(trigger, EventTriggerType.Drag, (data) => DuringDrag(data));
        AddTriggerEvent(trigger, EventTriggerType.EndDrag, (data) => EndDrag());
    }

    /// <summary>
    /// 为EventTrigger添加事件监听
    /// </summary>
    private void AddTriggerEvent(EventTrigger trigger, EventTriggerType type,
        UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    // ========== 拖拽逻辑 ==========
    /// <summary>
    /// 开始拖拽卡牌
    /// </summary>
    private void StartDrag(CardData card, BaseEventData data)
    {
        Debug.Log("开始拖拽事件触发");

        // 1. 检查必要引用
        if (cardUIPrefab == null)
        {
            Debug.LogError("cardUIPrefab 未赋值！");
            return;
        }

        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas == null)
            {
                Debug.LogError("未找到Canvas！");
                return;
            }
        }

        // 2. 创建拖拽用的卡牌副本
        draggedCard = Instantiate(cardUIPrefab, rootCanvas.transform);
        if (draggedCard == null)
        {
            Debug.LogError("卡牌实例化失败！");
            return;
        }

        // 3. 设置拖拽卡牌属性
        draggedCard.transform.SetAsLastSibling();  // 确保显示在最上层

        Image img = draggedCard.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("卡牌预制体缺少Image组件！");
            Destroy(draggedCard);
            return;
        }

        img.sprite = card?.icon;  // 设置卡牌图标
        img.raycastTarget = false; // 禁用射线检测，避免干扰
        img.color = new Color(1, 1, 1, dragAlpha);  // 设置透明度

        // 4. 设置初始位置（带偏移）
        var pointerData = data as PointerEventData;
        if (pointerData != null)
        {
            Vector3 mousePos = new Vector3(pointerData.position.x, pointerData.position.y, 0);
            Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
            draggedCard.transform.position = mousePos + offset;
        }

        currentDraggedCard = card;  // 保存当前拖拽的卡牌数据
    }

    /// <summary>
    /// 拖拽过程中更新卡牌位置
    /// </summary>
    private void DuringDrag(BaseEventData data)
    {
        if (draggedCard == null) return;

        var pointerData = (PointerEventData)data;
        Vector3 mousePosition = new Vector3(pointerData.position.x, pointerData.position.y, 0);
        Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
        draggedCard.transform.position = mousePosition + offset;
    }

    /// <summary>
    /// 结束拖拽时的处理
    /// </summary>
    private void EndDrag()
    {
        Debug.Log("结束拖拽事件触发");
        if (draggedCard == null) return;

        // 检查是否拖拽到了有效区域
        bool used = CheckIfUsedOnPlayArea();
        Destroy(draggedCard);  // 无论是否使用都要销毁拖拽副本

        if (used && currentDraggedCard != null)
        {
            // 通知卡牌管理器使用卡牌
            CardManager.Instance.PlayCard(currentDraggedCard);
        }
        currentDraggedCard = null;
    }

    /// <summary>
    /// 检查是否将卡牌拖拽到了有效使用区域
    /// </summary>
    private bool CheckIfUsedOnPlayArea()
    {
        if (EventSystem.current == null || raycaster == null)
        {
            Debug.LogError("事件系统缺失！");
            return false;
        }

        // 创建射线检测事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // 执行射线检测
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        Debug.Log($"检测到{results.Count}个对象：");
        foreach (var result in results)
        {
            Debug.Log($"{result.gameObject.name} (Tag: {result.gameObject.tag})");

            // 检查是否拖拽到了标记为"PlayArea"的区域
            if (result.gameObject.CompareTag("PlayArea"))
            {
                Debug.Log("有效释放区域");
                return true;
            }
        }

        Debug.Log("未找到有效释放区域");
        return false;
    }

    // ========== 辅助方法 ==========
    /// <summary>
    /// 清空当前所有手牌UI
    /// </summary>
    private void ClearHandUI()
    {
        foreach (var card in cardUIInstances)
        {
            if (card != null) Destroy(card);
        }
        cardUIInstances.Clear();
    }
}