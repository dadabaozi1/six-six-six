using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ����UI���������������Ƶ���ʾ����ק�����߼�
/// </summary>
public class CardUIManager : MonoBehaviour
{
    // ========== ���ò��� ==========
    [Header("��������")]
    public GameObject cardUIPrefab;    // ����UIԤ����
    public Transform handPanel;        // ���ƴ�ŵĸ��ڵ�

    [Header("��ק����")]
    public float dragAlpha = 0.7f;     // ��קʱ���Ƶ�͸����
    public Vector2 dragOffset = new Vector2(0, 20f); // ��קʱ����뿨�Ƶ�ƫ����

    // ========== ����ʱ״̬ ==========
    private List<GameObject> cardUIInstances = new List<GameObject>(); // ��ǰ��ʾ�����п���UIʵ��
    private GameObject draggedCard;    // ��ǰ����ק�Ŀ��ƶ���
    private CardData currentDraggedCard; // ��ǰ����ק�Ŀ�������
    private Canvas rootCanvas;         // ������
    private GraphicRaycaster raycaster; // UI���߼����

    // ========== ��ʼ�� ==========
    private void Awake()
    {
        // ��ȡ��Ҫ���������
        rootCanvas = GetComponentInParent<Canvas>();
        if (handPanel == null) handPanel = transform;

        // ��ʼ�����߼����
        raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError("������δ�ҵ�GraphicRaycaster�����");
            }
        }
    }

    // ========== ����UI���� ==========
    /// <summary>
    /// ���ݵ�ǰ�������ݸ���UI��ʾ
    /// </summary>
    /// <param name="hand">��ǰ���������б�</param>
    public void UpdateHandUI(List<CardData> hand)
    {
        ClearHandUI();  // �������������

        // Ϊÿ�ſ��ƴ���UIʵ��
        foreach (var card in hand)
        {
            var cardUI = Instantiate(cardUIPrefab, handPanel);
            SetupCardUI(cardUI, card);  // ���ÿ���UI
            cardUIInstances.Add(cardUI);
        }
    }

    /// <summary>
    /// ���õ�������UI����ʾ�ͽ���
    /// </summary>
    private void SetupCardUI(GameObject cardUI, CardData card)
    {
        // ������ʾ����
        var img = cardUI.GetComponent<Image>();
        img.sprite = card.icon;        // ���ÿ���ͼ��
        img.preserveAspect = true;     // ����ͼ�����

        // ���ÿ��������ı�������У�
        var text = cardUI.GetComponentInChildren<Text>();
        if (text != null) text.text = card.type.ToString();

        // ���ý����¼�
        var trigger = cardUI.GetComponent<EventTrigger>() ?? cardUI.AddComponent<EventTrigger>();
        trigger.triggers.Clear();  // ������¼�

        // �����ק����¼�
        AddTriggerEvent(trigger, EventTriggerType.BeginDrag, (data) => StartDrag(card, data));
        AddTriggerEvent(trigger, EventTriggerType.Drag, (data) => DuringDrag(data));
        AddTriggerEvent(trigger, EventTriggerType.EndDrag, (data) => EndDrag());
    }

    /// <summary>
    /// ΪEventTrigger����¼�����
    /// </summary>
    private void AddTriggerEvent(EventTrigger trigger, EventTriggerType type,
        UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    // ========== ��ק�߼� ==========
    /// <summary>
    /// ��ʼ��ק����
    /// </summary>
    private void StartDrag(CardData card, BaseEventData data)
    {
        Debug.Log("��ʼ��ק�¼�����");

        // 1. ����Ҫ����
        if (cardUIPrefab == null)
        {
            Debug.LogError("cardUIPrefab δ��ֵ��");
            return;
        }

        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas == null)
            {
                Debug.LogError("δ�ҵ�Canvas��");
                return;
            }
        }

        // 2. ������ק�õĿ��Ƹ���
        draggedCard = Instantiate(cardUIPrefab, rootCanvas.transform);
        if (draggedCard == null)
        {
            Debug.LogError("����ʵ����ʧ�ܣ�");
            return;
        }

        // 3. ������ק��������
        draggedCard.transform.SetAsLastSibling();  // ȷ����ʾ�����ϲ�

        Image img = draggedCard.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("����Ԥ����ȱ��Image�����");
            Destroy(draggedCard);
            return;
        }

        img.sprite = card?.icon;  // ���ÿ���ͼ��
        img.raycastTarget = false; // �������߼�⣬�������
        img.color = new Color(1, 1, 1, dragAlpha);  // ����͸����

        // 4. ���ó�ʼλ�ã���ƫ�ƣ�
        var pointerData = data as PointerEventData;
        if (pointerData != null)
        {
            Vector3 mousePos = new Vector3(pointerData.position.x, pointerData.position.y, 0);
            Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
            draggedCard.transform.position = mousePos + offset;
        }

        currentDraggedCard = card;  // ���浱ǰ��ק�Ŀ�������
    }

    /// <summary>
    /// ��ק�����и��¿���λ��
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
    /// ������קʱ�Ĵ���
    /// </summary>
    private void EndDrag()
    {
        Debug.Log("������ק�¼�����");
        if (draggedCard == null) return;

        // ����Ƿ���ק������Ч����
        bool used = CheckIfUsedOnPlayArea();
        Destroy(draggedCard);  // �����Ƿ�ʹ�ö�Ҫ������ק����

        if (used && currentDraggedCard != null)
        {
            // ֪ͨ���ƹ�����ʹ�ÿ���
            CardManager.Instance.PlayCard(currentDraggedCard);
        }
        currentDraggedCard = null;
    }

    /// <summary>
    /// ����Ƿ񽫿�����ק������Чʹ������
    /// </summary>
    private bool CheckIfUsedOnPlayArea()
    {
        if (EventSystem.current == null || raycaster == null)
        {
            Debug.LogError("�¼�ϵͳȱʧ��");
            return false;
        }

        // �������߼���¼�
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // ִ�����߼��
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        Debug.Log($"��⵽{results.Count}������");
        foreach (var result in results)
        {
            Debug.Log($"{result.gameObject.name} (Tag: {result.gameObject.tag})");

            // ����Ƿ���ק���˱��Ϊ"PlayArea"������
            if (result.gameObject.CompareTag("PlayArea"))
            {
                Debug.Log("��Ч�ͷ�����");
                return true;
            }
        }

        Debug.Log("δ�ҵ���Ч�ͷ�����");
        return false;
    }

    // ========== �������� ==========
    /// <summary>
    /// ��յ�ǰ��������UI
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