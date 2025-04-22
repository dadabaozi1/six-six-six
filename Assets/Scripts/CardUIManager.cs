using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardUIManager : MonoBehaviour
{
    // ���ò���
    [Header("��������")]
    public GameObject cardUIPrefab;
    public Transform handPanel;

    [Header("��ק����")]
    public float dragAlpha = 0.7f;
    public Vector2 dragOffset = new Vector2(0, 20f); // ��קʱ����뿨�Ƶ�ƫ��

    // ����ʱ״̬
    private List<GameObject> cardUIInstances = new List<GameObject>();
    private GameObject draggedCard;
    private CardData currentDraggedCard;
    private Canvas rootCanvas;
    private GraphicRaycaster raycaster;  // ��������
    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (handPanel == null) handPanel = transform;
        // ��ʼ��raycaster
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


    public void UpdateHandUI(List<CardData> hand)
    {
        ClearHandUI();
        foreach (var card in hand)
        {
            var cardUI = Instantiate(cardUIPrefab, handPanel);
            SetupCardUI(cardUI, card);
            cardUIInstances.Add(cardUI);
        }
    }

    private void SetupCardUI(GameObject cardUI, CardData card)
    {
        // ��������
        var img = cardUI.GetComponent<Image>();
        img.sprite = card.icon;
        img.preserveAspect = true;

        var text = cardUI.GetComponentInChildren<Text>();
        if (text != null) text.text = card.type.ToString();

        // �¼�����
        var trigger = cardUI.GetComponent<EventTrigger>() ?? cardUI.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        AddTriggerEvent(trigger, EventTriggerType.BeginDrag, (data) => StartDrag(card, data));
        AddTriggerEvent(trigger, EventTriggerType.Drag, (data) => DuringDrag(data));
        AddTriggerEvent(trigger, EventTriggerType.EndDrag, (data) => EndDrag());
    }

    private void AddTriggerEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void StartDrag(CardData card, BaseEventData data)
    {
        // 1. ����Ҫ����
        Debug.Log("��ʼ��ק�¼�����"); // ȷ���¼����
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

        // 2. ʵ��������
        draggedCard = Instantiate(cardUIPrefab, rootCanvas.transform);
        if (draggedCard == null)
        {
            Debug.LogError("����ʵ����ʧ�ܣ�");
            return;
        }

        // 3. ���ÿ�������
        draggedCard.transform.SetAsLastSibling();

        Image img = draggedCard.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("����Ԥ����ȱ��Image�����");
            Destroy(draggedCard);
            return;
        }

        img.sprite = card?.icon; // ��ȫ����card
        img.raycastTarget = false;
        img.color = new Color(1, 1, 1, dragAlpha);

        // 4. ����λ��
        var pointerData = data as PointerEventData;
        if (pointerData != null)
        {
            Vector3 mousePos = new Vector3(pointerData.position.x, pointerData.position.y, 0);
            Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
            draggedCard.transform.position = mousePos + offset;
        }

        currentDraggedCard = card;
    }

    private void DuringDrag(BaseEventData data)
    {
        if (draggedCard == null) return;

        var pointerData = (PointerEventData)data;
        Vector3 mousePosition = new Vector3(pointerData.position.x, pointerData.position.y, 0);
        Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
        draggedCard.transform.position = mousePosition + offset;
    }

    private void EndDrag()
    {
        Debug.Log("������ק�¼�����");
        if (draggedCard == null) return;

        bool used = CheckIfUsedOnPlayArea();
        Destroy(draggedCard);

        if (used && currentDraggedCard != null)
        {
            CardManager.Instance.PlayCard(currentDraggedCard);
        }
        currentDraggedCard = null;
    }

    private bool CheckIfUsedOnPlayArea()
    {

        if (EventSystem.current == null || raycaster == null)
        {
            Debug.LogError("�¼�ϵͳȱʧ��");
            return false;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // �������б����߼�⵽�Ķ���
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        Debug.Log($"��⵽{results.Count}������");
        foreach (var result in results)
        {
            Debug.Log($"{result.gameObject.name} (Tag: {result.gameObject.tag})");

            if (result.gameObject.CompareTag("PlayArea"))
            {
                Debug.Log("��Ч�ͷ�����");
                return true;
            }
        }

        Debug.Log("δ�ҵ���Ч�ͷ�����");
        return false;
    }

    private void ClearHandUI()
    {
        foreach (var card in cardUIInstances)
        {
            if (card != null) Destroy(card);
        }
        cardUIInstances.Clear();
    }
}
