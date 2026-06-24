using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator : MonoBehaviour
{
    
    public static float HeadShotDamage(float atk, float cri)
    {
        float damage = 0f;
        damage = atk * cri;
        return damage;
    }
    public static float NormalDamage(float atk)
    {
        float damage = 0f;
        damage = atk;
        return damage;
    }

    // 수정
    public static int TotalDamage(float atk, float cri, float luckyShot, string type)
    {
        float damage = 0f;
        int resultDamage = 0;
        float luckyShotDmg = 0f;
        float rand = 0;
        rand = Random.Range(1f, 101f);
        if (luckyShot == 0)
        {
            luckyShotDmg = 0;
        }
        else if (luckyShot > 0f && luckyShot <= 100f)
        {
            // 100% 데미지 증가
            if (rand <= luckyShot)
            {
                luckyShotDmg = atk * 2f;
            }
            else
            {
                luckyShotDmg = atk;
            }
        }
        else if (luckyShot > 100f && luckyShot <= 200f)
        {
            // 200% 데미지 증가
            luckyShot = luckyShot - 100f;
            if (rand <= luckyShot)
            {
                luckyShotDmg = atk * 3f;
            }
            else
            {
                luckyShotDmg = atk * 2f;
            }
        }
        else if (luckyShot > 200f)
        {
            // 300% 데미지 증가
            luckyShot = luckyShot - 200f;
            if (rand <= luckyShot)
            {
                luckyShotDmg = atk * 4f;
            }
            else
            {
                luckyShotDmg = atk * 3f;
            }
        }

        if (type == "Head")
        {
            damage = HeadShotDamage(atk, cri) + luckyShotDmg;
        }
        else if(type == "Normal")
        {
            damage = NormalDamage(atk) + luckyShotDmg;
        }

        resultDamage = Mathf.CeilToInt(damage);

        return resultDamage;
    }
    //
    // totlaDamage = 현재 무기 데미지 계산한 TotalDamage 함수 들어감
    public static int ElementalDamage(Gun type, Monster hitmon, string hitType)
    {
        bool monIsShield = false;
        float damage = 0;
        if (hitmon.m_status.m_def > 0)
        {
            monIsShield = true;
        }
        int totalDamage = TotalDamage(type.m_enforceDamage, type.m_critical, type.m_luckyShot, hitType);
        if (type.m_gunElemental == Define.ElementalType.Fire)
        {
            //체력 150 %, 갑옷, 쉴드 75 % 데미지
            //발동 대상은 지속적으로 무기 데미지의 20 % 의 화염 데미지 적용
            //중첩, 갱신 x
            //근데 강한 대미지가 우선 적용됨
            if (monIsShield)
            {
                damage = (float)totalDamage * 0.75f;
            }
            else
            {
                damage = (float)totalDamage * 1.5f;
            }
            totalDamage = Mathf.CeilToInt(damage);

            return totalDamage;
        }
        else if (type.m_gunElemental == Define.ElementalType.Lightning)
        {
            //쉴드 100 %, 갑옷, 체력 100 % 데미지
            //발동 대상은 10 % 의 추가 데미지
            damage = (float)totalDamage * 0.1f;
            totalDamage += Mathf.CeilToInt(damage);

            return totalDamage;
        }
        else if (type.m_gunElemental == Define.ElementalType.Corrosion)
        {
            //갑옷 150 %, 쉴드, 체력 75 % 데미지
            //발동 대상은 이동 속도가 50 % 감소
            if (monIsShield)
            {
                damage = (float)totalDamage * 1.5f;
            }
            else
            {
                damage = (float)totalDamage * 0.25f;
            }
            totalDamage = Mathf.CeilToInt(damage);
            return totalDamage;
        }
        return totalDamage;
    }
    public static int GrenadeDamage(Monster hitmon)
    {
        bool monIsShield = false;
        float damage = 0;
        if (hitmon.m_status.m_def > 0)
        {
            monIsShield = true;
        }
        int totalDamage = 100;
        //갑옷 150 %, 쉴드, 체력 75 % 데미지
        //발동 대상은 이동 속도가 50 % 감소
        if (monIsShield)
        {
            damage = (float)totalDamage * 1.5f;
            totalDamage = Mathf.CeilToInt(damage);
            return totalDamage;
        }
        else
        {
            return totalDamage;
        }
        

        
    }
    public static int TrueDamageCal(float atk)
    {
        float damage = 0;
        int resultDamage = 0;

        damage = atk;

        resultDamage = Mathf.RoundToInt(damage);

        return resultDamage;
    }
}
