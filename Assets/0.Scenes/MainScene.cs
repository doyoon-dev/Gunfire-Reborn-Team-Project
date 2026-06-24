public class MainScene : BaseScene
{
    protected override void LoadGameObject()
    {
        Managers.Pool.CreatePool(Managers.Resource.Load<Monster>(Define.Path.Monster_bossmodel).gameObject);
        Managers.Pool.CreatePool(Managers.Resource.Load<Monster>(Define.Path.Monster_Monster_Melee_Beatle).gameObject);
        Managers.Pool.CreatePool(Managers.Resource.Load<Monster>(Define.Path.Monster_Monster_Melee_Spear).gameObject);
        Managers.Pool.CreatePool(Managers.Resource.Load<Monster>(Define.Path.Monster_Monster_Ranged_Crossbow).gameObject);
    }

    protected override void Init()
    {
        Managers.Sound.InitAudioMixer();
        Managers.Sound.Play(Define.Path.SOUND_BGM_Boss1, true, Define.Sound.BGM);
        Managers.Game.StartGame();
    }


}
