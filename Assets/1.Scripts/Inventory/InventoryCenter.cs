using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCenter : MonoBehaviour
{
    GameObject m_weaponCenter;
    GameObject[] m_slots;
    int m_slotCount;

    GameObject[] m_firstSlotChildObj;
    GameObject[] m_secondSlotChildObj;
    GameObject[] m_thirdSlotChildObj;
    int m_slotChildCount;

    WeaponInfo[] m_weaponInfo;

    GameObject[] m_bulletBg;
    int m_bulletBgCount;

    UnityEngine.UI.Text m_greenBulletText;
    UnityEngine.UI.Text m_blueBulletText;
    UnityEngine.UI.Text m_yellowBulletText;

    UnityEngine.UI.Image m_greenBulletImage;
    UnityEngine.UI.Image m_blueBulletImage;
    UnityEngine.UI.Image m_yellowBulletImage;

    // Start is called before the first frame update
    void Start()
    {
        // 총 인벤토리 슬롯
        m_weaponCenter = transform.GetChild(0).gameObject;
        m_slotCount = m_weaponCenter.transform.childCount;
        m_slots = new GameObject[m_slotCount];

        for (int i = 0; i < m_slotCount; i++)
        {
            m_slots[i] = m_weaponCenter.transform.GetChild(i).gameObject;
        }

        m_slotChildCount = m_slots[0].transform.childCount;
       
        m_firstSlotChildObj = new GameObject[m_slotChildCount];
        m_secondSlotChildObj = new GameObject[m_slotChildCount];
        m_thirdSlotChildObj = new GameObject[m_slotChildCount];

        for (int i = 0; i < m_slotChildCount; i++)
        {
            m_firstSlotChildObj[i] = m_slots[0].transform.GetChild(i).gameObject;
            m_secondSlotChildObj[i] = m_slots[1].transform.GetChild(i).gameObject;
            m_thirdSlotChildObj[i] = m_slots[2].transform.GetChild(i).gameObject;
        }
        m_weaponInfo = new WeaponInfo[3];
        m_weaponInfo[0] = m_firstSlotChildObj[1].GetComponent<WeaponInfo>();
        m_weaponInfo[1] = m_secondSlotChildObj[1].GetComponent<WeaponInfo>();
        m_weaponInfo[2] = m_thirdSlotChildObj[1].GetComponent<WeaponInfo>();


        // 총알 인벤토리
        m_bulletBgCount = m_slots[3].transform.GetChild(0).GetChild(0).childCount;
        m_bulletBg = new GameObject[m_bulletBgCount];
        for (int i = 0; i < m_bulletBgCount; i++)
        {
            m_bulletBg[i] = m_slots[3].transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
        }
        m_greenBulletText = m_bulletBg[1].transform.GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        m_blueBulletText = m_bulletBg[2].transform.GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Text>();
        m_yellowBulletText = m_bulletBg[3].transform.GetChild(3).GetChild(0).GetComponent<UnityEngine.UI.Text>();

        m_greenBulletImage = m_bulletBg[1].transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        m_blueBulletImage = m_bulletBg[2].transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        m_yellowBulletImage = m_bulletBg[3].transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        InitInventorySetting();
        InventoryUI.m_current.m_inventoryBg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitInventorySetting()
    {
        for (int i = 1; i < 3; i++)
        {
            m_firstSlotChildObj[i].SetActive(false);
            m_secondSlotChildObj[i].SetActive(false);
        }
        for (int i = 0; i < 3; i++)
        {
            m_weaponInfo[i].ArrowSetOff();
        }
    }


    // 무기 하나 일 때 / 2개 일 때 구분
    public void ShowWeaponSlot()
    {
        if (WeaponManager.m_current.CheckWeaponCount() == 2 && WeaponManager.m_current.m_currentGunCount < 3)
        {
            for (int i = 1; i < 3; i++)
            {
                m_firstSlotChildObj[i].SetActive(true);
            }
            WeaponUseUI(true, false, false);
            m_weaponInfo[0].ShowGunText(WeaponManager.m_current.m_gunList[0]);
            m_weaponInfo[2].ShowGunText(WeaponManager.m_current.m_gunList[2]);
            if (WeaponManager.m_current.m_currentWeapon == WeaponManager.m_current.m_gunList[0])
            {
                WeaponUseUI(true, false, false);
            }
            // 기본 무기 사용중 일 때
            else
            {
                WeaponUseUI(false, false, true);
            }
        }
        else if (WeaponManager.m_current.CheckWeaponCount() == 3 && WeaponManager.m_current.m_currentGunCount == 3)
        {
            for (int i = 1; i < 3; i++)
            {
                m_firstSlotChildObj[i].SetActive(true);
                m_secondSlotChildObj[i].SetActive(true);
            }
            m_weaponInfo[0].ShowGunText(WeaponManager.m_current.m_gunList[0]);
            m_weaponInfo[1].ShowGunText(WeaponManager.m_current.m_gunList[1]);
            m_weaponInfo[2].ShowGunText(WeaponManager.m_current.m_gunList[2]);
            // 1번 슬롯 무기 사용중 일 때
            if (WeaponManager.m_current.m_currentWeapon == WeaponManager.m_current.m_gunList[0])
            {
                WeaponUseUI(true, false, false);
            }
            // 2번 슬롯 무기 사용중 일 때
            else if (WeaponManager.m_current.m_currentWeapon == WeaponManager.m_current.m_gunList[1])
            {
                WeaponUseUI(false, true, false);
            }
            // 기본 무기 사용중 일 때
            else
            {
                WeaponUseUI(false, false, true);
            }
        }
        else
        {
            m_weaponInfo[2].ShowGunText(WeaponManager.m_current.m_gunList[2]);
            WeaponUseUI(false, false, true);
        }
    }

    void WeaponUseUI(bool firstActive, bool secondActive, bool thirdActive)
    {
        m_firstSlotChildObj[3].SetActive(firstActive);
        m_secondSlotChildObj[3].SetActive(secondActive);
        m_thirdSlotChildObj[3].SetActive(thirdActive);
    }

    public void ShowBulletCount()
    {
        m_greenBulletText.text = WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Green] + " / 450";
        m_blueBulletText.text = WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Blue] + " / 120";
        m_yellowBulletText.text = WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Yellow] + " / 30";
        m_greenBulletImage.fillAmount = (float)WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Green] / 450f;
        m_blueBulletImage.fillAmount = (float)WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Blue] / 120f;
        m_yellowBulletImage.fillAmount = (float)WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Yellow] / 30f;
    }
}
