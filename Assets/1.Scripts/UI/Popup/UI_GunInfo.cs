using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GunInfo : UI_Popup
{
    //GameObject m_bg;

    //#region WeaponStat 변수
    //[SerializeField]
    //GameObject[] m_weaponStat;

    //public UnityEngine.UI.Text m_bulletTypeText;
    //public UnityEngine.UI.Text m_dmgNum;
    //public UnityEngine.UI.Text m_criNum;
    //public UnityEngine.UI.Text m_fireRateNum;
    //public UnityEngine.UI.Text m_carryBulletNum;

    //GameObject m_dmgArrow;
    //GameObject m_criArrow;
    //GameObject m_fireRateArrow;
    //GameObject m_carryBulletArrow;
    //#endregion

    //#region WeaponImage 변수
    //UnityEngine.UI.Image m_weaponImage;
    //UnityEngine.UI.Image[] m_weaponBulletImage;
    //#endregion

    //UnityEngine.UI.Text m_name;

    //void Start()
    //{
    //    m_bg = transform.GetChild(0).gameObject;

    //    m_weaponImage = m_bg.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>();
    //    m_weaponBulletImage = m_weaponImage.transform.GetChild(0).GetChild(1).GetComponentsInChildren<UnityEngine.UI.Image>();

    //    m_dmgArrow = m_weaponStat[0].transform.GetChild(2).gameObject;
    //    m_criArrow = m_weaponStat[1].transform.GetChild(2).gameObject;
    //    m_fireRateArrow = m_weaponStat[2].transform.GetChild(2).gameObject;
    //    m_carryBulletArrow = m_weaponStat[3].transform.GetChild(2).gameObject;

    //    m_dmgNum = m_weaponStat[0].transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
    //    m_criNum = m_weaponStat[1].transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
    //    m_fireRateNum = m_weaponStat[2].transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
    //    m_carryBulletNum = m_weaponStat[3].transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();

    //    m_name = m_bg.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>();
    //    InfoWindowOnOff(false);
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        InfoWindowOnOff(true);
    //        ShowCurrentWeaponInfo();
    //    }
    //    if (Input.GetKeyUp(KeyCode.M))
    //    {
    //        InfoWindowOnOff(false);
    //    }
    //}

    //// 총 전체 정보
    //public void ShowGunInfo(GameObject hit)
    //{
    //    Gun m_gunInfo = hit.transform.GetChild(0).gameObject.GetComponent<Gun>();
    //    CompareWeaponStat(m_gunInfo, WeaponManager.m_current.m_gunCon.m_currentGun);
    //    ShowGunText(m_gunInfo);
    //}

    //void ShowGunText(Gun gunInfo)
    //{
    //    Gun m_gunInfo = gunInfo;
    //    m_name.text = m_gunInfo.m_gunName;
    //    ShowWeaponImage(m_gunInfo);
    //    m_dmgNum.text = m_gunInfo.m_damage.ToString();
    //    m_criNum.text = m_gunInfo.m_critical + "x";
    //    m_fireRateNum.text = CalFireRate(m_gunInfo).ToString();
    //    m_carryBulletNum.text = m_gunInfo.m_maxCarryBulletCount.ToString();
    //}
    //// 100초 당 발사량 계산
    //int CalFireRate(Gun gun)
    //{
    //    int result = 0;
    //    float fireRate = gun.m_fireRate;
    //    fireRate = 100 / fireRate;
    //    result = Mathf.CeilToInt(fireRate);
    //    return result;
    //}
    //// 무기 이미지, 총알 타입 UI 보여주는 함수
    //void ShowWeaponImage(Gun gunInfo)
    //{
    //    Gun m_gunInfo = gunInfo;
    //    m_weaponImage.sprite = gunInfo.m_currentGunImage.sprite;
    //    if (m_gunInfo.m_bulletType == Gun.BulletType.Bullet_Green)
    //    {
    //        BulletTypeActive(m_gunInfo, true);
    //        m_bulletTypeText.text = "일반탄";
    //    }

    //    else if (m_gunInfo.m_bulletType == Gun.BulletType.Bullet_Blue)
    //    {
    //        BulletTypeActive(m_gunInfo, true);
    //        m_bulletTypeText.text = "대형탄";
    //    }

    //    else
    //    {
    //        BulletTypeActive(m_gunInfo, true);
    //        m_bulletTypeText.text = "특수탄";
    //    }
    //}

    //void BulletTypeActive(Gun gunInfo, bool active)
    //{
    //    for (int i = 0; i < m_weaponBulletImage.Length; i++)
    //    {
    //        if (i == (int)gunInfo.m_bulletType - 2)
    //        {
    //            m_weaponBulletImage[i].gameObject.SetActive(active);
    //        }
    //        else
    //        {
    //            m_weaponBulletImage[i].gameObject.SetActive(!active);
    //        }
    //    }
    //}

    //// 현재 무기와 성능 비교
    //void CompareWeaponStat(Gun hitGun, Gun currentGun)
    //{
    //    if (hitGun.m_damage > currentGun.m_damage)
    //    {
    //        ArrowSet(m_dmgArrow, true);
    //    }
    //    else if (hitGun.m_damage < currentGun.m_damage)
    //    {
    //        ArrowSet(m_dmgArrow, false);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            m_dmgArrow.transform.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }

    //    if (hitGun.m_critical > currentGun.m_critical)
    //    {
    //        ArrowSet(m_criArrow, true);
    //    }
    //    else if (hitGun.m_critical < currentGun.m_critical)
    //    {
    //        ArrowSet(m_criArrow, false);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            m_criArrow.transform.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }

    //    if (hitGun.m_fireRate > currentGun.m_fireRate)
    //    {
    //        ArrowSet(m_fireRateArrow, false);
    //    }
    //    else if (hitGun.m_fireRate < currentGun.m_fireRate)
    //    {
    //        ArrowSet(m_fireRateArrow, true);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            m_fireRateArrow.transform.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }

    //    if (hitGun.m_maxCarryBulletCount > currentGun.m_maxCarryBulletCount)
    //    {
    //        ArrowSet(m_carryBulletArrow, true);
    //    }
    //    else if (hitGun.m_fireRate < currentGun.m_fireRate)
    //    {
    //        ArrowSet(m_carryBulletArrow, false);
    //    }
    //    else
    //    {
    //        for (int i = 0; i < 2; i++)
    //        {
    //            m_carryBulletArrow.transform.GetChild(i).gameObject.SetActive(false);
    //        }
    //    }
    //}
    //void ArrowSet(GameObject arrow, bool active)
    //{
    //    arrow.transform.GetChild(0).gameObject.SetActive(active);
    //    arrow.transform.GetChild(1).gameObject.SetActive(!active);
    //}

    //public void InfoWindowOnOff(bool active)
    //{
    //    transform.GetChild(0).gameObject.SetActive(active);
    //}

    //// 현재 총 정보 보여주기
    //public void ShowCurrentWeaponInfo()
    //{
    //    Gun m_gunInfo = WeaponManager.m_current.m_currentWeapon.gameObject.GetComponent<Gun>();
    //    m_weaponImage.sprite = m_gunInfo.m_currentGunImage.sprite;
    //    CompareWeaponStat(m_gunInfo, WeaponManager.m_current.m_gunCon.m_currentGun);
    //    ShowGunText(m_gunInfo);
    //}
}
