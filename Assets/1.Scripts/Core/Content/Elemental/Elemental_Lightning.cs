public class Elemental_Lightning : Elemental
{
    public override void AllTimeEffect(Monster monster) { }
    public override void EverySecondEffect(Monster monster) { }
    public override void ClearEffect(Monster monster) { base.Clear(); }
}