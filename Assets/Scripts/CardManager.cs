// �����Ҫ��Unity�����ռ�
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardManager : MonoBehaviour
{
    // ========== ����ģʽ ==========
    [Header("��Ϸ����")]
    public List<CardData> cardDeck;        // ��ʼ���ƿ⣨�ڱ༭�������ã�
    public int initialHandSize = 5;        // ��ʼ��������
    public int cardsPerTurn = 2;           // ÿ�غϳ鿨����
    public int maxHandSize = 7;            // ��������

    private List<CardData> currentDeck;    // ��ǰ�ƿ⣨����ʱʹ�ã�
    private List<CardData> currentHand;    // ��ǰ����
    private List<CardData> discardPile = new List<CardData>(); // ���ƶ�

    [Header("UI����")]
    public CardUIManager cardUIManager;    // ����UI����������

    public static CardManager Instance;    // ����ʵ��

    // ========== �������ڷ��� ==========
    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null) Instance = this;
        else Destroy(gameObject);  // ��ֹ�ظ�ʵ��
    }

    private void Start()
    {
        InitializeDeck();     // ��ʼ���ƿ�
        DrawInitialHand();    // ��ȡ��ʼ����
    }

    // ========== �ƿ���� ==========
    /// <summary>
    /// ��ʼ���ƿ⣨����Ԥ���ƿⲢϴ�ƣ�
    /// </summary>
    private void InitializeDeck()
    {
        currentDeck = new List<CardData>(cardDeck);  // �����ʼ�ƿ�
        ShuffleDeck();  // ϴ��
        Debug.Log($"��ʼ�ƿ�: {currentDeck.Count}�ſ�");
    }

    /// <summary>
    /// ϴ���㷨��Fisher-Yatesϴ�Ʒ���
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

    // ========== �鿨�߼� ==========
    /// <summary>
    /// ��ȡ��ʼ����
    /// </summary>
    private void DrawInitialHand()
    {
        currentHand = new List<CardData>();
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCardWithReshuffle();  // �Զ������ƿⲻ������
        }
    }

    /// <summary>
    /// ÿ�غϿ�ʼʱ�ĳ鿨�߼�
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
                Debug.Log("�����Ѵ����ޣ����ٳ鿨");
                break;
            }
        }
    }

    /// <summary>
    /// ���ܳ鿨���Զ�����ƿⲢϴ�����ƶѣ�
    /// </summary>
    private void DrawCardWithReshuffle()
    {
        if (currentDeck.Count == 0)
        {
            ReshuffleDiscardPile();  // �ƿ��ʱϴ�����ƶ�
        }

        if (currentDeck.Count > 0)
        {
            DrawCard();  // �����鿨
        }
        else
        {
            Debug.LogWarning("�ƿ�����ƶѾ��ѿգ��޷��鿨");
        }
    }

    /// <summary>
    /// �����ƶ�ϴ���ƿ�
    /// </summary>
    private void ReshuffleDiscardPile()
    {
        if (discardPile.Count > 0)
        {
            Debug.Log($"��{discardPile.Count}������ϴ���ƿ�");
            currentDeck.AddRange(discardPile);  // �ϲ����ƶ�
            discardPile.Clear();              // ������ƶ�
            ShuffleDeck();                     // ����ϴ��
        }
    }

    /// <summary>
    /// �����鿨���������ƿⶥ��һ���ƣ�
    /// </summary>
    private void DrawCard()
    {
        CardData drawnCard = currentDeck[0];  // ȡ��һ����
        currentDeck.RemoveAt(0);              // ���ƿ��Ƴ�
        currentHand.Add(drawnCard);           // ��������
        cardUIManager.UpdateHandUI(currentHand);  // ����UI

        Debug.Log($"�鿨: {drawnCard.type} (ʣ��:{currentDeck.Count})");
    }

    // ========== ����ʹ�� ==========
    /// <summary>
    /// ����ʹ�ÿ��ƣ�����ڣ�
    /// </summary>
    /// <param name="card">Ҫʹ�õĿ���</param>
    /// <returns>�Ƿ�ʹ�óɹ�</returns>
    public bool PlayCard(CardData card)
    {
        if (!TurnManager.Instance.CanPlayCard())
        {
            Debug.Log("��ǰ����ʹ�ÿ��ƣ�������һغϻ��Ѵ��������");
            return false;
        }

        Debug.Log($"����ʹ�ÿ��ƣ�{card.type}");

        // ���ݿ������͵��ò�ͬЧ��
        switch (card.type)
        {
            case CardData.CardType.Move:
                PlayMoveCard(Vector3Int.right); // ʾ������ʵ��Ӧ�������ѡ��ķ���
                break;
                // �����������ʹ���...
        }

        // ��ʹ�ú�Ŀ����������ƶ�
        currentHand.Remove(card);
        discardPile.Add(card);
        cardUIManager.UpdateHandUI(currentHand);

        TurnManager.Instance.RegisterCardPlayed();  // ֪ͨ�غϹ�����
        return true;
    }

    // ========== ����Ч��ʵ�� ==========
    public void PlayMoveCard(Vector3Int direction)
    {
        /* �ƶ���Ч��ʵ�֣���ָ�������ƶ���� */
    }

    public void PlayShockCard(Vector3Int direction)
    {
        /* �����Ч��ʵ�֣�����ָ��������� */
    }

    public void PlayEnergyStoneCard(Vector3Int pillarPosition)
    {
        /* ����ʯ��Ч��������ָ��λ�õ�ʯ�� */
    }

    public void PlayTeleportCard()
    {
        /* ���Ϳ�Ч�������������� */
    }
}