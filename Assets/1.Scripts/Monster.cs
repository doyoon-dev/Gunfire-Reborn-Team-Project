using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    protected Transform m_playerTransform;
    protected Collider m_collid;    
    protected float m_traceDist; //추적사정거리//CheckState()에서 사용
    protected float m_battleDist; //전투사정거리//CheckState()에서 사용
    protected float m_attackDist; ////공격사정거리//CheckState()에서 사용
    protected float m_arrangeDist; ////정비거리(너무가까울때 거리조정)//CheckState()에서 사용
    protected bool m_isDead = false;//사망여부
    protected float m_maxShotDelay;//최대공격딜레이//Attack()에서 사용
    protected float m_curShotDelay;//딜레이계산//Attack()에서 사용
    protected float m_bulletSpeed;//총알속도//Attack()에서 사용
    protected float m_reTraceTime;//전투상태에서 해당 시간 이후 다시 공격하러감
    protected UnityEngine.AI.NavMeshAgent m_nvAgent;
    protected eCurrentState m_curState = eCurrentState.eIdle;
    protected eCurrentState m_prevState;    // 이전상태 저장해서 행동 정할때 사용
    protected float m_dist;//레이 검사용
    protected float m_height;            //오브젝트 높이
    protected float m_destroyTime = 2f;//오브젝트 삭제시간
    protected float m_currentTime;    //오브젝트 삭제시간계산용 
    protected PlayerController m_playerStat; //플레이어의 스탯 불러오기
    protected float m_monSpeed;     //몬스터 이동속도

    Define.ElementalType elementalType;

    //애니메이션추가//
    protected Animator m_anim;
    
    public Status m_status; //몬스터 스탯
    
    public float m_maxhp;
    public float m_maxdef;

    protected float m_maxShieldBoss = 0;
    protected float m_maxHpBoss = 0;
    protected GameObject m_monsterHpBarFolder;

    //protected bool m_isDeBuffed = false;

    public bool m_hasShield = false;

    public bool m_isManipulation = false;

    public bool m_isBoss;     //몬스터가 보스인지 아닌지 확인(자식클래스에서 각각 지정)

    protected Monster m_closestMonster;     //정신지배시 근처 몬스터 찾을때 사용

    protected bool hasSetnvAgent = false;

    protected bool m_isMeleeMonster = false;

    protected bool m_isRangedMonster = false;

    protected bool m_onMeleeAttackCoolDown = false;

    protected bool m_onRangeAttackCoolDown = false;

    protected string m_monsterName;

    public string m_monsterNameView;

    protected bool isOnPattern = false;

    //FSM 적용

    [SerializeField]
    protected GameObject prfHpBar;

    [SerializeField]
    protected GameObject prfDefBar;

    [SerializeField]
    GameObject prfBossBar;

    protected RectTransform rectTransform;

    ElementalInfo m_elementalInfo;
    //private MeshRenderer meshRenderer;

    protected PlayerHpUI m_playerHp;

    [SerializeField]
    protected SkinnedMeshRenderer meshRenderer;

    GameObject hitEffect;

    protected GameObject bossRageEffect;

    protected enum eCurrentState
    {
        eIdle,
        eTrace,
        eBattle,
        eAttack,
        eArrange
    }

    protected virtual void OnDrawGizmos()
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
    protected virtual void OnEnable()
    {
        hasSetnvAgent = false;
        if(rectTransform != null)
            rectTransform.gameObject.SetActive(true);        
        Material[] mats = meshRenderer.materials;
        mats[0].SetFloat("_Cutoff", 0);
        m_prevState = eCurrentState.eIdle;        
    }
    protected virtual void OnDisable()
    {
        m_nvAgent.enabled = false;
        m_onMeleeAttackCoolDown = false;
        m_onRangeAttackCoolDown = false;
        m_isManipulation = false;
        //if(m_collid.enabled == false)
        //{
        //    m_collid.enabled = true;
        //}
    }
    private void Awake()
    {
        m_nvAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_elementalInfo = new ElementalInfo(this);
        meshRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
        hitEffect = transform.Find("ElectricalSparksHitEffect").gameObject;
    }
    
    protected virtual void Start()
    {
        m_anim = GetComponent<Animator>();

        m_playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        m_status = new Status(m_status.m_hp, m_status.m_atk, m_status.m_def, m_status.m_cri, m_status.m_luckyShot);//몬스터의 능력치 저장함
        m_monsterNameView = m_monsterName;

        m_maxhp = m_status.m_hp;
        m_maxdef = m_status.m_def;

        m_collid = GetComponent<Collider>();
        m_height = m_collid.bounds.size.y;

        m_playerStat = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        m_monsterHpBarFolder = GameObject.Find("monsterHpBarFolder");
        if (m_monsterHpBarFolder == null)
        {
            m_monsterHpBarFolder = new GameObject { };
            m_monsterHpBarFolder.name = "monsterHpBarFolder";
        }
        if(m_isBoss != true)
        {
            if (rectTransform == null)
            {
                if (m_maxdef == 0)
                {
                    rectTransform = Managers.Pool.Pop(prfHpBar).GetComponent<RectTransform>();                    
                }
                else
                {
                    rectTransform = Managers.Pool.Pop(prfDefBar).GetComponent<RectTransform>();
                }
                rectTransform.SetParent(m_monsterHpBarFolder.transform, false);
                rectTransform.GetComponent<MonsterHealth>().m_textMonsterName.text = m_monsterNameView;
            }
        }
        if(m_isBoss)
        {
            if (rectTransform == null)
            {
                rectTransform = Instantiate(prfBossBar).GetComponent<RectTransform>();
                //rectTransform.SetParent(m_monsterHpBarFolder.transform, false);
            }
            rectTransform.SetParent(m_monsterHpBarFolder.transform, false);
            m_maxShieldBoss = m_status.m_def;
            m_maxHpBoss = m_status.m_hp;
            rectTransform.GetComponent<MonsterHealth>().m_textMonsterName.text = m_monsterNameView;                    
        }
        //몬스터이동속도 설정
        m_nvAgent.speed = m_monSpeed;
    }

    protected virtual void Update()
    {
        if (m_playerStat != null && Managers.Game.Player.m_playerHpUI.m_currentPlayerHp <= 0)
        {
            m_curState = eCurrentState.eIdle;
            if(m_isMeleeMonster == true)
            {
                m_anim.SetBool("onMeleeAttack", false);
                m_anim.ResetTrigger("attackTrigger");
            }
            else
            {
                m_anim.SetBool("isAttacking", false);
            }
            m_anim.SetBool("isMoving", false);
            return;
        }
        if (m_isBoss != true)
            ShowMonsterHealthBar();
        else
            ShowBossHealthBar();          
        CheckShield();
        CheckState();
        CheckStateForAction();
        m_elementalInfo.OnUpdate();
    }

    protected virtual void ShowMonsterHealthBar()
    {
        rectTransform.transform.rotation = m_playerTransform.transform.rotation;
        rectTransform.transform.position = transform.position;
        rectTransform.transform.position = new Vector3(rectTransform.transform.position.x
            , rectTransform.transform.position.y + 3f
            , rectTransform.transform.position.z);
        rectTransform.GetComponent<MonsterHealth>().HpSet(this);
    }
    protected virtual void ShowBossHealthBar()
    {
        rectTransform.GetComponent<MonsterHealth>().HpSet(this);
    }

    public void PushElemental(ElementalStat stat, int damage)
    {
        m_elementalInfo.ApplyElemental(stat, damage);
    }
    
    protected virtual void CheckState()
    {
        if(m_isDead != true)
        {
            if(m_isManipulation != true)
            {
                float dist = Vector3.Distance(m_playerTransform.position, transform.position);
                
                if (dist <= m_attackDist)
                {
                    if (m_nvAgent.enabled == true)
                        m_nvAgent.SetDestination(this.transform.position);
                    m_curState = eCurrentState.eAttack;
                    if (m_isMeleeMonster != true)
                    {
                        hasSetnvAgent = false;
                    }
                }
                else if (dist <= m_battleDist)
                {
                    if(hasSetnvAgent != true)
                    {
                        if (m_prevState == eCurrentState.eTrace)
                        {
                            return;
                        }
                        else
                        {
                            m_curState = eCurrentState.eBattle;                            
                        }
                        if (m_isMeleeMonster != true)
                        {
                            hasSetnvAgent = true;
                        }
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
            }
            m_prevState = m_curState;
        }
    }
    
    protected virtual void CheckStateForAction()
    {        
        if (m_isDead != true)
        {
            if (m_isManipulation)
            {
                //정신지배 당했을 때 행동
                AttackOtherMonster();
                return;
            }
            if (m_isManipulation != true)
            {
                switch (m_curState)
                {
                    case eCurrentState.eIdle://현재상태가 대기중일때 네비끈다
                        m_nvAgent.enabled = false;
                        if (m_anim != null)
                        {
                            m_anim.SetBool("isAttacking", false);
                            m_anim.SetBool("isMoving", false);
                            //if(m_prevState == eCurrentState.eTrace)
                            //{
                            //    m_anim.SetBool("isMoving", false);
                            //}
                        }
                        break;
                    case eCurrentState.eTrace://현재상태가 추적중일때 네비킨다, 목적지는 플레이어의 위치
                        m_nvAgent.enabled = true;
                        
                        if (m_nvAgent.enabled == true)
                        {                            
                            m_nvAgent.destination = m_playerTransform.position;
                        }
                            

                        if(m_isMeleeMonster == true)
                        {
                            if (m_onMeleeAttackCoolDown == true)
                            {
                                m_nvAgent.destination = transform.position;
                            }
                        }

                        if (m_anim != null)
                        {
                            m_anim.SetBool("isAttacking", false);
                            m_anim.SetBool("isMoving", true);
                            if(m_isMeleeMonster == true)
                            {
                                m_anim.ResetTrigger("attackTrigger");                                
                            }                            
                        }
                        m_curShotDelay = 0;
                        break;
                    case eCurrentState.eBattle:
                        if (m_prevState == eCurrentState.eTrace)
                            m_nvAgent.enabled = true;
                        else if (m_prevState == eCurrentState.eBattle || m_prevState == eCurrentState.eAttack)
                        {
                            if (m_isMeleeMonster != true)
                            {
                                if (m_anim != null)
                                {
                                    m_anim.SetBool("isMoving", true);
                                    m_anim.SetBool("isAttacking", false);
                                    m_nvAgent.enabled = true;
                                }
                                    //m_nvAgent.enabled = false;
                            }
                            if(m_onMeleeAttackCoolDown == true)
                            {
                                m_nvAgent.enabled = false;
                            }
                        }
                        Battle();
                        break;
                    case eCurrentState.eAttack:
                        if (m_anim != null)
                        {
                            if(m_isMeleeMonster != true)
                            {
                                m_anim.SetBool("isAttacking", true);
                            }
                        }
                        if (m_prevState == eCurrentState.eTrace || m_prevState == eCurrentState.eBattle)
                        {
                            if (m_isMeleeMonster != true)
                            {
                                m_nvAgent.destination = transform.position;
                            }
                            if (m_onMeleeAttackCoolDown != true)
                            {
                                m_nvAgent.destination = transform.position;
                            }
                        }
                        Attack();
                        break;
                }
            }
        }        
    }

    protected virtual void Battle()
    {        
        if (m_isDead != true)
        {
            Attack();
            m_reTraceTime += Time.deltaTime;
            if (m_curState == eCurrentState.eBattle)
            {
                m_nvAgent.enabled = true;
                if (m_nvAgent.enabled == true)
                {
                    //new Vector3(m_playerTransform.position.x, transform.position.y, m_playerTransform.position.z);
                    m_nvAgent.destination = m_playerTransform.position;
                }                    
                if (m_reTraceTime < 1f)
                    return;
                
                m_reTraceTime = 0;
            }
        }
    }

    protected virtual void Attack()
    {

    }

    protected virtual void OnHit()//몬스터가 플레이어를 때릴때
    {
        m_playerStat.m_status.m_hp -= DamageCalculator.TrueDamageCal(m_status.m_atk);        
    }

    public void OnDamage(int damage)//몬스터가 플레이어에게 맞을때
    {
        if (m_isDead != true)
        {
            if(isOnPattern != true)
            {
                if (m_status.m_def > 0)
                {
                    m_status.m_def -= damage;

                    if (hitEffect != null)
                    {
                        // 이펙트 오브젝트를 활성화하여 재생
                        hitEffect.SetActive(true);

                        // 이펙트가 끝나면 비활성화
                        StartCoroutine(DeactivateHitEffect(hitEffect));
                    }
                    if (m_status.m_def < 0)
                    {
                        m_status.m_hp += m_status.m_def;
                        m_status.m_def = 0;
                        if (m_status.m_hp < 0)
                        {
                            m_isDead = true;
                            Die();
                        }
                    }
                }
                else
                {
                    m_status.m_hp -= damage;

                    if (hitEffect != null)
                    {
                        // 이펙트 오브젝트를 활성화하여 재생
                        hitEffect.SetActive(true);

                        // 이펙트가 끝나면 비활성화
                        StartCoroutine(DeactivateHitEffect(hitEffect));
                    }

                    if (m_status.m_hp < 0)
                    {
                        m_status.m_hp = 0;
                        Die();

                    }
                }
                if (m_isBoss != true)
                {
                    Managers.UI.MakeWorldSpaceUI<UI_DamageTaxt>(Managers.UI.Root.transform).
                OnDamage(damage, transform.position);
                }
                if (m_isBoss == true)
                {
                    Managers.UI.MakeWorldSpaceUI<UI_DamageTaxt>(Managers.UI.Root.transform).
                OnDamage(damage, new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z));
                }
            }            
            
            /*//몬스터 스폰되자마자 추적하게 바꾸면서 필요없어짐
            if (m_isBoss != true)
            {
                if ((m_curState == eCurrentState.eIdle))//대기상태에서 맞았을때 추적거리 늘림
                {
                    if (m_dist <= m_traceDist)
                    {
                        m_traceDist = m_traceDist * 2f;
                    }
                }
            }
            */
        }
    }
    public IEnumerator DeactivateHitEffect(GameObject hitEffectPrefab)
    {
        float effectDuration = 0.5f;

        // 일정 시간이 지난 후 이펙트 오브젝트를 비활성화
        yield return new WaitForSeconds(effectDuration);

        if (hitEffectPrefab != null)
        {
            // 이펙트 오브젝트를 비활성화
            hitEffectPrefab.SetActive(false);
        }
    }

    protected virtual void Die()
    {
        if (m_isDead != true)
        {            
            m_isDead = true;
            m_collid.enabled = false;
            if (m_anim != null)
            {
                m_anim.SetTrigger("dieTrigger");
            }
            if (m_isBoss)
            {
                //GameObject bossWeaponEffect = transform.Find("WildFire").gameObject;
                //if(bossWeaponEffect != null)
                //{
                //    bossWeaponEffect.SetActive(false);
                //}
                if(bossRageEffect != null)
                {
                    bossRageEffect.SetActive(false);
                }
            }
            Managers.Game.MiniMap.PopMiniMapSprite(Util.FindChild<MiniMapSprite>(gameObject));

            if(m_isMeleeMonster || m_isRangedMonster)
            {
                Managers.Game.Item.RandomDrop(transform);
            }
            else if(m_isBoss)
            {
                Managers.Game.Item.DropAllBullet(transform);
                Managers.Game.Item.RandomDrop(transform);
            }

            m_elementalInfo.Clear();
            StartSinking();
        }
        m_curState = eCurrentState.eIdle;
        rectTransform.gameObject.SetActive(false);
    }

    protected virtual void StartSinking()
    {
        m_nvAgent.enabled = false;
        m_currentTime = 0;
        StartCoroutine("Sinking");
    }

    protected virtual IEnumerator Sinking()
    {
        Managers.Game.Monster.DeSpawnPreprocessing(gameObject);
        while (m_currentTime < m_destroyTime)
        {
            Material[] mats = meshRenderer.materials;
            mats[0].SetFloat("_Cutoff", m_currentTime *0.5f);
            m_currentTime += Time.deltaTime;
            //transform.Translate(Vector3.down * m_height * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        //Managers.Game.Monster.DeSpawn(gameObject);
    }
    protected virtual void DeSpawn()
    {
        m_collid.enabled = true;
        Managers.Game.Monster.DeSpawn(gameObject);
    }
    
    //public void ApplyDeBuff(Define.ElementalType elementalType, bool isDeBuff)
    public void ApplyDeBuff(bool isDeBuff)
    {
        if(elementalType.HasFlag(Define.ElementalType.Corrosion))   //산성, 효과:슬로우
        {
            m_nvAgent.speed = isDeBuff ? (m_monSpeed * 0.5f) : m_monSpeed;
        }
        if (elementalType.HasFlag(Define.ElementalType.Manipulation))   //화염+전기, 효과:정신지배
        {
            if(m_isBoss != true)
            {
                m_isManipulation = isDeBuff ? (m_isManipulation = true) : (m_isManipulation = false);
            }
        }
    }
    public void CheckShield()
    {
        if (m_status.m_def > 0)
        {
            m_hasShield = true;
        }
        else
        {
            m_hasShield = false;
        }
    }
    
    protected virtual void AttackOtherMonster()
    {        
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        Vector3 selfPosition = transform.position;

        float closestDistance = Mathf.Infinity;
        int closestMonsterIndex = -1;

        // 자신을 제외한 다른 몬스터들 중 가장 가까운 몬스터 선택
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i] != gameObject)
            {
                // 다른 몬스터와의 거리 계산
                float distance = Vector3.Distance(selfPosition, monsters[i].transform.position);

                // 가장 가까운 몬스터 업데이트
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestMonsterIndex = i;
                }
            }
        }
        // 가장 가까운 몬스터가 있을 경우에만 공격 수행
        if (closestMonsterIndex != -1)
        {
            // 가장 가까운 몬스터의 거리 확인
            float closestDistanceThreshold = m_traceDist;

            if(closestDistance <= closestDistanceThreshold)// 가장 가까운 몬스터가 일정 범위 안에 있을 때
            {
                // 가장 가까운 몬스터 공격
                Monster m_closestMonster = monsters[closestMonsterIndex].GetComponent<Monster>();
                AttackOtherMonsterAction(m_closestMonster);
            }
            else
            {
                // 가장 가까운 몬스터가 공격범위 안에 없다면 가만히 대기
            }
        }
    }
    protected virtual void AttackOtherMonsterAction(Monster m_closestMonster)
    {
        
    }
}
