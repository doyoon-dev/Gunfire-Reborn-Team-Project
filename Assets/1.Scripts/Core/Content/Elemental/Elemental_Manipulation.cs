public class Elemental_Manipulation : Elemental
{
    public override void AllTimeEffect(Monster monster)
    {
        //발동 대상은 일정 시간동안 적을 공격
        //엘리트, 보스는 면역
    }
    public override void EverySecondEffect(Monster monster) { }
    public override void ClearEffect(Monster monster)
    {
        base.Clear();
    }
}