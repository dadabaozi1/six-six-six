using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 玩家控制脚本
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;  // 玩家移动速度

    private Vector3 targetPosition; // 玩家要移动到的目标位置
    private bool isMoving;          // 玩家是否正在移动
    public static PlayerController Instance; // 单例模式，方便其他脚本调用

    private void Awake()
    {
        // 单例模式：确保场景中只有一个PlayerController实例
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 如果已经有实例，销毁重复的对象
    }

    private void Start()
    {
        // 初始化目标位置为当前玩家的位置
        targetPosition = transform.position;
    }

    private void Update()
    {
        // 如果正在移动，则逐帧平滑移动到目标位置
        if (isMoving)
        {
            // 以指定速度将当前位置移动到目标位置
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 判断是否已经接近目标位置（小于0.01距离时视为到达）
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // 停止移动

                // TODO: 到达目标位置后的逻辑可以在这里写
            }
        }
    }

    // 对外公开的方法，用于移动到指定的六边形格子位置
    public void MoveToHex(Vector3 hexPosition)
    {
        targetPosition = hexPosition; // 设置目标位置
        isMoving = true;              // 启动移动状态
    }

    // 当玩家与带有"ExitPoint"标签的物体碰撞时触发
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExitPoint"))
        {
            // 调用游戏管理器的GameOver方法，表示胜利
            GameManager.Instance.GameOver(true);
        }
    }
}