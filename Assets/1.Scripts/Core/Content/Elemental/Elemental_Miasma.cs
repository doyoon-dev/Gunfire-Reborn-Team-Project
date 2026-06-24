public class Elemental_Miasma : Elemental
{
    const float m_generalRatio = 0.09f;
    const float m_eliteRatio = 0.0075f;
    const float m_bossRatio = 0.003f;

    int m_curStack;
    int m_maxStack = 9;
    public override void AllTimeEffect(Monster monster) { }
    public override void EverySecondEffect(Monster monster)
    {
        float damage = monster.m_status.m_hp * ++m_curStack;
        monster.OnDamage((int)damage);
    }
    public override void ClearEffect(Monster monster) { base.Clear(); }
}