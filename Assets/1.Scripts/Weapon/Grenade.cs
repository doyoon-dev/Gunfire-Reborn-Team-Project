using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (layerName == "Ground")
        {
            Managers.Sound.Play(Define.Path.SOUND_Effect_Grenade, false, Define.Sound.Effect);
            WeaponManager.m_current.CreateDamagedFloor(gameObject.transform);
            Managers.Pool.Push(gameObject);
        }
    }
}
