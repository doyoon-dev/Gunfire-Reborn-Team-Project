using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScone : BaseScene
{
    protected override void Init()
    {
        Managers.Sound.Play(Define.Path.SOUND_BGM_MainMenu, true, Define.Sound.BGM);
    }
}
