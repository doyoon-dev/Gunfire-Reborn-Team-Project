using UnityEngine;

public class MonsterRanged : Monster
{
    //원거리공격 몬스터
    Ray m_ShootRay; //총쏘는 위치
    RaycastHit m_ShootHit; //맞은애
    int m_groundMask; //레이어마스크 장애물 저장
    int m_playerMask; //레이어마스크 플레이어 저장
    int m_wallMask;
    int m_enemybulletMask; //레이어마스크 몬스터총알 저장
    
    [SerializeField]
    GameObject BulletPrefab;

    Vector3 m_castStartPos;
    Vector3 m_castDir;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_isDead = false;
        m_traceDist = 300f;//추적사정거리_원거리//CheckState()에서 사용
        m_battleDist = 50f;//전투사정거리_원거리//첫공격은 공격시작거리에서 실행, 이후 전투사정거리 내에서는 계속 공격함
        m_attackDist = 40f;//공격시작거리_원거리//CheckState()에서 사용
        m_arrangeDist = 2f;//정비거리
        m_maxShotDelay = 1.5f;//공격딜레이_원거리//Attack()에서 사용
        m_bulletSpeed = 30f;//총알속도//Attack()에서 사용
        m_groundMask = LayerMask.GetMask("Ground");
        m_playerMask = LayerMask.GetMask("Player");
        m_wallMask = LayerMask.GetMask("Wall");
        m_enemybulletMask = LayerMask.GetMask("EnemyBullet");
        //
        m_status = new Status(600, 20, 0, 0, 0);
        //
        m_monSpeed = 20f; //이동속도_원거리(유니티 기본설정은 3.5)
        m_isBoss = false;   //보스인지 아닌지
        m_monsterName = new string("석 궁 병");
        m_isRangedMonster = true;        
    }
    protected override void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.green;//추적거리
        Gizmos.DrawWireSphere(transform.position, m_traceDist);

        Gizmos.color = Color.gray;//전투거리
        Gizmos.DrawWireSphere(transform.position, m_battleDist);

        Gizmos.color = Color.magenta;//공격거리
        Gizmos.DrawWireSphere(transform.position, m_attackDist);

        Gizmos.color = Color.cyan;//정비거리(너무가까울때)
        Gizmos.DrawWireSphere(transform.position, m_arrangeDist);

        Vector3 castStartPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
        Vector3 castDir = new Vector3(m_playerTransform.position.x, m_playerTransform.position.y + 1f, m_playerTransform.position.z) - castStartPos;
        m_castStartPos = castStartPos;
        m_castDir = castDir;

        //bool isHit = Physics.BoxCast(transform.position, transform.lossyScale / 10, transform.forward, out m_ShootHit, transform.rotation, 20f);
        bool isHit = Physics.BoxCast(castStartPos, transform.lossyScale / 2f, castDir, out m_ShootHit, transform.rotation, m_attackDist, m_playerMask);
        Gizmos.color = Color.black;//원거리공격 레이
        if (isHit)
        {
            //Gizmos.DrawRay(transform.GetChild(0).position, transform.GetChild(0).forward * m_ShootHit.distance);
            //Gizmos.DrawWireCube(transform.GetChild(0).position + transform.GetChild(0).forward * m_ShootHit.distance, transform.lossyScale);
            Gizmos.DrawRay(castStartPos, castDir.normalized * m_ShootHit.distance);
            Gizmos.DrawWireCube(castStartPos + castDir.normalized * m_ShootHit.distance, transform.lossyScale / 2f);
        }
        else
        {
            //Gizmos.DrawRay(transform.GetChild(0).position, transform.GetChild(0).forward * 20f);
            Gizmos.DrawRay(castStartPos, transform.forward * m_attackDist);
        }
    }
    protected override void Attack()
    {
        if (m_isDead != true)
        {
            if (m_curState == eCurrentState.eBattle || m_curState == eCurrentState.eAttack)//현재상태가 전투 또는 공격중일때 총알발사
            {
                m_dist = m_ShootHit.distance;
                m_ShootRay.origin = m_castStartPos;
                m_ShootRay.direction = m_castDir.normalized;                
                transform.LookAt(m_playerTransform);

                if (Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_dist, m_groundMask)
                    || Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_dist, m_wallMask))
                {
                    if(m_nvAgent.enabled == true)
                    {
                        m_nvAgent.destination = m_playerTransform.position;
                    }                    
                }                
                else if (Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_attackDist, m_playerMask))                
                {
                    m_curShotDelay += Time.deltaTime;
                    if (m_curShotDelay >= m_maxShotDelay)
                    {
                        GameObject obj = Managers.Pool.Pop(BulletPrefab);
                        obj.GetComponent<MonsterBullet>().InitMosnter(this);

                        //obj.transform.position = transform.GetChild(0).position;//GetChild:총알발사 위치(FirePos)
                        obj.transform.position = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);//GetChild:총알발사 위치(FirePos)

                        Vector3 pos = new Vector3(m_playerTransform.position.x, m_playerTransform.position.y + 1f, m_playerTransform.position.z) - obj.transform.position;

                        obj.GetComponent<Rigidbody>().velocity = pos.normalized * m_bulletSpeed;

                        //obj.transform.rotation = transform.GetChild(0).rotation;
                        obj.transform.rotation = transform.rotation;
                        obj.SetActive(true);

                        m_curShotDelay = 0; 
                        m_anim.Play("Shoot", 0, 0f);
                        m_anim.Play("Shoot");
                    }
                }
                else if (Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_dist, m_enemybulletMask))
                    return;
                else
                {                    
                    if (m_nvAgent.enabled == true)
                    {
                        m_nvAgent.destination = m_playerTransform.position;
                    }                                
                }
            }
        }
    }
    protected override void AttackOtherMonsterAction(Monster m_closestMonster)
    {
        if (m_closestMonster != null)
        {
            m_curShotDelay += Time.deltaTime;

            if (m_curShotDelay >= m_maxShotDelay)
            {
                GameObject obj = Managers.Pool.Pop(BulletPrefab);
                obj.GetComponent<MonsterBullet>().InitMosnter(this);

                //obj.transform.position = transform.GetChild(0).position;//GetChild:총알발사 위치(FirePos)
                obj.transform.position = new Vector3(transform.position.x, transform.position.y + m_height / 2, transform.position.z);//GetChild:총알발사 위치(FirePos)

                Vector3 pos = m_closestMonster.transform.position - obj.transform.position;

                obj.GetComponent<Rigidbody>().velocity = pos.normalized * m_bulletSpeed;

                //obj.transform.rotation = transform.GetChild(0).rotation;
                obj.transform.rotation = transform.rotation;
                obj.SetActive(true);

                m_curShotDelay = 0;
            }
        }
    }
}