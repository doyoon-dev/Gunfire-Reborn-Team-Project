public class UI_Scene : UI_Base
{
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, false);
    }

    // 몬스터 hp바 없앨떄 사용
    public void CloseSceneUI()
    {
        Managers.UI.CloseSceneUI(this);
    }
}
