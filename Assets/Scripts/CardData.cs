// 导入必要的Unity命名空间
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 该属性允许在Unity编辑器中创建此ScriptableObject的实例
// 在资源创建菜单中会显示："创建 > Hex Escape > 卡牌数据"
[CreateAssetMenu(fileName = "NewCard", menuName = "Hex Escape/Card Data")]
public class CardData : ScriptableObject  // 继承自ScriptableObject，用于存储数据
{
    // 定义游戏中可用的卡牌类型枚举
    public enum CardType
    {
        Move,        // 移动卡
        EnergyStone, // 能量石卡
        Replenish,   // 补给卡
        Shock,       // 电击卡
        Teleport     // 传送卡
    }

    public CardType type;     // 当前卡牌的类型
    public Sprite icon;       // 卡牌显示的图标
    public string description;// 卡牌描述文本

    [Header("移动卡参数")]      // Unity编辑器中的分组标题
    public int moveDistance = 1;  // 移动卡可移动的格数

    [Header("电击卡参数")]
    public int shockDistance = 3; // 电击卡的攻击距离
}