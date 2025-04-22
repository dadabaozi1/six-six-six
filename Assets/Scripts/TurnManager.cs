using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public enum GameState { PlayerTurn, EnemyTurn, GameOver }

    [Header("Settings")]
    public int darkTilesExpandMin = 1;
    public int darkTilesExpandMax = 2;

    private GameState currentState;
    private int cardsPlayedThisTurn;
    private const int maxCardsPerTurn = 3;
    public int CurrentTurn { get; private set; }
    public int CardsPlayedThisTurn => cardsPlayedThisTurn;

    private void Awake()
    {
        // ������ʼ��
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        CurrentTurn++;
        currentState = GameState.PlayerTurn;
        cardsPlayedThisTurn = 0;
        // ���俨���߼����ڿ���ϵͳ��ʵ��
        CardManager.Instance.DrawCardsForTurn();
        GameUIController.Instance.UpdateTurnInfo();
        Debug.Log("Player Turn Started");
    }

    public void EndPlayerTurn()
    {
        if (currentState != GameState.PlayerTurn) return;

        // ȷ�����úڰ��ؿ���չ
        DarkTileSystem.Instance.ExpandDarkTiles(
            Random.Range(darkTilesExpandMin, darkTilesExpandMax + 1)
        );

        StartPlayerTurn(); // ��ʼ�»غ�
    }

    private void ExpandDarkTiles()
    {
        int expandCount = Random.Range(darkTilesExpandMin, darkTilesExpandMax + 1);
        DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
    }
    
    public bool CanPlayCard()
    {
        return currentState == GameState.PlayerTurn &&
               cardsPlayedThisTurn < maxCardsPerTurn;
    }

    public void RegisterCardPlayed()
    {
        cardsPlayedThisTurn++;
        GameUIController.Instance.UpdateTurnInfo();
    }
}
