using UnityEngine;

public class MonsterMeleeBeatle : Monster
{
    //근거리 공격 몬스터
    private float m_attackRangeDegreeGizmo = 50f;
    private float m_attackDistGizmo = 6f;

    int m_groundMask; //레이어마스크 장애물 저장
    int m_playerMask; //레이어마스크 플레이어 저장

    private bool m_hasSetDestination = false; //정신지배상태시 다른 적 쫓아갈 때 한번만 쫓아가게

    private AudioSource m_audioSource;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_isDead = false;
        m_traceDist = 300f;//추적사정거리_근거리_딱정벌레
        m_battleDist = 10f;//전투거리_근거리_딱정벌레
        m_attackDist = 5f;//공격사정거리_근거리_딱정벌레
        m_arrangeDist = 0f;//정비거리_근거리는0
        m_maxShotDelay = 2f;//공격딜레이_근거리_딱정벌레
        m_status = new Status(400, 10, 0, 0, 0);
        
        m_playerMask = LayerMask.GetMask("Player");
        m_curState = eCurrentState.eIdle;

        m_monSpeed = 20f; //이동속도_근거리_딱정벌레(유니티 기본설정은 3.5)
        m_isBoss = false;   //보스인지 아닌지
        m_hasSetDestination = false;
        m_isMeleeMonster = true;
        m_monsterName = new string("딱 정 벌 레");
        m_audioSource = GetComponent<AudioSource>();
    }
    //근접공격범위표시 계산부분
    Vector3[] CalculateSightPoint(float radius, float angle)
    {
        Vector3[] results = new Vector3[2];

        // 우측 끝 점의 좌표를 구한다.
        float theta = 90 - angle - transform.eulerAngles.y;
        float posX = Mathf.Cos(theta * Mathf.Deg2Rad) * radius;
        float posY = transform.position.y;
        float posZ = Mathf.Sin(theta * Mathf.Deg2Rad) * radius;
        results[0] = new Vector3(posX, posY, posZ);

        // 좌측 끝 점의 좌표를 구한다.
        theta = 90 + angle - transform.eulerAngles.y;
        posX = Mathf.Cos(theta * Mathf.Deg2Rad) * radius;
        posY = transform.position.y;
        posZ = Mathf.Sin(theta * Mathf.Deg2Rad) * radius;
        results[1] = new Vector3(posX, posY, posZ);

        return results;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;//근접공격범위표시
        Vector3[] sightPos = CalculateSightPoint(m_attackDistGizmo, m_attackRangeDegreeGizmo);
        for (int i = 0; i < sightPos.Length; i++)
        {
            Vector3 endPos = transform.position + sightPos[i].normalized * m_attackDistGizmo;
            endPos.y = transform.position.y;
            Gizmos.DrawLine(transform.position, endPos);
        }
        Gizmos.color = Color.green;//추적거리
        Gizmos.DrawWireSphere(transform.position, m_traceDist);

        Gizmos.color = Color.gray;//전투거리
        Gizmos.DrawWireSphere(transform.position, m_battleDist);

        Gizmos.color = Color.magenta;//공격거리
        Gizmos.DrawWireSphere(transform.position, m_attackDist);        
    }
    //bool cooldown=false;
    protected override void Attack()
    {
        if (m_isDead != true)
        {
            if (m_curState == eCurrentState.eAttack)//현재상태가 공격상태일때
            {                
                m_anim.SetBool("onMeleeAttack", true);
                if (m_onMeleeAttackCoolDown != true)
                {
                    transform.LookAt(m_playerTransform);
                    m_anim.SetTrigger("attackTrigger");
                    m_anim.Play("Shoot", 0, 0);
                    m_onMeleeAttackCoolDown = true;
                }
                m_curShotDelay += Time.deltaTime;
                
                if (m_curShotDelay < m_maxShotDelay)
                {
                    return;
                }
                m_curShotDelay = 0;                
            }
        }
    }
    void MeleeAttack()
    {        
        Collider[] targets = Physics.OverlapSphere(this.transform.position, m_attackDist + 1f, m_playerMask);

        for (int i = 0; i < targets.Length; ++i)
        {
            Transform target = targets[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 HitDir = transform.position - target.transform.position;

            if (Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos(m_attackRangeDegreeGizmo * Mathf.Deg2Rad))
            {
                if (target.CompareTag("Player"))
                {
                    PlayerController player = target.GetComponent<PlayerController>();
                    if (player != null)
                    {                        
                        player.Damaged(DamageCalculator.TrueDamageCal(m_status.m_atk));
                        Managers.Resource.Instantiate<UI_MonsterAtkDir>(Define.Path.UI_MonsterAtkDir).Init(HitDir);
                    }
                }
            }
        }
    }
    
    void MeleeAttackCoolDown()
    {
        if (m_curState != eCurrentState.eAttack)
            m_anim.SetBool("onMeleeAttack", false);
        m_onMeleeAttackCoolDown = false;
    }
    
    protected override void AttackOtherMonsterAction(Monster m_closestMonster)
    {        
        if (m_closestMonster != null)
        {
            Transform target = m_closestMonster.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;            

            transform.LookAt(m_closestMonster.transform);
            m_nvAgent.enabled = true;
            if (!m_hasSetDestination)
            {
                m_nvAgent.destination = m_closestMonster.transform.position;
                m_hasSetDestination = true;
            }

            if (Vector3.Distance(target.transform.position, transform.position) <= m_attackDist)
            {
                m_nvAgent.enabled = false;
            }

            m_curShotDelay += Time.deltaTime;
            if (m_curShotDelay < m_maxShotDelay)
                return;
            m_curShotDelay = 0;
            
            if (Vector3.Dot(transform.forward, dirToTarget) > Mathf.Cos(m_attackRangeDegreeGizmo * Mathf.Deg2Rad))
            {
                Debug.Log("MELEE ATTACK OTHER MONSTER : " + m_status.m_atk);
                m_closestMonster.OnDamage(DamageCalculator.TrueDamageCal(m_status.m_atk));
            }
        }
    }
    void PlayAttackSound()
    {
        m_audioSource.Play();
    }
}
