using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    ParticleSystem m_bulletSmoke;
    //public GameObject m_test;
    static Vector3 m_hitPoint;

    static int m_bulletDmg = 0;

    static Define.ElementalType m_gunElementalType;
    static int m_gunDmg;
    static int m_elementalProbability;

    static Gun m_currentGun;

    GameObject m_testObj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        Invoke("Die", 2f);
    }

    private void OnDisable()
    {
        CancelInvoke("Die");
    }
    void Die()
    {
        Managers.Pool.Push(gameObject);
    }

    // 안씀
    //public static void SetBulletSmoke(Vector3 hitPoint)
    //{
    //    m_hitPoint = hitPoint;
    //}
    public static void SetGunType(Gun type)
    {
        m_currentGun = type;
        //m_bulletDmg = DamageCalculator.TotalDamage(currentGunDmg, currentGunCir, currentGunLuckyShot, type);
        //m_bulletDmg = DamageCalculator.ElementalDamage(m_bulletDmg, element, true);
    }
    public static void GunType(Gun gun)
    {
        m_gunElementalType = gun.m_gunElemental;
        m_gunDmg = gun.m_enforceDamage;
        m_elementalProbability = gun.m_elementalProbability;
    }
    private void OnTriggerEnter(Collider other)
    {
        string layerName = LayerMask.LayerToName(other.gameObject.layer);

        if (layerName == "Monster")
        {
            Monster mon = other.gameObject.GetComponent<Monster>();
            //몬스터 헤드샷 기능 추가_박기환
            if (mon == null)
                mon = other.gameObject.GetComponentInParent<Monster>();

            if (mon != null)
            {
                m_bulletDmg = DamageCalculator.ElementalDamage(m_currentGun, mon, "Normal");

                if (other.tag == "Head")
                {
                    mon.OnDamage(m_bulletDmg * 2);
                    //Debug.LogError("Hit!!!!");
                }
                else
                    mon.OnDamage(m_bulletDmg);
                //몬스터헤드샷

                //mon.PushElemental(new ElementalStat(Define.ElementalType.Fire, 100), 100);//총 데미지 넣어야함
                mon.PushElemental(new ElementalStat(m_gunElementalType, m_elementalProbability), m_gunDmg);
            }
            Managers.Pool.Push(gameObject);
        }

        if (layerName == "Ground" || layerName == "Wall")
        {
            if (m_bulletSmoke != null)
            {
                GameObject particleObj = Managers.Pool.Pop(m_bulletSmoke.gameObject); // Managers.Pool.Pop(m_bulletSmoke.gameObject);
                Vector3 dir = GetComponent<Rigidbody>().velocity;
                dir.Normalize();
                particleObj.transform.forward = dir;
                particleObj.SetActive(true);
                particleObj.transform.position = m_hitPoint;


                
                particleObj.transform.position = other.ClosestPointOnBounds(transform.position) - (dir * 3f);
                //Destroy(particleObj, 2f);
            }
            
        }


    }
}
