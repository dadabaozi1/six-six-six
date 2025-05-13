using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ��ҿ��ƽű�
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;  // ����ƶ��ٶ�

    private Vector3 targetPosition; // ���Ҫ�ƶ�����Ŀ��λ��
    private bool isMoving;          // ����Ƿ������ƶ�
    public static PlayerController Instance; // ����ģʽ�����������ű�����

    private void Awake()
    {
        // ����ģʽ��ȷ��������ֻ��һ��PlayerControllerʵ��
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // ����Ѿ���ʵ���������ظ��Ķ���
    }

    private void Start()
    {
        // ��ʼ��Ŀ��λ��Ϊ��ǰ��ҵ�λ��
        targetPosition = transform.position;
    }

    private void Update()
    {
        // ��������ƶ�������֡ƽ���ƶ���Ŀ��λ��
        if (isMoving)
        {
            // ��ָ���ٶȽ���ǰλ���ƶ���Ŀ��λ��
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // �ж��Ƿ��Ѿ��ӽ�Ŀ��λ�ã�С��0.01����ʱ��Ϊ���
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // ֹͣ�ƶ�

                // TODO: ����Ŀ��λ�ú���߼�����������д
            }
        }
    }

    // ���⹫���ķ����������ƶ���ָ���������θ���λ��
    public void MoveToHex(Vector3 hexPosition)
    {
        targetPosition = hexPosition; // ����Ŀ��λ��
        isMoving = true;              // �����ƶ�״̬
    }

    // ����������"ExitPoint"��ǩ��������ײʱ����
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExitPoint"))
        {
            // ������Ϸ��������GameOver��������ʾʤ��
            GameManager.Instance.GameOver(true);
        }
    }
}