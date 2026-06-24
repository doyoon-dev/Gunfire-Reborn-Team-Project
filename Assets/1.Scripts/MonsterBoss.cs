using UnityEngine;
using System.Collections;

public class MonsterBoss : Monster
{
    //보스 몬스터
    Ray m_ShootRay; //총쏘는 위치
    RaycastHit m_ShootHit; //맞은애
    int m_groundMask; //레이어마스크 장애물 저장
    int m_playerMask; //레이어마스크 플레이어 저장
    int m_enemybulletMask; //레이어마스크 플레이어 저장
    float m_rockSpeed = 50f;//보스 바위던지기 투사체 속도
    //float m_rockAttackRange = 10f;//보스 바위던지기 폭발범위
    //float m_rockAttackDamage = 30f;//보스 바위던지기 공격력
        
    bool isOnRecovery = false;
    int isOnPhase = 0;
    bool isRageMode = false;
    private eCurrentStateBossPattern m_curStateBossPattern;

    float m_slamAttackRange = 10f;
    float m_slamAttackDamage = 50f;
    Vector3 m_slamAttackTarget;
    Vector3 m_knockbackDirection;

    int m_attackType;

    GameObject recoveryZone;

    float m_curShotDelayRock;
    float m_maxShotDelayRock = 1.5f;

    float m_curShotDelaySlam;
    float m_maxShotDelaySlam = 2f;

    [SerializeField]
    GameObject BulletPrefab;

    [SerializeField]
    GameObject BossRockPrefab;

    [SerializeField]
    GameObject RockAttackRangeViewPrefab;

    [SerializeField]
    GameObject normalFirePos;

    [SerializeField]
    GameObject throwRockPos;

    GameObject MonsterSpawner;

    public float range;

    private bool isOnAction;

    GameObject rageEffect;

    private AudioSource m_audioSource;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_isDead = false;
        m_traceDist = 300f;//추적사정거리_보스//CheckState()에서 사용
        m_battleDist = 35f;//전투사정거리_보스
        m_attackDist = 20f;//공격거리_보스//CheckState()에서 사용
        m_arrangeDist = 0f;//정비거리_보스
        m_maxShotDelay = 1f;//공격딜레이(평타)_보스//Attack()에서 사용
        m_bulletSpeed = 40f;//총알속도(평타)_보스//Attack()에서 사용
        m_groundMask = LayerMask.GetMask("Ground");
        m_playerMask = LayerMask.GetMask("Player");
        m_enemybulletMask = LayerMask.GetMask("EnemyBullet");
        //
        m_status = new Status(8000, 50, 10000, 0, 0);
        //
        m_monSpeed = 60f; //이동속도_보스(유니티 기본설정은 3.5)
        m_isBoss = true;   //보스인지 아닌지
        m_monsterName = new string("육 오");
        isOnPattern = false;
        recoveryZone = GameObject.Find("BossRecoveryZone");
        MonsterSpawner = GameObject.Find("Spawner_Boss");
        //if (m_isBoss)
        //{
        //    bossRageEffect = transform.Find("LargeFlames").gameObject;
        //}
        m_audioSource = GetComponent<AudioSource>();
    }
    Vector3 m_castStartPos;
    Vector3 m_castDir;
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
        //Vector3 castDir = castStartPos - m_playerTransform.position;
        bool isHit = Physics.BoxCast(castStartPos, transform.lossyScale / 2f, castDir, out m_ShootHit, transform.rotation, 20f, m_playerMask);
        Gizmos.color = Color.black;//원거리공격 레이
        if (isHit)
        {
            Gizmos.DrawRay(castStartPos, castDir.normalized * m_ShootHit.distance);
            Gizmos.DrawWireCube(castStartPos + castDir.normalized * m_ShootHit.distance, transform.lossyScale / 2f);
        }
        else
        {
            Gizmos.DrawRay(castStartPos, transform.forward * 20f);
        }
    }
    protected enum eCurrentStateBossPattern
    {
        eShieldRecovery
    }
    protected override void CheckState()
    {
        //DieCheat();
        if (isOnPhase == 0 && m_status.m_hp != m_maxHpBoss)//패턴 시작, 1차 실드 회복
        {            
            isOnPattern = true;
        }
        else if(isOnPhase == 1 && m_status.m_def == m_maxShieldBoss)//패턴 종료, 전투 시작 
        {
            ++isOnPhase;
            isOnPattern = false;
        }
        else if(isOnPhase == 2 && m_status.m_def <= m_maxShieldBoss * 0.5f)//패턴 시작, 2차 실드 회복
        {
            isOnPattern = true;
        }
        else if(isOnPhase == 3 && m_status.m_def == m_maxShieldBoss)//패턴 종료, 전투시작(분노모드)
        {
            isOnPattern = false;
            if (bossRageEffect != null)
            {
                // 이펙트 오브젝트를 활성화하여 재생
                bossRageEffect.SetActive(true);                
            }
            //m_anim.ResetTrigger("isOnRunTrigger");
            //m_anim.SetTrigger("onPatternEsc");
            isRageMode = true;
        }

        if (m_isDead != true)
        {
            if (isOnPattern != true)
            {
                float dist = Vector3.Distance(m_playerTransform.position, transform.position);
                
                if (dist <= m_attackDist)
                {
                    if (m_nvAgent.enabled == true)
                        m_nvAgent.SetDestination(this.transform.position);
                    m_curState = eCurrentState.eAttack;
                    hasSetnvAgent = false;
                }
                else if (dist <= m_battleDist)
                {
                    if (hasSetnvAgent != true)
                    {
                        if (m_prevState == eCurrentState.eTrace)
                        {
                            return;
                        }
                        else
                        {
                            m_curState = eCurrentState.eBattle;
                        }
                        hasSetnvAgent = true;
                    }
                }
                else if (dist <= m_traceDist)
                {
                    m_curState = eCurrentState.eTrace;
                }
                else
                {
                    m_curState = eCurrentState.eIdle;
                }
                m_prevState = m_curState;
            }
            else if(isOnPattern == true)
            {
                if (isOnPhase == 1 || isOnPhase == 2)
                    m_curStateBossPattern = eCurrentStateBossPattern.eShieldRecovery;
            }
        }        
    }
    void ActionStart()
    {
        isOnAction = true;
        if(m_nvAgent.enabled==true)
        {
            m_nvAgent.destination = transform.position;
        }
    }
    void ActionEnd()
    {
        isOnAction = false;
        if (m_nvAgent.enabled == true)
        {
            m_nvAgent.destination = m_playerTransform.transform.position;
        }
    }
    protected override void CheckStateForAction()
    {
        if (m_isDead != true)
        {
            if (isOnPattern != true)
            {
                if (isOnAction)
                    return;

                switch (m_curState)
                {
                    case eCurrentState.eIdle:
                        m_nvAgent.enabled = false;
                        if (m_anim != null)
                        {
                            m_anim.SetBool("isAttacking", false);
                            m_anim.SetBool("isMoving", false);
                        }
                        break;
                    case eCurrentState.eTrace:
                        m_nvAgent.enabled = true;
                        if (m_nvAgent.enabled == true)
                            m_nvAgent.destination = m_playerTransform.position;

                        if (m_anim != null)
                        {
                            m_anim.SetBool("isMoving", true);
                            m_anim.SetBool("isAttacking", false);                            
                        }
                        break;
                    case eCurrentState.eBattle:
                        if (m_prevState == eCurrentState.eTrace)
                            m_nvAgent.enabled = true;                        
                        Battle();                        
                        break;
                    case eCurrentState.eAttack:
                        m_anim.SetBool("isMoving", false);
                        Attack();
                        break;
                }
            }
            else
                switch(m_curStateBossPattern)
                {
                    case eCurrentStateBossPattern.eShieldRecovery:
                        ShieldRecovery();
                        break;
                }
        }
    }
    protected override void Battle()
    {
        Attack();
        m_reTraceTime += Time.deltaTime;        
        if (m_reTraceTime >= 1f)
        {            
            m_nvAgent.enabled = true;
            if (m_nvAgent.enabled == true)
            {
                m_nvAgent.destination = m_playerTransform.position;
                m_anim.SetBool("isMoving", true);
            }
            m_reTraceTime = 0;
        }
    }
    protected override void Attack()
    {
        if (m_isDead != true)
        {
            if (m_curState == eCurrentState.eBattle || m_curState == eCurrentState.eAttack)//현재상태가 전투 또는 공격중일때 총알발사
            {
                m_curShotDelay += Time.deltaTime;
                m_curShotDelayRock += Time.deltaTime;
                m_curShotDelaySlam += Time.deltaTime;

                m_dist = m_ShootHit.distance;
                m_ShootRay.origin = m_castStartPos;
                m_ShootRay.direction = m_castDir.normalized;
                transform.LookAt(m_playerTransform);

                if (Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_dist, m_groundMask))
                {
                    //레이경로 내 장애물 있을시 행동
                    if(m_nvAgent.enabled == true)
                    {
                        m_nvAgent.destination = m_playerTransform.position;
                    }                    
                }
                else if (Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_dist + 1f, m_playerMask))
                {
                    //레이경로 내 플레이어 찾았을때 행동(공격)
                    NormalAttack();
                }
                else if (Physics.BoxCast(m_ShootRay.origin, transform.lossyScale / 2f, m_ShootRay.direction, Quaternion.identity, m_dist, m_enemybulletMask))
                    //레이경로 내 몬스터가 발사한 총알이 있을 때
                    return;
                //else
                //{
                //    m_nvAgent.enabled = true;
                //    m_nvAgent.destination = m_playerTransform.position;
                //}
            }
        }
    }
    
    private void NormalAttack()
    {
        //m_curShotDelay += Time.deltaTime;
        //m_curShotDelayRock += Time.deltaTime;
        //m_curShotDelaySlam += Time.deltaTime;
        if (isRageMode != true)
        {
            m_attackType = Random.Range(1, 3);
        }
        else
        {
            m_attackType = Random.Range(1, 4);
        }        
        switch (m_attackType)
        {
            case 1:                
                NormalRangeAttack();                
                break;
            case 2:                
                ThrowRockAttack();                
                break;
            case 3://case 3은 분노모드일때 추가패턴
                SlamAttack();
                break;
        }        
    }
    private void NormalRangeAttack()
    {
        if (m_curShotDelay >= m_maxShotDelay)
        {
            m_curShotDelay = 0;
            m_anim.ResetTrigger("throwRockTrigger");
            m_anim.ResetTrigger("slamTrigger");

            m_anim.SetTrigger("normalAttackTrigger");
            m_anim.SetTrigger("normalShotTrigger");

            if (isRageMode == true)
                m_anim.SetTrigger("RageShotTrigger");
        }
        else
        {
            return;
        }
    }

    public void Shot()
    {
        GameObject obj = Managers.Pool.Pop(BulletPrefab);
        obj.GetComponent<MonsterBullet>().InitMosnter(this);

        obj.transform.position = normalFirePos.transform.position;

        Vector3 pos = new Vector3(m_playerTransform.position.x, m_playerTransform.position.y + 1f, m_playerTransform.position.z) - obj.transform.position;

        obj.GetComponent<Rigidbody>().velocity = pos.normalized * m_bulletSpeed;
        obj.transform.rotation = normalFirePos.transform.rotation;
        obj.SetActive(true);
    }

    private void ThrowRockAttack()
    {
        if (m_curShotDelayRock >= m_maxShotDelayRock)
        {
            m_curShotDelayRock = 0;            
            m_anim.ResetTrigger("slamTrigger");
            m_anim.ResetTrigger("normalAttackTrigger");
            m_anim.SetTrigger("throwRockTrigger");
        }
        else
        {
            return;
        }
    }
    void Throw()
    {
        transform.LookAt(m_playerTransform);
        GameObject obj = Managers.Pool.Pop(BossRockPrefab);
        obj.GetComponent<MonsterBossRockBullet>().InitMosnter(this);
        obj.transform.position = new Vector3(throwRockPos.transform.position.x, throwRockPos.transform.position.y + 5f, throwRockPos.transform.position.z);
        Vector3 playerPos = new Vector3(m_playerTransform.position.x, m_playerTransform.position.y, m_playerTransform.position.z);

        RaycastHit hit;
        if (Physics.Raycast(playerPos, Vector3.down, out hit, Mathf.Infinity, m_groundMask))
        {
            Vector3 targetPos = hit.point;
            Vector3 throwDirection = targetPos - obj.transform.position;
            obj.GetComponent<Rigidbody>().velocity = throwDirection.normalized * m_rockSpeed;

            GameObject rangeview = Managers.Pool.Pop(RockAttackRangeViewPrefab);
            rangeview.transform.position = new Vector3(targetPos.x, targetPos.y + 0.1f, targetPos.z);

            float rangeviewSize = 10f * 2f;//지름의 2배
            float ratio = rangeviewSize / rangeview.GetComponent<SpriteRenderer>().bounds.size.x; // 크기 비율 계산
            rangeview.transform.localScale = new Vector3(ratio, ratio, 1f);
        }
    }
    
    private void ShieldRecovery()
    {        
        m_curState = eCurrentState.eIdle;
        if (isOnRecovery == false)
        {
            m_nvAgent.destination = transform.position;
            SummonMonster();
        }        
        if(transform.position.x != recoveryZone.transform.position.x && transform.position.z != recoveryZone.transform.position.z)
        {
            m_anim.ResetTrigger("normalAttackTrigger");
            m_anim.ResetTrigger("throwRockTrigger");
            m_anim.SetTrigger("isOnRunTrigger");
            m_nvAgent.speed = m_monSpeed * 5f;
            m_nvAgent.destination = recoveryZone.transform.position;           
        }
        if(transform.position.x == recoveryZone.transform.position.x && transform.position.z == recoveryZone.transform.position.z)
        {
            m_anim.ResetTrigger("isOnRunTrigger");
            m_anim.SetBool("isMoving", false);
            m_anim.SetTrigger("isOnRecoveryTrigger");
        }

        //실드회복 실행
        if (isOnRecovery == false)
        {
            StartCoroutine("ShieldFillUp");
            ++isOnPhase;
        }
        isOnRecovery = true;
    }
    private void SummonMonster()
    {        
        MonsterSpawner.GetComponent<MonsterSpawner>().StartSpawn();
    }

    IEnumerator ShieldFillUp()
    {        
        yield return new WaitForSeconds(2f);
        while (isOnPattern == true)
        {
            m_status.m_def += m_maxShieldBoss * 0.1f * Time.deltaTime;
            if(m_status.m_def < 100f)
            {
                m_status.m_def = 100f;
            }
            if (m_status.m_def >= m_maxShieldBoss)
            {
                m_status.m_def = m_maxShieldBoss;
                isOnRecovery = false;
                m_nvAgent.speed = m_monSpeed;
                m_anim.ResetTrigger("isOnRecoveryTrigger");
                m_anim.ResetTrigger("isOnRunTrigger");
                m_anim.ResetTrigger("normalShotTrigger");
                isOnAction = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void SlamAttack()
    {        
        if (m_curShotDelaySlam >= m_maxShotDelaySlam)
        {
            m_curShotDelaySlam = 0;
            m_anim.ResetTrigger("throwRockTrigger");
            m_anim.ResetTrigger("normalAttackTrigger");
            m_anim.SetTrigger("slamTrigger");            
        }
        else
        {
            return;
        }
    }
    
    void Slam()
    {        
        Vector3 playerPos = m_playerTransform.position;
        m_knockbackDirection = playerPos - transform.position;
        m_knockbackDirection = new Vector3(m_knockbackDirection.x, 0, m_knockbackDirection.z);
        m_knockbackDirection.Normalize();

        //바닥 범위나오게 하는 부분 수정할것.
        RaycastHit hit;
        if (Physics.Raycast(playerPos, Vector3.down, out hit, Mathf.Infinity, m_groundMask))
        {
            Vector3 targetPos = hit.point;
            m_slamAttackTarget = targetPos;

            GameObject rangeview = Managers.Pool.Pop(RockAttackRangeViewPrefab);
            rangeview.transform.position = new Vector3(targetPos.x, targetPos.y + 0.1f, targetPos.z);

            float rangeviewSize = m_slamAttackRange * 2f;//지름의 2배
            float ratio = rangeviewSize / rangeview.GetComponent<SpriteRenderer>().bounds.size.x; // 크기 비율 계산
            rangeview.transform.localScale = new Vector3(ratio, ratio, 1f);
        }        

        Collider[] colliders = Physics.OverlapSphere(m_slamAttackTarget, m_slamAttackRange, m_playerMask);
        foreach (Collider collider in colliders)
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Damaged(DamageCalculator.TrueDamageCal(m_slamAttackDamage));
                player.KnockBack(m_knockbackDirection);                
            }
        }
    }

    public bool diebutton;

    void DieCheat()
    {
        if(diebutton == true)
        {            
            Die();
        }
    }
    void PlayAttackSound()
    {
        m_audioSource.Play();
    }
}