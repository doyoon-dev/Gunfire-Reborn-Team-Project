using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSound : MonoBehaviour
{
    public void ReloadIn()
    {
        Managers.Sound.Play(Define.Path.SOUND_Effect_ReloadIn, false, Define.Sound.Effect);
    }

    public void ReloadOut()
    {
        Managers.Sound.Play(Define.Path.SOUND_Effect_ReloadOut, false, Define.Sound.Effect);
    }

    public void Reload()
    {
        Managers.Sound.Play(Define.Path.SOUND_Effect_Reload, false, Define.Sound.Effect);
    }
}
