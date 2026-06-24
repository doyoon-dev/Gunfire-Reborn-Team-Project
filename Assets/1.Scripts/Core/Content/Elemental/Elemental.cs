using Define;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementalStat//무기 등에서 몬스터 공격할때 사용
{
    #region Constructor
    public ElementalStat() { m_type = ElementalType.None; m_probability = 0; }
    public ElementalStat(ElementalType type) : this() => m_type = type;
    public ElementalStat(ElementalType type, int probability) : this(type) => m_probability = probability;
    #endregion

    public ElementalType m_type;
    [Range(0, 100)] public int m_probability;
}

public class ElementalInfo
{

    ElementalInfo()=>Initialize();
    public ElementalInfo(Monster monster):this()=>m_monster= monster;

    private List<Elemental> m_elementalList = new List<Elemental>();
    private ElementalType m_active = ElementalType.None;
    private Monster m_monster;

    public void Initialize()
    {
        m_elementalList.Add(new Elemental_Fire());
        m_elementalList.Add(new Elemental_Lightning());
        m_elementalList.Add(new Elemental_Corrosion());
        m_elementalList.Add(new Elemental_Combustion());
        m_elementalList.Add(new Elemental_Manipulation());
        m_elementalList.Add(new Elemental_Miasma());
    }

    public void OnUpdate()
    {
        foreach (var elemental in m_elementalList)
        {
            if (elemental.IsActive)
            {
                m_active |= elemental.Type;
                elemental.OnUpdate(m_monster);
            }
            else m_active ^= elemental.Type;
        }
    }
    public void ApplyElemental(ElementalStat stat, int damage)
    {
        if (stat.m_type == ElementalType.None) return;

        int randnum = UnityEngine.Random.Range(0, 100);
        bool flag = (stat.m_probability - 1 >= randnum);

        if (flag)
        {
            ApplyElemental(stat.m_type, damage);
            CheckCombinationElemental(stat.m_type, damage);
        }
    }

    public void ApplyElemental(ElementalType type, int damage)
    {
        foreach (var elemental in m_elementalList)
        {
            if (elemental.Type == type)
            {
                elemental.Apply(true, damage);
                break;
            }
        }
    }

    public void CheckCombinationElemental(ElementalType type, int damage)
    {
        ElementalType temp = ElementalType.Fire;
        for (int i = 0; i < 3; i++)
        {
            temp = (ElementalType)((int)temp << i);
            if (type != temp &&
                (m_active & temp) != 0)
            {
                ApplyElemental(temp | type, damage);
            }
        }
    }

    public void Clear()
    {
        foreach (var elemental in m_elementalList)
        {
            elemental.ClearEffect(m_monster);
        }
    }
}

public abstract class Elemental
{
    const float m_lifeTime = 5f;
    float m_nextTime = 1f;
    float m_curTime = 0;
    bool m_isActive = false;

    int m_damage=0;

    public ElementalType Type => (ElementalType)System.Enum.Parse(typeof(ElementalType),
        GetType().ToString().Split('_')[1]);
    public bool IsActive => m_isActive;

    public void Apply(bool flag, int damage = 0)
    {
        m_damage = damage;
        if (m_isActive && flag)
        {
            m_isActive = flag;
            m_nextTime -= m_curTime;
            m_curTime = 0;
        }
        else m_isActive = flag;
    }
    public int Damage => m_damage;

    public void OnUpdate(Monster monster)
    {
        if (m_isActive)
        {
            AllTimeEffect(monster);
            m_curTime += Time.deltaTime;
            if (m_curTime >= m_nextTime)
            {
                m_nextTime++;
                EverySecondEffect(monster);
            }
            if (m_curTime >= m_lifeTime)
            {
                ClearEffect(monster);
            }
        }
    }

    public abstract void AllTimeEffect(Monster monster);
    public abstract void EverySecondEffect(Monster monster);
    public abstract void ClearEffect(Monster monster);

    protected void Clear()
    {
        m_isActive = false;
        m_curTime = 0;
        m_nextTime = 1f;
    }
}