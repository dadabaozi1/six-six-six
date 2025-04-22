using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardManager : MonoBehaviour
{ // ��ӵ���ģʽ

    [Header("Settings")]
    public List<CardData> cardDeck;
    public int initialHandSize = 5;
    public int cardsPerTurn = 2;
    public int maxHandSize = 7;

    private List<CardData> currentDeck;
    private List<CardData> currentHand;
    private List<CardData> discardPile = new List<CardData>(); // �������ƶ�

    public CardUIManager cardUIManager;
    public static CardManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeDeck();
        DrawInitialHand();
    }

    private void InitializeDeck()
    {
        currentDeck = new List<CardData>(cardDeck);
        ShuffleDeck();
        Debug.Log($"��ʼ�ƿ�: {currentDeck.Count}�ſ�");
    }

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

    private void DrawInitialHand()
    {
        currentHand = new List<CardData>();
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCardWithReshuffle();
        }
    }

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
                Debug.Log("�����Ѵ����ޣ����ٳ鿨");
                break;
            }
        }
    }

    // ���������Զ�ϴ�Ƶĳ鿨����
    private void DrawCardWithReshuffle()
    {
        if (currentDeck.Count == 0)
        {
            ReshuffleDiscardPile();
        }

        if (currentDeck.Count > 0)
        {
            DrawCard();
        }
        else
        {
            Debug.LogWarning("�ƿ�����ƶѾ��ѿգ��޷��鿨");
        }
    }

    // ����������ϴ�����ƶ�
    private void ReshuffleDiscardPile()
    {
        if (discardPile.Count > 0)
        {
            Debug.Log($"��{discardPile.Count}������ϴ���ƿ�");
            currentDeck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }
    }

    private void DrawCard()
    {
        CardData drawnCard = currentDeck[0];
        currentDeck.RemoveAt(0);
        currentHand.Add(drawnCard);
        cardUIManager.UpdateHandUI(currentHand);

        Debug.Log($"�鿨: {drawnCard.type} (ʣ��:{currentDeck.Count})");
    }

    public bool PlayCard(CardData card)
    {
        if (!TurnManager.Instance.CanPlayCard())
        {
            Debug.Log("��ǰ����ʹ�ÿ��ƣ�������һغϻ��Ѵ��������");
            return false;
        }

        Debug.Log($"����ʹ�ÿ��ƣ�{card.type}");

        // ʵ�ʿ���Ч�����ã�ʾ�����ƶ�����
        switch (card.type)
        {
            case CardData.CardType.Move:
                PlayMoveCard(Vector3Int.right); // ʾ������
                break;
                // ������������...
        }

        TurnManager.Instance.RegisterCardPlayed();
        return true;
    }

    // ��������Ч���������ֲ���...
    public void PlayMoveCard(Vector3Int direction) { /* ԭ��ʵ�� */ }
    public void PlayShockCard(Vector3Int direction) { /* ԭ��ʵ�� */ }
    public void PlayEnergyStoneCard(Vector3Int pillarPosition) { /* ԭ��ʵ�� */ }
    public void PlayTeleportCard() { /* ԭ��ʵ�� */ }
}
