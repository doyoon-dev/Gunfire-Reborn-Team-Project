using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWeaponSlot : MonoBehaviour
{
    public UnityEngine.UI.Text m_name;
    public UnityEngine.UI.Text m_bulletTypeText;
    public UnityEngine.UI.Text m_dmgNum;
    public UnityEngine.UI.Text m_criNum;
    public UnityEngine.UI.Text m_fireRateNum;
    public UnityEngine.UI.Text m_carryBulletNum;
    public UnityEngine.UI.Image m_weaponImage;

    public UnityEngine.UI.Text m_use;

    int m_weaponSlotCount;
    GameObject[] m_weaponSlot;

    int m_bgCount;
    GameObject[] m_bg;

    Gun m_currentGun;
    // Start is called before the first frame update
    void Start()
    {
        m_weaponSlotCount = transform.childCount;
        m_weaponSlot = new GameObject[m_weaponSlotCount];
        for (int i = 0; i < m_weaponSlotCount; i++)
        {
            // m_weaponSlot[0] : Bg
            // m_weaponSlot[1] : SlotName
            // m_weaponSlot[2] : Use
            m_weaponSlot[i] = transform.GetChild(i).gameObject;
        }
        m_bgCount = m_weaponSlot[0].transform.childCount;
        m_bg = new GameObject[m_bgCount];
        for (int i = 0; i < m_bgCount; i++)
        {
            // m_weaponSlot[0] : Decoration
            // m_weaponSlot[1] : WeaponImage
            // m_weaponSlot[2] : WeaponStat
            // m_weaponSlot[3] : Name
            m_bg[i] = m_weaponSlot[0].transform.GetChild(i).gameObject;
        }
        m_currentGun = WeaponManager.m_current.m_currentWeapon.GetComponent<Gun>();
        InitSlotSetting(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowCurrentWeapon();
        }
        
    }
    void InitSlotSetting(bool active)
    {
        for (int i = 0; i < m_bg.Length; i++)
        {
            m_bg[i].SetActive(active);
        }
    }

    // 인벤토리에서 현재 총 정보 보여주는 함수
    void ShowCurrentWeapon()
    {
        
        InitSlotSetting(true);
        WeaponManager.m_current.m_weaponInfo.ShowGunText(WeaponManager.m_current.m_gunList[0]);
        if (WeaponManager.m_current.m_currentWeapon.name == WeaponManager.m_current.m_gunList[0].name)
        {
            m_use.gameObject.SetActive(true);
        }
        else
        {
            m_use.gameObject.SetActive(false);
        }
    }
}
