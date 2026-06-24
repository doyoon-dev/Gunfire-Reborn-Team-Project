public class Elemental_Fire : Elemental
{
    float m_damageRatio = 0.2f;
    public override void AllTimeEffect(Monster monster) { }
    public override void EverySecondEffect(Monster monster)
    {
        monster.OnDamage((int)(Damage * m_damageRatio));
    }
    public override void ClearEffect(Monster monster) { base.Clear(); }
}