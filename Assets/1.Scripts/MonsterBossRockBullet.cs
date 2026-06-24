using UnityEngine;

public class MonsterBossRockBullet : MonoBehaviour
{      
    float m_lifeTime = 2f;
    Monster m_mon;
    bool m_hasAttacked = false; //총알 한발 발사하면 공격 한번만 가능하도록 추가
    public float m_rockAttackRange = 20f;//보스 바위던지기 폭발범위
    float m_rockAttackDamage = 30f;//보스 바위던지기 공격력
    
    int m_playerMask;

    void OnDrawGizmos()
    {        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_rockAttackRange);
    }
    public void InitMosnter(Monster mon)
    {
        m_mon = mon;
    }
    
    public void OnEnable()
    {
        m_playerMask = LayerMask.GetMask("Player");
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
    //public void OnCollisionEnter(Collision collision)
    public void OnTriggerEnter()
    {        
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_rockAttackRange, m_playerMask);
        foreach (Collider collider in colliders)
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Damaged(DamageCalculator.TrueDamageCal(m_rockAttackDamage));
            }
        }
    }
}
