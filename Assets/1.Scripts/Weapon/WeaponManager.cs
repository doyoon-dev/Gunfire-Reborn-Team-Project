using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager m_current = null;

    public int m_currentGunCount = 1;       // 현재 가지고있는 총 개수

    // 총 쏘는 중인지 확인
    public bool m_isFire = false;

    // 무기 UI, 정보 변수
    public WeaponUI m_weaponUI;

    [SerializeField]
    public WeaponInfo m_weaponInfo;


    // 무기 중복 교체 실행 방지
    // false 일 때만 무기 교체
    public bool m_isChangeWeapn = false;

    // 무기 교체 딜레이
    [SerializeField]
    float m_changeWeaponDelayTime;
    // 무기 교체가 완전히 끝난 시점
    [SerializeField]
    float m_changeWeaponEndDelayTime;


    // 폭탄 최대 소지 가능 수량
    public int m_maxGrenade = 3;
    // 현재 폭탄 소지 수량
    public int m_curGrenade = 3;

    [SerializeField]
    GameObject m_prefabDamagedFloor;
    [SerializeField]
    GameObject m_prefabGrenade;             // 폭탄 프리펩

    GameObject m_obj;                       // 폭탄 생성시 프리펩 담을 오브젝트
    public List<Gun> m_gunList = new List<Gun>();
    

    [SerializeField]
    string m_currentWeaponType;             // 현재 무기 타입

    public Gun m_currentWeapon;       // 현재 무기

    [SerializeField]
    public GunController m_gunCon;

    int m_firstSlot = 0;
    int m_secondSlot = 1;
    int m_basicGunIndex = 2;
    [SerializeField]
    Camera m_cam;
    [SerializeField]
    GameObject m_handPos;

    void Awake()
    {
        if (m_current == null)
        {
            m_current = this;
        }
        else
        {
            Debug.LogError("Not Single WeaponManager");
            Destroy(gameObject);
        }

        m_weaponUI = Managers.UI.ShowSceneUI<WeaponUI>();
        m_gunList.Add(null);
        m_gunList.Add(null);
        m_gunList.Add(null);
        m_gunList[m_basicGunIndex] = m_currentWeapon.GetComponent<Gun>();
    }

    void Update()
    {
        if (!m_isChangeWeapn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (m_gunList[0] == null) return;
                m_weaponUI.ChangeWeaponSlot(0, CheckWeaponCount());
                if (m_gunList[0] != m_currentWeapon)
                {
                    StartCoroutine(ChangeWeaponCoroutine("GUN", 0));
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (m_gunList[1] == null) return;
                m_weaponUI.ChangeWeaponSlot(1, CheckWeaponCount());
                if (m_gunList[1] != m_currentWeapon)
                {
                    StartCoroutine(ChangeWeaponCoroutine("GUN", 1));
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_weaponUI.ChangeWeaponSlot(m_basicGunIndex, CheckWeaponCount());
                if(m_gunList[m_basicGunIndex]!= m_currentWeapon)
                {
                    StartCoroutine(ChangeWeaponCoroutine("GUN", m_basicGunIndex));
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            FullGrenade();
        }
    }

    public List<Gun> GetPossessGunList()
    {
        List<Gun> guns = new List<Gun>();
        for (int i = 0; i < 2; i++)
        {
            guns.Add(m_gunList[i]); 
        }
        return guns;
    }
    
    public Dictionary<Define.DropItemType, int> m_bulletDic = new Dictionary<Define.DropItemType, int>()
    {
        { Define.DropItemType.Bullet_Green, 200 },
        { Define.DropItemType.Bullet_Blue, 110 },
        { Define.DropItemType.Bullet_Yellow, 15 }
    };

    public bool IsBulletFull(Define.DropItemType type)
    {
        return m_bulletDic[type] >= m_weaponUI.MaxBulletValue(type);
    }


    public int GetBulletItem(Define.DropItemType bulletType)
    {
        m_bulletDic[bulletType] += (int)(m_weaponUI.MaxBulletValue(bulletType)*0.1f);
        if (IsBulletFull(bulletType))
        {
            m_bulletDic[bulletType] = m_weaponUI.MaxBulletValue(bulletType);
        }
        return m_bulletDic[bulletType];
    }

    void WeaponDrawAnimation()
    {
        m_currentWeapon.GetComponent<Gun>().m_anim.Play("Draw");
    }

    // 무기 교체 딜레이
    public IEnumerator ChangeWeaponCoroutine(string type, int index)
    {
        // 무기 집어넣는 중
        m_isChangeWeapn = true;
        WeaponDrawAnimation();

        // 무기 집어넣을 때까지 대기
        yield return new WaitForSeconds(m_changeWeaponDelayTime);

        m_gunCon.CancelReload();
        // 무기 꺼내는 중
        WeaponChange(type, index);

        // 무기를 꺼내는 시간동안 대기
        yield return new WaitForSeconds(m_changeWeaponEndDelayTime);

        m_currentWeaponType = type;          // 바꾼 무기의 타입 변경
        m_isChangeWeapn = false;
    }

    // 무기 교체
    void WeaponChange(string type, int index)
    {
        if (type == "GUN")
        {
            m_gunCon.GunChange(m_gunList[index]);
        }
    }

    // 현재 들고있는 무기 개수 체크
    // 캡슐에 사용중
    public int CheckWeaponCount()
    {
        int count = 0;
        count = transform.childCount;
        if (count == 3)
        {
            m_currentGunCount = 3;
        }
        return count;
    }

    // 드랍 된 무기 획득했을 때
    int GetDropWeapon(GameObject hitObj)
    {
        for (int i = 0; i < 3; i++)
        {
            m_currentWeapon.transform.GetChild(i).gameObject.SetActive(false);
        }
        m_currentWeapon.gameObject.GetComponent<Animator>().enabled = false;

        // 드랍무기부모오브젝트.transform.SetParent(gameObject);
        hitObj.transform.SetParent(gameObject.transform);
        // 드랍 무기 위치 zero로 설정해서 플레이어의 자식 오브젝트로 들어감
        hitObj.transform.localPosition = Vector3.zero;
        hitObj.transform.localRotation = Quaternion.identity;

        Gun PickUpGun = hitObj.GetComponent<Gun>();

        if (m_gunList[0] == null) m_gunList[0] = PickUpGun;
        else if (m_gunList[1] == null) m_gunList[1] = PickUpGun;
        else if (m_gunList[m_basicGunIndex] == m_currentWeapon)
        {
            m_gunCon.GunDrop(m_gunList[0]);
            m_gunList[m_gunList.IndexOf(m_gunList[0])] = PickUpGun;
        }
        else
        {
            m_gunCon.GunDrop(m_currentWeapon);
            m_gunList[m_gunList.IndexOf(m_currentWeapon)] = PickUpGun;
        }

        m_currentWeapon = PickUpGun;
        m_currentWeapon.gameObject.GetComponent<Animator>().enabled = true;
        m_currentWeapon.GetComponent<SphereCollider>().enabled = false;

        return m_gunList.IndexOf(PickUpGun);
    }
   
    // 드랍 무기 교체
    public void DropWeaponChange(GameObject hitObj)
    {
        int index = GetDropWeapon(hitObj);
        StartCoroutine(ChangeWeaponCoroutine("GUN", index));
        m_weaponUI.SetWeaponSlot(index, false);
    }

    // 폭탄 생성
    public GameObject CreateGrenade()
    {
        m_obj = Managers.Pool.Pop(m_prefabGrenade);
        m_obj.transform.position = m_handPos.transform.position;

        m_obj.SetActive(true);
        return m_obj;
    }

    // 폭탄 터진 후 데미지 장판 생성
    public void CreateDamagedFloor(Transform grenade)
    {
        GameObject obj = Managers.Pool.Pop(m_prefabDamagedFloor);
        obj.transform.position = grenade.position;

        obj.SetActive(true);
    }

    // 총알 최대로 채워주는 아이템
    public void FullBullet()
    {
        m_bulletDic[Define.DropItemType.Bullet_Green] = m_weaponUI.m_maxBullets[0];
        m_bulletDic[Define.DropItemType.Bullet_Blue] = m_weaponUI.m_maxBullets[1];
        m_bulletDic[Define.DropItemType.Bullet_Yellow] = m_weaponUI.m_maxBullets[2];
    }

    // Test 용 총알 채우는 함수
    public void TenBullet()
    {
        m_bulletDic[Define.DropItemType.Bullet_Green] += 10;
    }

    public void PickUPGrenade(int value)
    {
        m_curGrenade = (m_curGrenade + value > m_maxGrenade) ? m_maxGrenade : m_curGrenade + value;
    }

    // 폭탄 최대로 채워주는 아이템
    public void FullGrenade()
    {
        m_curGrenade = m_maxGrenade;
    }

    // 무기 강화시 공격력 상승
    public void EnforceWeapon(int enforce, int slot)
    {
        if (m_gunList[0] == null && m_gunList[1] == null)
            return;

        else if (m_gunList[0] != null && m_gunList[1] == null)
        {
            float firstWeaponDmg = m_gunList[0].m_damage;

            // 첫 번째 슬롯 무기 강화
            if (slot == 0)
            {
                m_gunList[0].m_gunEnforce = enforce;
                firstWeaponDmg += firstWeaponDmg * ((15 * (float)enforce) / 100);
                m_gunList[0].m_enforceDamage = Mathf.CeilToInt(firstWeaponDmg);
            }
            else
            {
                return;
            }
        }
        else if (m_gunList[0] != null && m_gunList[1] != null)
        {
            float firstWeaponDmg = m_gunList[0].m_damage;
            float secondWeaponDmg = m_gunList[1].m_damage;

            if (slot == 0)
            {
                m_gunList[0].m_gunEnforce = enforce;
                firstWeaponDmg += firstWeaponDmg * ((15 * (float)enforce) / 100);
                m_gunList[0].m_enforceDamage = Mathf.CeilToInt(firstWeaponDmg);
            }
            // 두 번째 슬롯 무기 강화
            else if (slot == 1)
            {
                m_gunList[1].m_gunEnforce = enforce;
                secondWeaponDmg += secondWeaponDmg * ((15 * (float)enforce) / 100);
                m_gunList[1].m_enforceDamage = Mathf.CeilToInt(secondWeaponDmg);
            }
            else
            {
                return;
            }
            
        }

        if (m_gunList[0].m_gunEnforce > 0 || m_gunList[1].m_gunEnforce > 0)
        {
            NormalWeaponEnforce();
        }
    }

    // 무기 바꿀 때, 강화할 때 호출
    public void NormalWeaponEnforce()
    {
        float thirdWeaponDmg = m_gunList[2].m_damage;

        // 두 무기 강화 비교해서 강화 작은 무기 강화 수 만큼 기본 무기 강화
        if (m_gunList[0] != null && m_gunList[1] == null)
        {
            thirdWeaponDmg += thirdWeaponDmg * ((15 * (float)m_gunList[0].m_gunEnforce) / 100);
            m_gunList[2].m_enforceDamage = Mathf.CeilToInt(thirdWeaponDmg);
        }
        else if (m_gunList[0] != null && m_gunList[1] != null)
        {
            if (m_gunList[0].m_gunEnforce > m_gunList[1].m_gunEnforce)
            {
                thirdWeaponDmg += thirdWeaponDmg * ((15 * (float)m_gunList[1].m_gunEnforce) / 100);
                m_gunList[2].m_enforceDamage = Mathf.CeilToInt(thirdWeaponDmg);
            }
            else
            {
                thirdWeaponDmg += thirdWeaponDmg * ((15 * (float)m_gunList[0].m_gunEnforce) / 100);
                m_gunList[2].m_enforceDamage = Mathf.CeilToInt(thirdWeaponDmg);
            }
        }
    }
}
