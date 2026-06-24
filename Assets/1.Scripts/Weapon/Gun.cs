using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum BulletType
    {
        Bullet_Default,
        Bullet_Green = 2,
        Bullet_Blue,
        Bullet_Yellow
    };

    public BulletType m_bulletType;

    [Header("무기 이미지")]
    public SpriteRenderer m_currentGunImage;

    [Header("총알 발사 위치")]
    public GameObject[] m_firePos;

    [Header("총 이름")]
    public string m_gunName;

    [Header("총 속성")]
    public Define.ElementalType m_gunElemental;
    [Header("총 속성 확률")]
    public int m_elementalProbability;

    [Header("총 강화 단계")]
    public int m_gunEnforce;

    [Header("연사속도")]
    public float m_fireRate;
    [Header("재장전 속도")]
    public float m_reloadTime;
    [Header("탄속")]
    public float m_shotSpeed;

    [Header("총의 기본 데미지")]
    public int m_damage;
    [Header("총의 강화 데미지")]
    public int m_enforceDamage;
    [Header("치명타 배율")]
    public float m_critical;
    [Header("럭키샷 확률")]
    public int m_luckyShot;

    [Header("총알 재장전 개수")]
    public int m_reloadBulletCount;
    [Header("현재 장전된 총알의 개수")]
    public int m_currentBulletCount;
    [Header("반동 세기")]
    public float m_retroActionForce;
    [Header("총구 화염")]
    public ParticleSystem m_muzzleFlash;

    public Animator m_anim;

    private void Awake()
    {
        m_enforceDamage = m_damage;
        m_anim = GetComponent<Animator>();
    }
}
