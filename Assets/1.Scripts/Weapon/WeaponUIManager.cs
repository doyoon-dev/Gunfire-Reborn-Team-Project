using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUIManager : MonoBehaviour
{
    GameObject m_grenadeUI;
    public RectTransform m_grenadeImage;
    int count;                                          // 텍스트 오브젝트 개수
    GameObject m_grenadeCntObj;
    UnityEngine.UI.Text[] m_grenadeText;

    [SerializeField]GameObject m_currentGunObj;
    UnityEngine.UI.Image[] m_currentGunImages;

    #region 대쉬 UI
    GameObject m_dashUI;
    GameObject m_dashCoolTime;
    #endregion

    #region 총알 UI
    // 총알 최대 소지 가능 수량
    public int[] m_maxBullets = { 450, 120, 30 };
    [SerializeField]
    GameObject m_bulletImage;

    GameObject m_green;
    GameObject m_blue;
    GameObject m_yellow;
    // 총알 게이지
    UnityEngine.UI.Image m_greenBulletImage;
    UnityEngine.UI.Image m_greenBulletInsideImage;

    UnityEngine.UI.Image m_blueBulletImage;
    UnityEngine.UI.Image m_blueBulletInsideImage;

    UnityEngine.UI.Image m_yellowBulletImage;
    UnityEngine.UI.Image m_yellowBulletInsideImage;

    // 총알 UI 기본 크기
    float originScale = 80f;
    float changeScale = 110f;
    float originInsideScale = 85f;
    float changeInsideScale = 115f;

    // 현재 소지중인 총알 수
    int m_greenBullet;
    int m_blueBullet;
    int m_yellowBullet;

    UnityEngine.UI.Text m_defaultBulletText;
    UnityEngine.UI.Text m_greenBulletText;
    UnityEngine.UI.Text m_blueBulletText;
    UnityEngine.UI.Text m_yellowBulletText;
    #endregion

    [SerializeField] GameObject[] m_slotUI;

    UnityEngine.UI.Image[] m_childslotUI1;
    UnityEngine.UI.Image[] m_childslotUI2;
    UnityEngine.UI.Image[] m_childslotUI3;

    public int MaxBulletValue(Define.DropItemType type) => m_maxBullets[(type - Define.DropItemType.Bullet_Green)];

    void Awake()
    {
        m_childslotUI1 = m_slotUI[0].GetComponentsInChildren<UnityEngine.UI.Image>();
        m_childslotUI2 = m_slotUI[1].GetComponentsInChildren<UnityEngine.UI.Image>();
        m_childslotUI3 = m_slotUI[2].GetComponentsInChildren<UnityEngine.UI.Image>();

        m_currentGunImages = m_currentGunObj.GetComponentsInChildren<UnityEngine.UI.Image>();

        for (int i = 0; i < m_currentGunImages.Length; i++)
        {
            m_currentGunImages[i].enabled = false;
            if (m_currentGunImages[i].name == "Gun_image")
                m_currentGunImages[i].enabled = true;
        }

        // 대쉬
        m_dashUI = transform.Find("DashUI").gameObject;
        m_dashCoolTime = m_dashUI.transform.GetChild(3).gameObject;

        // 총알 이미지
        m_green = m_bulletImage.transform.Find("Green").gameObject;
        m_blue = m_bulletImage.transform.Find("Blue").gameObject;
        m_yellow = m_bulletImage.transform.Find("Yellow").gameObject;

        m_defaultBulletText = m_bulletImage.transform.Find("DefaultBulletText").gameObject.GetComponent<UnityEngine.UI.Text>();

        // 총알 내부 이미지, 텍스트
        m_greenBulletImage = m_green.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        m_greenBulletInsideImage = m_greenBulletImage.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        m_greenBulletText = m_green.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>();

        m_blueBulletImage = m_blue.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        m_blueBulletInsideImage = m_blueBulletImage.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        m_blueBulletText = m_blue.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>();

        m_yellowBulletImage = m_yellow.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        m_yellowBulletInsideImage = m_yellowBulletImage.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
        m_yellowBulletText = m_yellow.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>();

        // 폭탄 상위 오브젝트
        m_grenadeUI = transform.Find("GrenadeUI").gameObject;
        m_grenadeImage = m_grenadeUI.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        m_grenadeCntObj = m_grenadeUI.transform.GetChild(1).gameObject;
        m_grenadeText = m_grenadeCntObj.GetComponentsInChildren<UnityEngine.UI.Text>();

        InitWeaponUI();
    }

    void Update()
    {
        SetBullet();
        FillBulletUI();
        GrenadeCount();
    }

    void InitWeaponUI()
    {
        m_slotUI[0].SetActive(false);
        m_slotUI[1].SetActive(false);
        m_slotUI[2].SetActive(true);

        m_defaultBulletText.gameObject.SetActive(true);
        m_greenBulletText.gameObject.SetActive(false);
        m_blueBulletText.gameObject.SetActive(false);
        m_yellowBulletText.gameObject.SetActive(false);
        m_defaultBulletText.text = "9 / ∞";
    }

    void SetBullet()
    {
        m_greenBullet = WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Green];
        m_blueBullet = WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Blue];
        m_yellowBullet = WeaponManager.m_current.m_bulletDic[Define.DropItemType.Bullet_Yellow];
    }

    // 현재 총의 총알 Text 보여주는 함수
    public void ShowCurBulletCnt(Gun.BulletType bulletType, int bulletCnt)
    {
        if (bulletType == Gun.BulletType.Bullet_Default)
        {
            ChangeScaleBulletInside(originInsideScale, originInsideScale, originInsideScale);
            ChangeScaleBulletUI(originScale, originScale, originScale);
            m_defaultBulletText.text = bulletCnt + " / ∞";
        }

        else if (bulletType == Gun.BulletType.Bullet_Green)
        {
            ChangeScaleBulletUI(changeScale, originScale, originScale);
            ChangeScaleBulletInside(changeInsideScale, originInsideScale, originInsideScale);
            m_greenBulletText.text = bulletCnt + " / " + m_greenBullet;
        }
        else if (bulletType == Gun.BulletType.Bullet_Blue)
        {
            ChangeScaleBulletInside(originInsideScale, changeScale, originInsideScale);
            ChangeScaleBulletUI(originScale, changeScale, originScale);
            m_blueBulletText.text = bulletCnt + " / " + m_blueBullet;
        }
        else if (bulletType == Gun.BulletType.Bullet_Yellow)
        {
            ChangeScaleBulletInside(originInsideScale, originInsideScale, changeScale);
            ChangeScaleBulletUI(originScale, originScale, changeScale);
            m_yellowBulletText.text = bulletCnt + " / " + m_yellowBullet;
        }
    }
    void SetBulletType(bool defaultBullet, bool greenBullet, bool blueBullet, bool yellowBullet)
    {
        m_defaultBulletText.gameObject.SetActive(defaultBullet);
        m_greenBulletText.gameObject.SetActive(greenBullet);
        m_blueBulletText.gameObject.SetActive(blueBullet);
        m_yellowBulletText.gameObject.SetActive(yellowBullet);
    }

    // 총알 변경
    public void ChangeCurBullet(Gun.BulletType bulletType)
    {
        if (bulletType == Gun.BulletType.Bullet_Default)
        {
            SetBulletType(true, false, false, false);
        }

        else if (bulletType == Gun.BulletType.Bullet_Green)
        {
            SetBulletType(false, true, false, false);
        }
        else if (bulletType == Gun.BulletType.Bullet_Blue)
        {
            SetBulletType(false, false, true, false);
        }
        else if (bulletType == Gun.BulletType.Bullet_Yellow)
        {
            SetBulletType(false, false, false, true);
        }
        else
        {
            SetBulletType(true, false, false, false);
        }
    }

    void SlotAlpha(float alpha1, float alpha2, float alpha3)
    {
        for (int i = 0; i < m_childslotUI1.Length; i++)
        {
            m_childslotUI1[i].color = new Color(1, 1, 1, alpha1);
            m_childslotUI2[i].color = new Color(1, 1, 1, alpha2);
            m_childslotUI3[i].color = new Color(1, 1, 1, alpha3);
        }
    }

    // 무기를 먹었을 때 슬롯 생성(1, 2번 슬롯 UI)
    // 무기 슬롯 다 찼을 때 현재 들고있는 무기 슬롯 이외의 슬롯 흐리게
    public void SetWeaponSlot(int WeaponSlotNum, bool isWeaponFull)
    {
        // 현재 들고있는 무기 슬롯 1번째
        if (WeaponSlotNum == 0)
        {
            // 무기 슬롯이 기본 슬롯(3번 슬롯) 밖에 없을 때
            // 1번 슬롯 생성하고 3번 슬롯 흐리게
            if (!isWeaponFull)
            {
                m_slotUI[0].SetActive(true);
                SlotAlpha(1, 0.5f, 0.5f);
            }
            // 무기 슬롯 다 찼을 때
            else
            {
                SlotAlpha(1, 0.5f, 0.5f);
            }
        }

        // 현재 들고있는 무기 슬롯 2번째
        else if (WeaponSlotNum == 1)
        {
            // 무기 슬롯이 기본 슬롯(3번 슬롯) + 1개 더 있을 때
            if (!isWeaponFull)
            {
                m_slotUI[1].SetActive(true);
                SlotAlpha(0.5f, 1, 0.5f);
            }
            else
            {
                SlotAlpha(0.5f, 1, 0.5f);
            }
        }
    }

    // 무기 슬롯 흐리게
    public void ChangeWeaponSlot(int slotNum, int slotCount)
    {
        if (slotCount > 1 && slotCount < 3)
        {
            if (slotNum == 0)
            {
                SlotAlpha(1, 0.5f, 0.5f);
            }

            else if (slotNum == 2)
            {
                SlotAlpha(0.5f, 0.5f, 1);
            }
        }
        else if (slotCount > 2)
        {
            if (slotNum == 0)
            {
                SlotAlpha(1, 0.5f, 0.5f);
            }
            else if (slotNum == 1)
            {
                SlotAlpha(0.5f, 1, 0.5f);
            }
            else if (slotNum == 2)
            {
                SlotAlpha(0.5f, 0.5f, 1);
            }
        }

    }

    // 현재 무기 이미지 보여주는 함수
    public void ShowWeaponImage(SpriteRenderer currentGunImage)
    {
        for (int i = 0; i < m_currentGunImages.Length; i++)
        {
            if (m_currentGunImages[i].name == currentGunImage.name)
            {
                m_currentGunImages[i].enabled = true;
                m_currentGunImages[i].sprite = currentGunImage.sprite;
            }
            else
            {
                m_currentGunImages[i].enabled = false;
            }
        }
    }

    void ChangeScaleBulletUI(float greenScale, float blueScale, float yellowScale)
    {
        m_greenBulletImage.rectTransform.sizeDelta = new Vector2(greenScale, greenScale);
        m_blueBulletImage.rectTransform.sizeDelta = new Vector2(blueScale, blueScale);
        m_yellowBulletImage.rectTransform.sizeDelta = new Vector2(yellowScale, yellowScale);
    }

    void ChangeScaleBulletInside(float greenScale, float blueScale, float yellowScale)
    {
        m_greenBulletInsideImage.rectTransform.sizeDelta = new Vector2(greenScale, greenScale);
        m_blueBulletInsideImage.rectTransform.sizeDelta = new Vector2(blueScale, blueScale);
        m_yellowBulletInsideImage.rectTransform.sizeDelta = new Vector2(yellowScale, yellowScale);
    }

    // 총알 게이지
    void FillBulletUI()
    {
        m_greenBulletInsideImage.fillAmount = (float)m_greenBullet / m_maxBullets[0];
        m_blueBulletInsideImage.fillAmount = (float)m_blueBullet / m_maxBullets[1];
        m_yellowBulletInsideImage.fillAmount = (float)m_yellowBullet / m_maxBullets[2];
    }

    // 현재 폭탄 개수
    void GrenadeCount()
    {
        for (int i = 0; i < m_grenadeText.Length; i++)
        {
            m_grenadeText[i].text = WeaponManager.m_current.m_curGrenade.ToString();
        }

    }
}
