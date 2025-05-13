using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ��Ϸ������ - ������Ϸ�������̺�״̬
/// </summary>
public class GameManager : MonoBehaviour
{
    // ========== ����ʵ�� ==========
    public static GameManager Instance;

    // ========== ��Ϸ���� ==========
    [Header("��Ϸ����")]
    public int initialCardCount = 5;  // ��ʼ��������
    public int cardsPerTurn = 2;      // ÿ�غϳ�������
    public int maxHandSize = 7;       // ��������

    // ========== �������� ==========
    [Header("��Ϸ��������")]
    public Transform playerPrefab;    // ���Ԥ����
    public Transform exitPoint;       // ���ڵ�λ��

    [Header("ϵͳ����")]
    public TurnManager turnManager;   // �غϹ�����
    public CardManager cardManager;   // ���ƹ�����

    // ========== �������ڷ��� ==========
    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // ��ѡ���糡������
        }
        else
        {
            Destroy(gameObject);  // ��ֹ�ظ�ʵ��
        }
    }

    private void Start()
    {
        InitializeGame();  // ��Ϸ��ʼ��
    }

    // ========== ��Ϸ���̿��� ==========
    /// <summary>
    /// ��ʼ����Ϸ״̬
    /// </summary>
    private void InitializeGame()
    {
        SpawnPlayer();               // �������
        turnManager.StartPlayerTurn(); // ��ʼ��һغ�
        // �����������ʼ������ϵͳ����������
    }

    /// <summary>
    /// ������ҽ�ɫ
    /// </summary>
    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, GetStartPosition(), Quaternion.identity);
        Debug.Log("���������");
    }

    /// <summary>
    /// ��ȡ�����ʼλ��
    /// </summary>
    /// <returns>��ʼ����</returns>
    private Vector3 GetStartPosition()
    {
        // ��ʵ�� - ʵ��Ӧ�ø��ݵ�ͼ���ȷ��
        // ���Կ��Ǵӵ�ͼ���ݻ������ļ��ж�ȡ
        return new Vector3(0, 0, 0);
    }

    // ========== ��Ϸ״̬���� ==========
    /// <summary>
    /// ��Ϸ��������
    /// </summary>
    /// <param name="win">�Ƿ�ʤ��</param>
    public void GameOver(bool win)
    {
        Debug.Log(win ? "��Ϸʤ����" : "��Ϸ����");

        // ���������Ӹ�����Ϸ�����߼���
        // 1. ��ʾ����UI
        // 2. ������Ϸ����
        // 3. ����ʤ��/ʧ�ܶ���
        // 4. �����ɾ͵�

        // ʾ������ͣ��Ϸ
        Time.timeScale = 0;

        // ע�⣺ʵ����Ŀ��Ӧ���ø����Ƶ�״̬����ϵͳ
    }
}