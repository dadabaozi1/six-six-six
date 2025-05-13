// �����Ҫ��Unity�����ռ�
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������������Unity�༭���д�����ScriptableObject��ʵ��
// ����Դ�����˵��л���ʾ��"���� > Hex Escape > ��������"
[CreateAssetMenu(fileName = "NewCard", menuName = "Hex Escape/Card Data")]
public class CardData : ScriptableObject  // �̳���ScriptableObject�����ڴ洢����
{
    // ������Ϸ�п��õĿ�������ö��
    public enum CardType
    {
        Move,        // �ƶ���
        EnergyStone, // ����ʯ��
        Replenish,   // ������
        Shock,       // �����
        Teleport     // ���Ϳ�
    }

    public CardType type;     // ��ǰ���Ƶ�����
    public Sprite icon;       // ������ʾ��ͼ��
    public string description;// ���������ı�

    [Header("�ƶ�������")]      // Unity�༭���еķ������
    public int moveDistance = 1;  // �ƶ������ƶ��ĸ���

    [Header("���������")]
    public int shockDistance = 3; // ������Ĺ�������
}