using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ϷUI������ - ������Ϸ���潻������ʾ
/// </summary>
public class GameUIController : MonoBehaviour
{
    // ========== UI������� ==========
    [Header("UI���")]
    public Text turnInfoText;        // �غ���Ϣ��ʾ�ı�
    public Button endTurnButton;     // �����غϰ�ť

    // ========== ����ʵ�� ==========
    public static GameUIController Instance;

    // ========== �������ڷ��� ==========
    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // �����Ҫ�糡������ȡ��ע��
        }
        else
        {
            Destroy(gameObject);    // ��ֹ�ظ�ʵ��
        }
    }

    private void Start()
    {
        // ��ʼ����ť����¼�
        endTurnButton.onClick.AddListener(OnEndTurnButtonClick);

        // ���³�ʼ�غ���Ϣ
        UpdateTurnInfo();
    }

    // ========== UI���·��� ==========
    /// <summary>
    /// ���»غ���Ϣ��ʾ
    /// </summary>
    public void UpdateTurnInfo()
    {
        // ��ʽ����ʾ��ǰ�غ�������ʹ�ÿ�����
        turnInfoText.text = $"��ǰ�غ�: {TurnManager.Instance.CurrentTurn} | " +
                          $"ʹ�ÿ���: {TurnManager.Instance.CardsPlayedThisTurn}/3";

        // ���Ը�����Ϸ״̬�ı��ı���ɫ
        if (TurnManager.Instance.CardsPlayedThisTurn >= 3)
        {
            turnInfoText.color = Color.red;  // �ﵽ��������ʱ���
        }
        else
        {
            turnInfoText.color = Color.white;
        }
    }

    // ========== ��ť�¼����� ==========
    /// <summary>
    /// �����غϰ�ť����¼�����
    /// </summary>
    private void OnEndTurnButtonClick()
    {
        // ���ûغϹ�����������ǰ��һغ�
        TurnManager.Instance.EndPlayerTurn();

        // ����UI��ʾ
        UpdateTurnInfo();

        // ������Ӱ�ť�����Ч
        // AudioManager.Instance.PlayButtonClickSound();

        // ���ð�ť��ֹ�ظ��������ѡ��
        endTurnButton.interactable = false;
        StartCoroutine(EnableButtonAfterDelay(1f));
    }

    // ========== �������� ==========
    /// <summary>
    /// �ӳٺ��������ð�ť
    /// </summary>
    /// <param name="delay">�ӳ�ʱ�䣨�룩</param>
    private IEnumerator EnableButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        endTurnButton.interactable = true;
    }
}