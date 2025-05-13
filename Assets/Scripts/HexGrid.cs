using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// ����������ϵͳ - �������������������ת�����ھӲ���
/// </summary>
public class HexGrid : MonoBehaviour
{
    [Header("��ͼ���")]
    public Tilemap tilemap;  // Unity��Tilemap�������

    // �������ھӷ���ƫ������ƽ�������β��֣�
    private static readonly Vector3Int[] neighborDirections =
    {
        // ��������ϵ�µ���������ƫ��
        new Vector3Int(1, 0, 0),    // �� (0��)
        new Vector3Int(0, 1, 0),    // ���� (60��)
        new Vector3Int(-1, 1, 0),   // ���� (120��)
        new Vector3Int(-1, 0, 0),   // �� (180��)
        new Vector3Int(0, -1, 0),   // ���� (240��)
        new Vector3Int(1, -1, 0)    // ���� (300��)
    };

    /// <summary>
    /// ��ȡָ��λ�õ�������Ч�ھӸ���
    /// </summary>
    /// <param name="position">���ĸ�������</param>
    /// <returns>��Ч�ھӸ����б�</returns>
    public List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // ���������������
        foreach (var dir in neighborDirections)
        {
            Vector3Int neighborPos = position + dir;

            // ֻ��Ӵ���tile���ھӣ���ֹԽ�磩
            if (tilemap.GetTile(neighborPos) != null)
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// ��������ת��������
    /// </summary>
    /// <param name="worldPosition">����ռ�����</param>
    /// <returns>��������</returns>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    /// <summary>
    /// ��������ת��������
    /// </summary>
    /// <param name="cellPosition">��������</param>
    /// <returns>����ռ���������</returns>
    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        // ��ȡ��������ĵ���������
        return tilemap.GetCellCenterWorld(cellPosition);
    }

    /// <summary>
    /// ���ָ������λ���Ƿ���Ч���Ƿ��ڵ�ͼ��Χ�ڣ�
    /// </summary>
    public bool IsValidCell(Vector3Int cellPosition)
    {
        return tilemap.GetTile(cellPosition) != null;
    }

    /// <summary>
    /// �������������θ���֮��ľ���
    /// </summary>
    public int GetDistance(Vector3Int a, Vector3Int b)
    {
        // ���������������㹫ʽ
        return (Mathf.Abs(a.x - b.x) +
               Mathf.Abs(a.y - b.y) +
               Mathf.Abs(a.z - b.z)) / 2;
    }
}
