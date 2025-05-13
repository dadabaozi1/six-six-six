using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �غϹ���������������������˵Ļغ�����
public class TurnManager : MonoBehaviour
{
    // ����ģʽ�����������ű����� TurnManager.Instance
    public static TurnManager Instance { get; private set; }

    // ��Ϸ״̬ö�٣���һغϡ����˻غϡ���Ϸ����
    public enum GameState { PlayerTurn, EnemyTurn, GameOver }

    [Header("Settings")]
    public int darkTilesExpandMin = 1; // ÿ�غϺڰ��ؿ���չ��С����
    public int darkTilesExpandMax = 2; // ÿ�غϺڰ��ؿ���չ�������

    private GameState currentState;    // ��ǰ��Ϸ״̬
    private int cardsPlayedThisTurn;   // ���غ��Ѵ���Ŀ�����
    private const int maxCardsPerTurn = 3; // ÿ�غ����ɴ���Ŀ�����

    public int CurrentTurn { get; private set; } // ��ǰ�غ���
    public int CardsPlayedThisTurn => cardsPlayedThisTurn; // �ⲿֻ�������Ѵ���Ŀ�����

    private void Awake()
    {
        // ������ʼ��
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // ����������ʵ���������ظ���
        }
    }

    private void Start()
    {
        // ��Ϸ��ʼʱ����ʼ����һغ�
        StartPlayerTurn();
    }

    // ��ʼ��һغ�
    public void StartPlayerTurn()
    {
        CurrentTurn++; // �غ���+1
        currentState = GameState.PlayerTurn; // ����Ϊ��һغ�
        cardsPlayedThisTurn = 0; // ���ñ��غ��Ѵ���Ŀ�����

        // �鿨�߼������ɿ��ƹ�����ʵ��
        CardManager.Instance.DrawCardsForTurn();

        // ����UI�غ���Ϣ
        GameUIController.Instance.UpdateTurnInfo();

        Debug.Log("Player Turn Started"); // ������Ϣ
    }

    // ������һغ�
    public void EndPlayerTurn()
    {
        if (currentState != GameState.PlayerTurn) return; // ����һغ�ʱ��ִ��

        // �ڰ��ؿ���չ�߼�
        DarkTileSystem.Instance.ExpandDarkTiles(
            Random.Range(darkTilesExpandMin, darkTilesExpandMax + 1)
        );

        // ����������������һ��һغ�
        StartPlayerTurn();
    }

    // ��δ��ʹ�õ�˽�з�������չ�ڰ��ؿ�
    private void ExpandDarkTiles()
    {
        int expandCount = Random.Range(darkTilesExpandMin, darkTilesExpandMax + 1);
        DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
    }

    // �ж��Ƿ��ܼ�������
    public bool CanPlayCard()
    {
        return currentState == GameState.PlayerTurn &&
               cardsPlayedThisTurn < maxCardsPerTurn;
    }

    // ע���Ѵ���Ŀ�������ÿ���һ�ſ�����һ�Σ�
    public void RegisterCardPlayed()
    {
        cardsPlayedThisTurn++; // �Ѵ��������+1

        // ����UI�غ���Ϣ
        GameUIController.Instance.UpdateTurnInfo();
    }
}