using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHpUI : UI_Scene
{
    public GameObject m_playerHpBg;

    public GameObject m_hpBg;
    public GameObject m_shieldBg;

    public UnityEngine.UI.Image m_playerHpBar;
    public UnityEngine.UI.Text m_playerHpText;

    public UnityEngine.UI.Image m_playerShieldBar;
    public UnityEngine.UI.Text m_playerShieldText;


    public float m_playerMaxHp = 80;
    public float m_currentPlayerHp = 80;

    public float m_playerMaxShield = 50;
    public float m_currentPlayerShield = 50;

    void Start()
    {
        m_playerHpBg = transform.GetChild(0).gameObject;

        m_hpBg = m_playerHpBg.transform.Find("HpBar_bg").gameObject;
        m_shieldBg = m_playerHpBg.transform.Find("Shield_bg").gameObject;

        m_playerHpBar = m_hpBg.transform.Find("HpBar").gameObject.GetComponent<UnityEngine.UI.Image>();
        m_playerHpText = m_hpBg.transform.Find("HpText").gameObject.GetComponent<UnityEngine.UI.Text>();

        m_playerShieldBar = m_shieldBg.transform.Find("ShieldBar").gameObject.GetComponent<UnityEngine.UI.Image>();
        m_playerShieldText = m_shieldBg.transform.Find("ShieldText").gameObject.GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CurrentHp()
    {
        m_playerShieldBar.fillAmount = m_currentPlayerShield / m_playerMaxShield;
        m_playerShieldText.text = (int)m_currentPlayerShield + " / " + m_playerMaxShield;
        m_playerHpBar.fillAmount = m_currentPlayerHp / m_playerMaxHp;
        m_playerHpText.text = (int)m_currentPlayerHp + " / " + m_playerMaxHp;
    }

    public void ShowPlayerStatus(bool isShield, int damage)
    {
        if (isShield)
        {
            m_currentPlayerShield -= damage;
            m_playerShieldBar.fillAmount = m_currentPlayerShield / m_playerMaxShield;
            m_playerShieldText.text = (int)m_currentPlayerShield + " / " + m_playerMaxShield;
        }
        else
        {
            m_currentPlayerHp -= damage;
            m_playerHpBar.fillAmount = m_currentPlayerHp / m_playerMaxHp;
            m_playerHpText.text = (int)m_currentPlayerHp + " / " + m_playerMaxHp;
        }
    }
}
