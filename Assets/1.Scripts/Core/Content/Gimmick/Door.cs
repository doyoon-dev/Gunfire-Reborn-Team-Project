public class Door : Gimmick
{
    public override void WorkGimmick()
    {
        bool flag = gameObject.activeSelf == true ? false : true;
        gameObject.SetActive(flag);
    }
}
