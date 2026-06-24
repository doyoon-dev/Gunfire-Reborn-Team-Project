using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Rate
{
    Normal,
    Rare,
    Legendary, 
    Cursed,
    Enhanced
}

enum EffectType
{
    Damage,
    MaxHp,
    AttackSpeed,
    MoveSpeed,
};
struct ScrollEffect
{
    EffectType adfasd;
    //스탯
    public float m_value;
}

public class OccultScroll : MonoBehaviour
{
    Rate m_rate; //희귀도
    int m_maxStack;//최대 중첩 수
    string m_name;
    Sprite m_icon;
    string m_description;//설명

    //적용 조건
    List<ScrollEffect> m_scrollEffects;//효과
}
