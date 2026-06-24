using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UI_Smith : UI_Popup
{
    Smith m_smith;
    [SerializeField] UI_SmithButton[] m_upgradeButtons;
    [SerializeField] WeaponInfo[] m_infos;
    List<Gun> m_gunList;

    [SerializeField] UI_NumberString m_goldString;
    [SerializeField] Text m_remainUpgradeCount;
    [SerializeField] Image m_noUpgradeChance;

    public bool IsPossibleUpgrade(int index)
    {
        return m_smith.CurUpgradeChance > 0 && Managers.Game.Gold >= m_smith.GetUpgradePrice(m_gunList[index]);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI(this);
            Managers.Game.IsPopUpUI = false;
        }
    }

    public void Initialize(Smith craftsman)
    {
        m_smith = craftsman;
        m_gunList = WeaponManager.m_current.GetPossessGunList();
        for (int i = 0; i < 2; i++)
        {
            if (m_gunList[i] == null)
            {
                m_infos[i].gameObject.SetActive(false);
                m_upgradeButtons[i].gameObject.SetActive(false);
            }
            else
            {
                m_infos[i].ShowGunText(m_gunList[i]);
                m_infos[i].InfoWindowOnOff(true);
                m_infos[i].ArrowSetOff();
                m_infos[i].gameObject.SetActive(true);
                m_upgradeButtons[i].gameObject.SetActive(true);
                m_upgradeButtons[i].Init(this);
            }
        }
        OnUpdate();

    }

    public void OnUpdate()
    {
        m_remainUpgradeCount.text = $"잔여 강화 횟수 :{m_smith.CurUpgradeChance}";
        if (m_smith.CurUpgradeChance <= 0) m_remainUpgradeCount.color = Color.red;
        else m_remainUpgradeCount.color = Color.white;

        int count = m_gunList.Count;
        for (int i = 0; i < count; i++)
        {
            if (m_gunList[i] == null)
            {
                m_infos[i].gameObject.SetActive(false);
                m_upgradeButtons[i].gameObject.SetActive(false);
            }
            else
            {
                m_upgradeButtons[i].UpdateState(m_smith.CurUpgradeChance,
                    m_smith.GetUpgradePrice(m_gunList[i]));
                m_infos[i].ShowGunText(m_gunList[i]);
                m_infos[i].gameObject.SetActive(true);
                m_upgradeButtons[i].gameObject.SetActive(true);
            }
        }
        m_goldString.ChangeNumber(Managers.Game.Gold);
    }

    public void ShowGunInfo(int index, bool upgrad)
    {
        if(upgrad) m_infos[index].ShowEnforceGunText(m_gunList[index]);
        else m_infos[index].ShowGunText(m_gunList[index]);
    }

    public void TryUpgradeGun(int index)
    {
        if (!IsPossibleUpgrade(index))return;

        Managers.Game.Gold-= m_smith.GetUpgradePrice(m_gunList[index]);
        WeaponManager.m_current.EnforceWeapon(
            m_gunList[index].m_gunEnforce+1, index);
        m_smith.CurUpgradeChance--;
        Initialize(m_smith);
    }
}