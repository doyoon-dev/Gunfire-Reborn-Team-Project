using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun m_currentGun;               // 현재 총

    [SerializeField]
    GameObject m_player;

    [SerializeField]
    Camera m_cam;

    [SerializeField]
    GameObject m_bulletPrefab;

    [SerializeField]
    Transform m_firePos;
    [SerializeField]
    ParticleSystem m_bulletSmoke;

    public Crosshair m_crosshair;

    Vector3 m_originPos;            // 총의 현재 위치

    float m_currentFireRate;        // 연사속도
    public static bool m_isReload = false;


    // Start is called before the first frame update
    void Start()
    {
        m_crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Crosshair>();
        m_originPos = Vector3.zero;
        WeaponManager.m_current.m_currentWeapon = m_currentGun;
        m_currentGun.m_bulletType = Gun.BulletType.Bullet_Default;
        WeaponManager.m_current.m_weaponUI.ShowWeaponImage(m_currentGun.m_currentGunImage);
    }

    // Update is called once per frame
    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();
        if (!m_isReload)
        {
            if (m_currentGun.m_currentBulletCount == 0)
            {
                m_currentGun.m_currentBulletCount = 0;
                WeaponManager.m_current.m_weaponUI.ShowCurBulletCnt(m_currentGun.m_bulletType, m_currentGun.m_currentBulletCount);
                StartCoroutine(Reload(m_currentGun.m_bulletType));
            }
            else
            {
                WeaponManager.m_current.m_weaponUI.ShowCurBulletCnt(m_currentGun.m_bulletType, m_currentGun.m_currentBulletCount);
            }
        }
    }

    // 연사속도 계산(m_currentFireRate 값이 클 수록 연사 속도 느려짐)
    void GunFireRateCalc()
    {
        if (m_currentFireRate > 0)
        {
            m_currentFireRate -= Time.deltaTime;
        }
    }

    // 발사 시도
    void TryFire()
    {
        if (WeaponManager.m_current.m_isChangeWeapn)
        {
            return;
        }
        // 일시정지일 때 총 안쏘기
        if (!Managers.Game.IsPopUpUI)
        {
            if (Input.GetButton("Fire1") && m_currentFireRate <= 0 && !m_isReload)
            {
                WeaponManager.m_current.m_isFire = true;
                Fire();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                WeaponManager.m_current.m_isFire = false;
                m_currentGun.m_anim.SetBool("Fire", false);
            }

        }
    }

    // 발사 전
    public void Fire()
    {
        if (!m_isReload)
        {
            if (m_currentGun.m_currentBulletCount > 0)
            {
                m_currentGun.m_anim.Play("Fire");
                Managers.Sound.Play(Define.Path.SOUND_Effect_Gun, false, Define.Sound.Effect);
                Shoot();
            }
            else
            {
                m_currentGun.m_anim.SetBool("Fire", false);
                StartCoroutine(Reload(m_currentGun.m_bulletType));
            }
        }
    }

    // 발사 후
    void Shoot()
    {
        m_crosshair.ShootAnim();
        m_currentGun.m_currentBulletCount--;
        m_currentFireRate = m_currentGun.m_fireRate;      // 현재 연사속도가 0보다 작아져서 다시 총의 연사속도를 넣어줘서 재계산 한다.
        m_currentGun.m_muzzleFlash.Play();
        Hit();
    }

    
    // 적 피격 함수
    void Hit()
    {
        int groundMask = LayerMask.GetMask("Ground") + LayerMask.GetMask("Wall") + LayerMask.GetMask("Door");
        RaycastHit m_hit;
        if (Physics.Raycast(m_cam.transform.position, m_cam.transform.forward, out m_hit, 1000, LayerMask.GetMask("Monster")))
        {
            Monster mon = m_hit.collider.gameObject.GetComponentInParent<Monster>();
            if (mon != null)
            {
                if (m_hit.collider.CompareTag("Head"))
                {
                    mon.OnDamage(DamageCalculator.ElementalDamage(m_currentGun, mon, "Head"));
                }
                else
                {
                    mon.OnDamage(DamageCalculator.ElementalDamage(m_currentGun, mon, "Normal"));
                }
                mon.PushElemental(new ElementalStat(m_currentGun.m_gunElemental, m_currentGun.m_elementalProbability), m_currentGun.m_damage);
            }
        }
        else if(Physics.Raycast(m_cam.transform.position, m_cam.transform.forward, out m_hit, 1000, groundMask))
        {
            if (m_bulletSmoke != null)
            {
                GameObject particleObj = Managers.Pool.Pop(m_bulletSmoke.gameObject);
                Vector3 dir = particleObj.transform.position - transform.position;
                particleObj.transform.forward = dir;
                particleObj.SetActive(true);
                particleObj.transform.position = m_hit.point;
            }
        }

    }

    // 재장전 시도
    void TryReload()
    {
        // 재장전 중이 아니고, 재장전 개수 보다 현재 총알 개수가 작을 때
        if (Input.GetKeyDown(KeyCode.R) && !m_isReload && m_currentGun.m_currentBulletCount < m_currentGun.m_reloadBulletCount)
        {
            // 장전 애니메이션 추가
            m_currentGun.m_anim.SetBool("Fire", false);
            StartCoroutine(Reload(m_currentGun.m_bulletType));
        }
    }

    // 재장전 중 무기교체 안되게 막는 함수
    public void CancelReload()
    {
        if (m_isReload)
        {
            StopAllCoroutines();
            m_isReload = false;
        }
    }

    
    IEnumerator Reload(Gun.BulletType type)
    {
        Define.DropItemType[] dropBulletType = { Define.DropItemType.Bullet_Green, Define.DropItemType.Bullet_Blue, Define.DropItemType.Bullet_Yellow };
        type = m_currentGun.m_bulletType;
        if ((int)type == (int)Gun.BulletType.Bullet_Default)
        {
            m_isReload = true;
            m_currentGun.m_anim.SetTrigger("Reload");
            yield return new WaitForSeconds(m_currentGun.m_reloadTime);
            m_currentGun.m_currentBulletCount = 9;
            m_isReload = false;
        }
        for (int i = 0; i < dropBulletType.Length; i++)
        {
            if ((int)type == (int)dropBulletType[i])
            {
                
                if (WeaponManager.m_current.m_bulletDic[dropBulletType[i]] > 0)
                {
                    m_isReload = true;
                    m_currentGun.m_anim.SetTrigger("Reload");

                    yield return new WaitForSeconds(m_currentGun.m_reloadTime);

                    int reloadCount = m_currentGun.m_reloadBulletCount - m_currentGun.m_currentBulletCount;
                    if (WeaponManager.m_current.m_bulletDic[dropBulletType[i]] >= reloadCount)
                    {
                        m_currentGun.m_currentBulletCount += reloadCount;       // 전부 재장전
                        WeaponManager.m_current.m_bulletDic[dropBulletType[i]] -= reloadCount;        // 현재 소유 총알 개수 - 재장전 총알 개수
                    }
                    else
                    {
                        m_currentGun.m_currentBulletCount += WeaponManager.m_current.m_bulletDic[dropBulletType[i]];        // 현재 소유한 총알 개수만큼 장전
                        WeaponManager.m_current.m_bulletDic[dropBulletType[i]] = 0;  
                    }


                    m_isReload = false;
                }
                else
                {
                }
            }
        }
        
        
    }

    // 무기 변경
    public void GunChange(Gun gun)
    {
        // 무기를 들고 있을 경우 현재 무기 비활성화
        if (WeaponManager.m_current.m_currentWeapon != null)
        {
            for (int i = 0; i < 3; i++)
            {
                WeaponManager.m_current.m_currentWeapon.transform.GetChild(i).gameObject.SetActive(false);
            }
            m_currentGun.m_anim.enabled = false;
        }


        m_currentGun = gun;     // 현재 들고있는 무기 변경

        WeaponManager.m_current.m_currentWeapon = m_currentGun;

        m_currentGun.transform.GetChild(1).gameObject.SetActive(true);
        m_currentGun.m_anim.enabled = true;

        WeaponManager.m_current.m_weaponUI.ChangeCurBullet(m_currentGun.m_bulletType);
        WeaponManager.m_current.m_weaponUI.ShowWeaponImage(m_currentGun.m_currentGunImage);
        WeaponManager.m_current.NormalWeaponEnforce();
    }

    // 무기 버리기
    public void GunDrop(Gun gun)
    {
        // 변경할 내용 버린 총 위치, 콜라이더, Active 켜기
        m_currentGun = gun;
        m_currentGun.transform.parent = null;
        m_currentGun.transform.position = m_player.transform.position;
        m_currentGun.GetComponent<SphereCollider>().enabled = true;
    }
}
