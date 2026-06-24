using UnityEngine;
//발동 대상은 무기 데미지의 200%피해, 주변 5m 100% 범위 피해
public class Elemental_Combustion : Elemental
{
    float m_directDamageRatio = 2f;
    float m_splashDamageRatio = 1f;
    float m_splashRadius = 5f;

    float m_damage;

    public override void AllTimeEffect(Monster monster)
    {                                                                                                            
        m_damage = Damage;
        Apply(false, Damage);
        monster.OnDamage((int)(m_damage * m_directDamageRatio));
        Collider[] colliders = Physics.OverlapSphere(monster.transform.position, m_splashRadius);
        foreach (var item in colliders)
        {
            item.GetComponent<Monster>()?.OnDamage((int)(m_damage * m_splashDamageRatio));
        }
    }
    public override void EverySecondEffect(Monster monster) { }
    public override void ClearEffect(Monster monster) { base.Clear(); }
}