using Define;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameResult : UI_Popup
{
    enum Texts
    {
        PlayTimeText,
        MaxDamageText,
        KillCountText,
    }
    enum Images
    {
        TextImg,
    }
    enum Buttons
    {
        ExitButton
    }


    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickEixtButton);

        SetGameResult();
    }

    private void SetGameResult()
    {
        Managers.Game.IsPopUpUI = true;

        GetImage((int)Images.TextImg).sprite = Managers.Game.Player.IsDie ?
            Managers.Resource.Load<Sprite>(Path.Arttext_Fail) : 
            Managers.Resource.Load<Sprite>(Path.Arttext_Win);

        TimeSpan timeSpan = TimeSpan.FromSeconds(Managers.Game.GetGameTime);
        string answer = string.Format("{0:D2}:{1:D2}",
                      timeSpan.Minutes,
                      timeSpan.Seconds);

        GetText((int)Texts.PlayTimeText).text = answer;
        GetText((int)Texts.MaxDamageText).text="";
        GetText((int)Texts.KillCountText).text= Managers.Game.Monster.KillCount.ToString();
    }
    public void OnClickEixtButton(PointerEventData data)
    {
        Managers.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(Define.Scene.StartScene.ToString());
        //SceneLoader.Instance.LoadScene(Define.Scene.StartScene.ToString());
    }
}
