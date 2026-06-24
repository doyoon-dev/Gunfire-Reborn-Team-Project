using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Status
{
    public float m_hp;
    public float m_atk;
    public float m_def;
    public float m_cri;
    public int m_luckyShot;

    public Status(float hp, float atk, float def, float cri, int luckyShot)
    {
        this.m_hp = hp;
        this.m_atk = atk;
        this.m_def = def;
        this.m_cri = cri;
        this.m_luckyShot = luckyShot;
    }
}
