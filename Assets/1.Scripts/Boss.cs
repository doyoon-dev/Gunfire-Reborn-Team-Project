using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Transform m_playerTransform;
    Collider m_collid;
    Ray m_ShootRay; //총쏘는 위치
    RaycastHit m_ShootHit; //맞은애
    [SerializeField]
    GameObject BulletPrefab;
    int m_groundMask; //레이어마스크 장애물 저장
    int m_playerMask; //레이어마스크 플레이어 저장
    int m_enemybulletMask; //레이어마스크 플레이어 저장

    private float m_traceDist; //추적사정거리//CheckState()에서 사용
    float m_battleDist; //전투사정거리//CheckState()에서 사용
    float m_attackDist; ////공격사정거리//CheckState()에서 사용
    float m_arrangeDist; ////정비거리(너무가까울때 거리조정)//CheckState()에서 사용
    bool m_isDead = false;//사망여부
    float m_maxShotDelay;//최대공격딜레이//Attack()에서 사용
    float m_curShotDelay;//딜레이계산//Attack()에서 사용
    float m_bulletSpeed;//총알속도//Attack()에서 사용
    float m_reTraceTime;//전투상태에서 해당 시간 이후 다시 공격하러감
    UnityEngine.AI.NavMeshAgent m_nvAgent;
    eCurrentState m_curState = eCurrentState.eIdle;
    float dist;//레이 검사용
    float m_height;            //오브젝트 높이
    float m_destroyTime = 2f;//오브젝트 삭제시간
    float m_currentTime;    //오브젝트 삭제시간계산용 
    PlayerController m_playerStat; //플레이어의 스탯 불러오기

    public Status m_status; //몬스터 스탯

    public float m_maxhp;
    public float m_maxdef;

    GameObject m_monsterHpBarFolder;
    
    [SerializeField]
    GameObject prfBossBar;

    RectTransform rectTransform;

    enum eCurrentState
    {
        eIdle,
        eTrace,
        eBattle,
        eAttack,
        eArrange
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;//추적거리
        Gizmos.DrawWireSphere(transform.position, m_traceDist);

        Gizmos.color = Color.gray;//전투거리
        Gizmos.DrawWireSphere(transform.position, m_battleDist);

        Gizmos.color = Color.magenta;//공격거리
        Gizmos.DrawWireSphere(transform.position, m_attackDist);

        Gizmos.color = Color.cyan;//정비거리(너무가까울때)
        Gizmos.DrawWireSphere(transform.position, m_arrangeDist);
    }

    void Start()
    {        
        m_playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        m_nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        //m_animator = this.gameObject.GetComponent<Animator>();

        m_status = new Status(m_status.m_hp, m_status.m_atk, m_status.m_def, m_status.m_cri, m_status.m_luckyShot);//몬스터의 능력치 저장함
                                                                                                                   //hp, atk, def, cri, luckyShot
                                                                                                                   //float hp, float atk, float def, float cri, int luckyShot
        m_traceDist = 20f;//추적사정거리_원거리//CheckState()에서 사용
        m_battleDist = 18f;//전투사정거리_원거리
        m_attackDist = 15f;//공격사정거리_원거리//CheckState()에서 사용
        m_arrangeDist = 2f;//정비거리
        m_maxShotDelay = 3f;//공격딜레이_원거리//Attack()에서 사용
        m_bulletSpeed = 7f;//총알속도//Attack()에서 사용
        m_groundMask = LayerMask.GetMask("Ground");
        m_playerMask = LayerMask.GetMask("Player");
        m_enemybulletMask = LayerMask.GetMask("EnemyBullet");
        m_status = new Status(2000, 50, 1000, 0, 0);

        m_maxhp = m_status.m_hp;
        m_maxdef = m_status.m_def;

        m_collid = GetComponent<Collider>();
        m_height = m_collid.bounds.size.y;

        m_playerStat = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        GameObject m_monsterHpBarFolder = GameObject.Find("monsterHpBarFolder");
        if (rectTransform == null)
        {            
            rectTransform = Instantiate(prfBossBar).GetComponent<RectTransform>();
            //rectTransform.SetParent(m_monsterHpBarFolder.transform, false);
        }
    }

    void Update()
    {
        if (m_isDead != true)
        {
            //rectTransform.GetComponent<MonsterHealth>().HpBossSet(this);
            CheckState();
            CheckStateForAction();
        }
        if (m_isDead == true)
        {
            if (rectTransform != null)
                Destroy(rectTransform.gameObject);
        }
    }

    void CheckState()
    {
        if (m_isDead != true)
        {
            float dist = Vector3.Distance(m_playerTransform.position, transform.position);
            if (dist < m_arrangeDist)
            {
                m_curState = eCurrentState.eArrange;
            }
            else if (dist <= m_attackDist)
            {
                if (m_nvAgent.enabled == true)
                    m_nvAgent.SetDestination(this.transform.position);
                m_curState = eCurrentState.eAttack;
            }
            else if (dist <= m_battleDist)
            {
                m_curState = eCurrentState.eBattle;
            }
            else if (dist <= m_traceDist)
            {
                m_curState = eCurrentState.eTrace;
            }
            else
            {
                m_curState = eCurrentState.eIdle;
            }
        }
    }

    void CheckStateForAction()
    {
        if (m_isDead != true)
        {
            switch (m_curState)
            {
                case eCurrentState.eIdle://현재상태가 대기중일때 네비끈다
                    m_nvAgent.enabled = false;
                    //_animator.SetBool("isTrace", false);
                    break;
                case eCurrentState.eTrace://현재상태가 추적중일때 네비킨다, 목적지는 플레이어의 위치
                    m_nvAgent.enabled = true;
                    m_nvAgent.destination = m_playerTransform.position;
                    //_animator.SetBool("isTrace", true);
                    m_curShotDelay = 0;
                    break;
                case eCurrentState.eBattle:
                    //_animator.SetBool("isBattle", true);//전투시작
                    m_nvAgent.enabled = true;
                    Battle();
                    break;
                case eCurrentState.eArrange:
                    //m_nvAgent.enabled = true;
                    //Vector3 normalized = Vector3.Normalize(transform.position - m_playerTransform.position);
                    //transform.position = Vector3.Lerp(-m_playerTransform.position, transform.position, 0.1f);
                    //Vector3 newPosition = Vector3.MoveTowards(transform.position, m_playerTransform.position, -1f);
                    //transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref newPosition, 1f);
                    /*
                    Vector3 a = transform.position - m_playerTransform.position;
                     = Vector3.Normalize(a);
                    m_nvAgent.destination = transform.position;
                    */
                    //m_nvAgent.destination = m_playerTransform.position;
                    Attack();
                    break;

                case eCurrentState.eAttack://현재상태가 공격중일때 네비끈다, 공격시작
                    //m_nvAgent.enabled = false;
                    Attack();//공격
                    break;
            }
        }
    }

    void Battle()
    {
        if (m_isDead != true)
        {
            Attack();
            m_reTraceTime += Time.deltaTime;
            if (m_curState == eCurrentState.eBattle)
            {
                m_nvAgent.enabled = true;
                m_nvAgent.destination = m_playerTransform.position;

                if (m_reTraceTime < 3f)
                    return;
                Vector3 playerpos = new Vector3(m_playerTransform.position.x,
                m_playerTransform.position.y, m_playerTransform.position.z);
                transform.RotateAround(playerpos, Vector3.up, Time.deltaTime * 5f);

                m_reTraceTime = 0;
            }
        }
    }    

    void Attack()
    {
        if (m_isDead != true)
        {
            if (m_curState == eCurrentState.eBattle || m_curState == eCurrentState.eAttack)//현재상태가 전투 또는 공격중일때 총알발사
            {
                //float Dist = Vector3.Distance(transform.position, m_playerTransform.position);
                dist = m_ShootHit.distance;
                m_ShootRay.origin = transform.position;
                m_ShootRay.direction = transform.forward;
                transform.LookAt(m_playerTransform);

                //if (Physics.Raycast(ShootRay, out ShootHit, 100f, cubeMask)
                if (Physics.BoxCast(transform.position, transform.lossyScale / 10, transform.forward, Quaternion.identity, dist, m_groundMask))
                {
                    //distance
                    //m_nvAgent.enabled = true;
                    m_nvAgent.destination = m_playerTransform.position;
                }
                /*
                if(ShootHit.collider.tag != "Player")
                {
                    m_nvAgent.destination = m_playerTransform.position;
                }
                */
                //if (ShootHit.collider.tag == "Player")
                else if (Physics.BoxCast(transform.position, transform.lossyScale / 10, transform.forward, Quaternion.identity, dist, m_playerMask))
                //|| Physics.BoxCast(transform.GetChild(0).position, transform.lossyScale / 2, transform.GetChild(0).forward, Quaternion.identity, dist, m_enemybulletMask))                    
                {
                    m_curShotDelay += Time.deltaTime;
                    if (m_curShotDelay >= m_maxShotDelay)
                    {
                        GameObject obj = Managers.Pool.Pop(BulletPrefab);//ObjectPool.current.GetObject(BulletPrefab);
                        //obj.GetComponent<Bullet>().InitBoss(this);

                        obj.transform.position = transform.position;//GetChild:총알발사 위치(FirePos)

                        Vector3 pos = m_playerTransform.position - obj.transform.position;

                        obj.GetComponent<Rigidbody>().velocity = pos.normalized * m_bulletSpeed;
                        obj.transform.rotation = transform.rotation;
                        obj.SetActive(true);
                        m_curShotDelay = 0;
                    }
                    //_animator.SetBool("isAttack", true);
                }
                //너무 가까우면 뒤로가도록
                /*
                else if (Dist < m_attackDist/2f)
                {
                    m_nvAgent.destination = transform.position - m_playerTransform.position;

                    //transform.position = Vector3.Lerp(transform.position, -transform.forward * 2f, 0.1f);
                }
                */
                //레이 인식때문에 조건 하나 더 추가함
                else if (Physics.BoxCast(transform.position, transform.lossyScale / 10, transform.forward, Quaternion.identity, dist, m_enemybulletMask))
                    return;
                else
                {
                    //m_nvAgent.destination = m_playerTransform.position;
                    m_nvAgent.destination = m_playerTransform.position;
                }
            }
        }

    }

    void OnHit()//몬스터가 플레이어를 때릴때
    {
        m_playerStat.m_status.m_hp -= DamageCalculator.TrueDamageCal(m_status.m_atk);
    }

    public void OnDamage(int damage)//몬스터가 플레이어에게 맞을때
    {
        if (m_status.m_def > 0)
        {
            m_status.m_def -= damage;
            if (m_status.m_def < 0)
            {
                m_status.m_hp += m_status.m_def;
                m_status.m_def = 0;
                if (m_status.m_hp < 0)
                    Die();
            }
        }
        else
        {
            m_status.m_hp -= damage;
            if (m_status.m_hp < 0)
            {
                m_status.m_hp = 0;
                Die();
            }
        }

        GameObject damageTaxt = Managers.Resource.Instantiate(Define.Path.UI_DamageTaxt);
        //damageTaxt.GetComponent<UI_DamageTaxt>().OnDamage(10, transform.position);//추후수정

        if ((m_curState == eCurrentState.eIdle))//대기상태에서 맞았을때 추적거리 늘림
        {
            if (dist <= m_traceDist)
            {
                m_traceDist = m_traceDist * 2f;
            }
        }
    }

    void Die()
    {
        if (m_isDead != true)
        {
            //Managers.Game.MiniMap.PopMiniMapSprite(Util.FindChild(gameObject).GetComponent<MiniMapSprite>());
            //Managers.Game.Item.RandomDropItemSpawn(false, transform, 5);
            StartSinking();
        }
        m_isDead = true;
    }
    void StartSinking()
    {
        m_nvAgent.enabled = false;
        StartCoroutine("Sinking");
    }

    IEnumerator Sinking()
    {
        //Managers.Game.Monster.DeSpawnPreprocessing(gameObject);//추후수정
        while (m_currentTime < m_destroyTime)
        {
            m_currentTime += Time.deltaTime;
            transform.Translate(Vector3.down * m_height * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        //Managers.Game.Monster.DeSpawn(gameObject);//추후수정
    }
}
