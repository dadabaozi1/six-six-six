using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ����������ϵͳ - ������Ϸ�е������������߼��͵ؿ������ж�
/// </summary>
public class HexGridSystem : MonoBehaviour
{
    [Header("��ͼ�㼶����")]
    public Tilemap baseMap;         // ������ͼ�㣨���Ρ����ܵؿ飩
    public Tilemap obstacleMap;     // �ϰ���㣨��ѡ��

    [Header("�ؿ���������")]
    public TileBase normalTile;      // ��ͨ�����ߵؿ�
    public TileBase darkTile;        // �ڰ��ؿ飨Σ������
    public TileBase cardReplenishTile; // ���Ʋ����ؿ�
    public TileBase blockPillarTile;  // ���赲��ʯ���ؿ�
    public TileBase exitTile;        // ���ڵؿ�

    // ����ʵ��
    public static HexGridSystem Instance;

    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // ����糡����ȡ��ע��
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��ȡ���������Ӧ����������������
    /// </summary>
    /// <param name="worldPosition">����ռ�����</param>
    /// <returns>��������������</returns>
    public Vector3Int GetHexAtPosition(Vector3 worldPosition)
    {
        return baseMap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// ��ȡ�����������������������
    /// </summary>
    /// <param name="hexCoords">��������</param>
    /// <returns>����ռ���������</returns>
    public Vector3 GetHexCenterPosition(Vector3Int hexCoords)
    {
        return baseMap.GetCellCenterWorld(hexCoords);
    }

    /// <summary>
    /// �ж��������Ƿ��ͨ��
    /// </summary>
    /// <param name="hexCoords">��������</param>
    /// <returns>�Ƿ��ͨ��</returns>
    public bool IsHexWalkable(Vector3Int hexCoords)
    {
        // ��������ͼ�ؿ�����
        TileBase baseTile = baseMap.GetTile(hexCoords);
        bool isWalkableTerrain = baseTile == normalTile ||
                               baseTile == cardReplenishTile ||
                               baseTile == blockPillarTile;

        // ����ϰ���㣨��������ã�
        bool hasObstacle = obstacleMap != null && obstacleMap.GetTile(hexCoords) != null;

        return isWalkableTerrain && !hasObstacle;
    }

    /// <summary>
    /// ��ȡ�����εؿ�����
    /// </summary>
    public TileBase GetHexType(Vector3Int hexCoords)
    {
        return baseMap.GetTile(hexCoords);
    }

    /// <summary>
    /// �ж��Ƿ������⹦�ܵؿ�
    /// </summary>
    public bool IsSpecialHex(Vector3Int hexCoords)
    {
        TileBase tile = baseMap.GetTile(hexCoords);
        return tile == cardReplenishTile || tile == blockPillarTile || tile == exitTile;
    }

    /// <summary>
    /// ��ȡָ��λ�õ�������Ч�ھӸ���
    /// </summary>
    public List<Vector3Int> GetNeighbors(Vector3Int centerHex)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),    // ��
            new Vector3Int(0, 1, 0),     // ����
            new Vector3Int(-1, 1, 0),    // ����
            new Vector3Int(-1, 0, 0),    // ��
            new Vector3Int(0, -1, 0),    // ����
            new Vector3Int(1, -1, 0)     // ����
        };

        foreach (var dir in directions)
        {
            Vector3Int neighbor = centerHex + dir;
            if (baseMap.GetTile(neighbor) != null) // ����Ƿ���ڸõؿ�
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}