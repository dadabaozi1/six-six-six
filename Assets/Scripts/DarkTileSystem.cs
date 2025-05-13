using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// �ڰ��ؿ�ϵͳ - ������Ϸ�кڰ��ؿ����չ��Ч��
/// </summary>
public class DarkTileSystem : MonoBehaviour
{
    [Header("��ͼ����")]
    public Tilemap baseMap;       // ������ͼTilemap���
    public TileBase darkTile;     // �ڰ��ؿ��Tile����
    public TileBase normalTile;   // ��ͨ�ؿ��Tile����

    public static DarkTileSystem Instance;  // ����ʵ��

    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null) Instance = this;
        else Destroy(gameObject);  // ��ֹ�ظ�ʵ��
    }

    /// <summary>
    /// ��չ�ڰ��ؿ�
    /// </summary>
    /// <param name="expandCount">�����չ����</param>
    public void ExpandDarkTiles(int expandCount)
    {
        List<Vector3Int> newDarkTiles = new List<Vector3Int>();  // �洢��Ҫ��ڰ����µؿ�

        // ������ͼ���е�Ԫ��
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            // �����ǰ�ؿ��Ǻڰ��ؿ�
            if (baseMap.GetTile(pos) == darkTile)
            {
                // ����������ڷ���
                for (int i = 0; i < 6; i++)
                {
                    Vector3Int neighbor = GetHexNeighbor(pos, i);  // ��ȡ���ڵؿ�����

                    // ������ڵؿ�����ͨ�ؿ���δ�ڴ������б��У���δ�ﵽ�����չ����
                    if (baseMap.GetTile(neighbor) == normalTile &&
                        !newDarkTiles.Contains(neighbor) &&
                        newDarkTiles.Count < expandCount)
                    {
                        newDarkTiles.Add(neighbor);  // ����������б�
                    }
                }
            }
        }

        // Ӧ���µĺڰ��ؿ�
        foreach (var tilePos in newDarkTiles)
        {
            baseMap.SetTile(tilePos, darkTile);  // ���ؿ�����Ϊ�ڰ�״̬
        }

        CheckPlayerCaught();  // �������Ƿ񱻺ڰ�����
    }

    /// <summary>
    /// ��ȡ������������ָ����������ڵؿ�����
    /// </summary>
    /// <param name="position">����λ��</param>
    /// <param name="direction">��������(0-5)</param>
    /// <returns>���ڵؿ�����</returns>
    private Vector3Int GetHexNeighbor(Vector3Int position, int direction)
    {
        // �������������������ƫ��������������ϵ��
        Vector3Int[] hexDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // �� (0��)
            new Vector3Int(1, -1, 0),  // ���� (60��)
            new Vector3Int(0, -1, 0),  // ���� (120��)
            new Vector3Int(-1, 0, 0),  // �� (180��)
            new Vector3Int(-1, 1, 0),  // ���� (240��)
            new Vector3Int(0, 1, 0)    // ���� (300��)
        };

        // ��������λ������
        return position + hexDirections[direction];
    }

    /// <summary>
    /// �������Ƿ񱻺ڰ��ؿ鲶��
    /// </summary>
    private void CheckPlayerCaught()
    {
        // ��ȡ������ڵ���������������
        Vector3Int playerHex = baseMap.WorldToCell(PlayerController.Instance.transform.position);

        // ����������λ���Ǻڰ��ؿ�
        if (baseMap.GetTile(playerHex) == darkTile)
        {
            // ������Ϸ������ʧ�ܣ�
            GameManager.Instance.GameOver(false);
        }
    }
}