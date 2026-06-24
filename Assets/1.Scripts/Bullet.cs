using UnityEngine;

public class Bullet : MonoBehaviour
{      
    float m_lifeTime = 2f;
    Monster m_mon;
    bool m_hasAttacked = false; //총알 한발 발사하면 공격 한번만 가능하도록 추가

    public void InitMosnter(Monster mon)
    {
        m_mon = mon;
    }    
    
    public void OnEnable()
    {        
        Invoke("Die", m_lifeTime);        
    }
   
    private void OnDisable()
    {
        m_hasAttacked = false;  //상태초기화
        CancelInvoke("Die");
    }
    public void Die()
    {
        //풀에 담아놓는다.        
        Managers.Pool.Push(gameObject);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if(!m_hasAttacked && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector3 HitDir = transform.position - other.transform.position;
                //MonsterAtkDirManager.current.m_originPos = HitDir;
                
                if(m_mon.m_status.m_atk !=0)
                {
                    player.Damaged(DamageCalculator.TrueDamageCal(m_mon.m_status.m_atk));
                    //MonsterAtkDirManager.current.OriginMonster(HitDir);
                    Managers.Resource.Instantiate<UI_MonsterAtkDir>(Define.Path.UI_MonsterAtkDir).Init(HitDir);
                    m_hasAttacked = true;
                }
            }
        }
        if(!m_hasAttacked && m_mon.m_isManipulation)
        {
            if (other.CompareTag("Monster"))
            {
                Monster monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    Vector3 HitDir = transform.position - other.transform.position;

                    if (m_mon.m_status.m_atk != 0)
                    {
                        monster.OnDamage(DamageCalculator.TrueDamageCal(m_mon.m_status.m_atk));
                        m_hasAttacked = true;
                    }
                }
            }
        }
    }
}
