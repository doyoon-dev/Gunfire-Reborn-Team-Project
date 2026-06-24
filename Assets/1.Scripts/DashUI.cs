using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashUI : MonoBehaviour
{
    UnityEngine.UI.Image m_dashImg;
    GameObject m_dashCoolTime;
    RectTransform[] m_dashCool;

    public UnityEngine.UI.Image m_coolBg;
    public UnityEngine.UI.Text m_coolTimeText;

    public float m_coolTime = 3;
    public float m_currentCoolTime = 3;

    void Start()
    {
        m_dashImg = transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
        m_dashCoolTime = transform.GetChild(3).gameObject;
        m_dashCool = m_dashCoolTime.GetComponentsInChildren<RectTransform>();
        m_coolBg = m_dashCool[1].GetComponent<UnityEngine.UI.Image>();
        m_coolTimeText = m_dashCool[2].GetComponent<UnityEngine.UI.Text>();

        m_coolBg.gameObject.SetActive(false);
        m_coolTimeText.gameObject.SetActive(false);
    }

    public IEnumerator Cooltime()
    {
        Managers.Game.Player.m_isDash = false;
        m_currentCoolTime = WeaponManager.m_current.m_weaponUI.m_dashUI.m_coolTime + 1;
        m_coolBg.gameObject.SetActive(true);
        m_coolTimeText.gameObject.SetActive(true);
        m_dashImg.color = new Color32(255, 255, 255, 80);
        while (m_coolBg.fillAmount < 1)
        {
            m_coolBg.fillAmount += 1 * Time.smoothDeltaTime / m_coolTime;
            m_currentCoolTime -= Time.deltaTime;

            m_coolTimeText.text = "" + (int)m_currentCoolTime;

            yield return new WaitForEndOfFrame();
        }
        m_dashImg.color = new Color32(255, 255, 255, 255);
        
        m_coolBg.gameObject.SetActive(false);
        m_coolTimeText.gameObject.SetActive(false);
        Managers.Game.Player.m_isDash = true;
        yield break;
    }

}
