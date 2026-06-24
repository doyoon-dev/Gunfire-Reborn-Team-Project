public class Elemental_Corrosion : Elemental
{
    public override void AllTimeEffect(Monster monster) { monster.ApplyDeBuff(true); }
    public override void EverySecondEffect(Monster monster) {}
    public override void ClearEffect(Monster monster)
    {
        base.Clear();
        //monster.ApplyDeBuff(Define.ElementalType.Corrosion, false);
        monster.ApplyDeBuff(false);
    }
}