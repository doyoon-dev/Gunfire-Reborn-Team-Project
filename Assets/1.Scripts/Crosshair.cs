using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    UnityEngine.UI.Image[] m_crosshairs;
    Animator m_anim;


    // Start is called before the first frame update
    void Start()
    {
        m_crosshairs = transform.GetComponentsInChildren<UnityEngine.UI.Image>();
        m_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootAnim()
    {
        m_anim.SetTrigger("Shoot");
    }

    public void HitEnemy()
    {
        for (int i = 0; i < 4; i++)
        {
            m_crosshairs[i].color = new Color(1, 0, 0, 1);
        }
    }
    public void OriginCrosshair()
    {
        for (int i = 0; i < 4; i++)
        {
            m_crosshairs[i].color = new Color(1, 1, 1, 1);
        }
    }
}
