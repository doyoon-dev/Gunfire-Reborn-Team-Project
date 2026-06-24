using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer m_playerSMR;
    [SerializeField]
    GameObject m_hand;

    Rigidbody m_rigid;
    CapsuleCollider m_capsuleCol;

    bool m_stop = false;

    Animator m_anim;
    #region 플레이어 체력 UI 관련 변수

    public PlayerHpUI m_playerHpUI;

    bool m_isFullShield = true;
    bool m_isDamaged = false;
    bool m_isDie = false;

    #endregion

    #region 플레이어 이동 관련 변수

    [SerializeField]
    float m_dashSpeed = 34f;                    // 플레이어 대쉬 속도
    [SerializeField]
    float m_speed = 15f;                         // 플레이어 이동 속도
    [SerializeField]
    float m_jumpForce;                          // 플레이어 점프

    Vector3 m_DashDirection;                    // 플레이어 대쉬 방향

    float m_moveDirX = 0;                       // 플레이어 x축 이동 값
    float m_moveDirZ = 0;                       // 플레이어 z축 이동 값
    public bool m_isDash = true;                      // 플레이어가 대쉬를 했는지 체크
    bool m_isGround = true;                     // 플레이어가 땅에 닿았는지 체크
    bool m_isDashWall = false;                  // 대쉬 할 때 벽 체크

    #endregion

    #region 카메라 관련 변수

    GameObject m_cam;
    [SerializeField]
    Transform m_originCamPos;

    [SerializeField]
    Transform m_dieCamPos;

    [SerializeField]
    float m_lookSensitivity;                    // 카메라 민감도
    [SerializeField]
    float m_cameraRotationLimit = 88f;          // 카메라 회전 제한

    float m_currentCameraRotationX = 0;         // 카메라 X축 회전 값

    #endregion

    bool m_inventoryOpen = false;
    
    public Status m_status;

    float m_knockbackForce;


    public bool IsDie => m_isDie;
    private void Awake()
    {
        m_cam = GameObject.FindGameObjectWithTag("MainCamera");
        m_playerHpUI = Managers.UI.ShowSceneUI<PlayerHpUI>();
        m_status = new Status(m_playerHpUI.m_currentPlayerHp, 20f, m_playerHpUI.m_currentPlayerShield, 1.5f, 1);

        m_rigid = GetComponent<Rigidbody>();
        m_capsuleCol = GetComponent<CapsuleCollider>();
        m_anim = GetComponent<Animator>();
        m_playerSMR.enabled = false;
        m_knockbackForce = 500f;

    }
    // Update is called once per frame
    void Update()
    {
        m_moveDirX = Input.GetAxisRaw("Horizontal");
        m_moveDirZ = Input.GetAxisRaw("Vertical");
        m_playerHpUI.CurrentHp();
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            // 움직임 막는 불린 변수 추가하기
            if (!m_inventoryOpen)
            {
                Managers.Game.IsPopUpUI = true;
                InventoryUI.m_current.m_inventoryBg.gameObject.SetActive(true);
                InventoryUI.m_current.m_inventoryCenter.ShowWeaponSlot();
                InventoryUI.m_current.m_inventoryCenter.ShowBulletCount();
                m_inventoryOpen = true;
            }
            else
            {
                Managers.Game.IsPopUpUI = false;
                InventoryUI.m_current.m_inventoryBg.gameObject.SetActive(false);
                m_inventoryOpen = false;
            }
        }
        CrosshairHitEnemy();
    }

    private void FixedUpdate()
    {
        // 상점 안 열렸을 때 이동
        
        if (!Managers.Game.IsPopUpUI)
        {
            IsGround();
            CameraRotate();
            Move();
            Jump();
            CharacterRotate();
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (m_isDash)
                {
                    StopCoroutine("Dash");
                    Managers.Sound.Play(Define.Path.SOUND_Effect_Dash, false, Define.Sound.Effect);
                    StartCoroutine("Dash");
                    WeaponManager.m_current.m_weaponUI.m_dashUI.m_coolBg.fillAmount = 0;
                    WeaponManager.m_current.m_weaponUI.m_dashUI.StartCoroutine("Cooltime");
                }
            }
        }
        else
        {
            MoveStop();
        }
    }

    #region 플레이어 이동관련 함수


    // 플레이어가 땅에 있는지 체크
    void IsGround()
    {
        // m_capsuleCol.bounds.extents.y : 캡슐콜라이더 영역(bounds)의 반(extents) 사이즈
        // 0.1f : 오차 범위
        m_isGround = Physics.Raycast(transform.position, Vector3.down, m_capsuleCol.bounds.extents.y + 0.1f);
    }

    // 플레이어 점프 함수
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_isGround)
        {
            m_rigid.velocity = transform.up * m_jumpForce;
        }
    }

    // 플레이어 이동함수
    private void Move()
    {
        Vector3 moveHorizontal = transform.right * m_moveDirX;
        Vector3 moveVertical = transform.forward * m_moveDirZ;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * m_speed;

        if (velocity != Vector3.zero && WeaponManager.m_current.m_isFire == false)
        {
            WeaponManager.m_current.m_currentWeapon.GetComponent<Gun>().m_anim.SetBool("Walk", true);
        }
        else
        {
            WeaponManager.m_current.m_currentWeapon.GetComponent<Gun>().m_anim.SetBool("Walk", false);
        }

        int wallMask = LayerMask.GetMask("Wall");
        if (Physics.Raycast(transform.position, velocity, 1.5f, wallMask))
        {
            velocity = Vector3.zero;
        }
        m_rigid.velocity = new Vector3(velocity.x, m_rigid.velocity.y, velocity.z);
    }
    private void MoveStop()
    {
        m_rigid.velocity = Vector3.zero;
    }

    // 카메라 상하 회전
    void CameraRotate()
    {
        float xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = xRotation * m_lookSensitivity;
        m_currentCameraRotationX -= cameraRotationX;
        m_currentCameraRotationX = Mathf.Clamp(m_currentCameraRotationX, -m_cameraRotationLimit, m_cameraRotationLimit);

        m_cam.transform.localEulerAngles = new Vector3(m_currentCameraRotationX, 0f, 0f);
    }

    // 캐릭터 좌우 회전
    void CharacterRotate()
    {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * m_lookSensitivity;
        m_rigid.MoveRotation(m_rigid.rotation * Quaternion.Euler(characterRotationY));
    }

    // 플레이어 대쉬 함수
    IEnumerator Dash()
    {
        if (m_moveDirX != 0f || m_moveDirZ != 0f)
        {
            Vector3 dirX = transform.right * m_moveDirX;
            Vector3 dirZ = transform.forward * m_moveDirZ;
            m_DashDirection = dirX + dirZ;
        }
        else
        {
            m_DashDirection = transform.forward;
        }

        m_DashDirection.Normalize();

        float timer = 0;
        while (timer < 0.5f)
        {
            timer += Time.fixedDeltaTime;

            int wallMask = LayerMask.GetMask("Wall");
            int doorMask = LayerMask.GetMask("Door");
            int layerMask = wallMask + doorMask;
            m_isDashWall = Physics.Raycast(transform.position, m_DashDirection, 1f, layerMask);

            if (!m_isDashWall)
            {
                m_rigid.MovePosition(transform.position + (m_DashDirection * Time.fixedDeltaTime * m_dashSpeed * (1 - timer)));
            }
            yield return new WaitForFixedUpdate();
        }
    }

    //
    #endregion

    // 실드 채우는 함수
    IEnumerator FillUpShield()
    {
        // 실드가 꽉 차있을 때 함수 호출 X
        // 피격중이 아닐 때, 실드가 깎여있을 때
        // 실드 충전중 피격 시 실드 충전 중지
        // 실드 충전 지연시간 3초
        // 최대 실드의 10% 충전
        yield return new WaitForSeconds(3f);
        while (m_isFullShield == false && m_isDie == false)
        {
            m_playerHpUI.m_currentPlayerShield += m_playerHpUI.m_playerMaxShield * 0.1f * Time.deltaTime;
            m_playerHpUI.m_playerShieldBar.fillAmount = m_playerHpUI.m_currentPlayerShield / m_playerHpUI.m_playerMaxShield;
            m_playerHpUI.m_playerShieldText.text = (int)m_playerHpUI.m_currentPlayerShield + " / " + m_playerHpUI.m_playerMaxShield;

            if (m_playerHpUI.m_currentPlayerShield >= m_playerHpUI.m_playerMaxShield)
            {
                m_playerHpUI.m_playerShieldBar.fillAmount = m_playerHpUI.m_playerMaxShield;
                m_playerHpUI.m_playerShieldText.text = (int)m_playerHpUI.m_currentPlayerShield + " / " + m_playerHpUI.m_playerMaxShield;
                m_isFullShield = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    // 회복 아이템 먹었을 때
    public void Heal()
    {
        m_playerHpUI.m_currentPlayerHp = m_playerHpUI.m_playerMaxHp;
    }

    // 플레이어 피격 함수
    public void Damaged(int damage)
    {
        m_isFullShield = false;

        if (m_playerHpUI.m_currentPlayerShield > 0)
        {
            m_playerHpUI.ShowPlayerStatus(true, damage);
            if (m_playerHpUI.m_currentPlayerShield < 0)
            {
                m_playerHpUI.m_currentPlayerHp += m_playerHpUI.m_currentPlayerShield;
                m_playerHpUI.m_playerHpBar.fillAmount = m_playerHpUI.m_currentPlayerHp / m_playerHpUI.m_playerMaxHp;
                m_playerHpUI.m_playerHpText.text = (int)m_playerHpUI.m_currentPlayerHp + " / " + m_playerHpUI.m_playerMaxHp;
                m_status.m_def = 0;
                m_playerHpUI.m_playerShieldBar.fillAmount = 0;
                m_playerHpUI.m_currentPlayerShield = 0;
                m_playerHpUI.m_playerShieldText.text = (int)m_playerHpUI.m_currentPlayerShield + " / " + m_playerHpUI.m_playerMaxShield;
                PlayerHPZero();
            }
        }
        else
        {
            m_playerHpUI.ShowPlayerStatus(false, damage);
            PlayerHPZero();
        }

        if (m_isFullShield == false && m_isDie == false)
        {
            StopCoroutine("FillUpShield");
            StartCoroutine("FillUpShield");
        }

    }

    void PlayerHPZero()
    {
        if (m_playerHpUI.m_currentPlayerHp <= 0)
        {
            m_playerHpUI.m_currentPlayerHp = 0;
            m_playerHpUI.m_playerHpBar.fillAmount = 0;
            m_playerHpUI.m_playerHpText.text = (int)m_playerHpUI.m_currentPlayerHp + " / " + m_playerHpUI.m_playerMaxHp;
            Die();
        }
    }

    void Die()
    {
        m_playerSMR.enabled = true;
        m_hand.GetComponent<GunController>().CancelReload();
        m_anim.SetBool("Death", true);
        m_hand.gameObject.SetActive(false);
        m_cam.transform.localPosition = m_dieCamPos.localPosition;
        m_cam.transform.localRotation = m_dieCamPos.localRotation;
        m_isDie = true;
        m_rigid.isKinematic = true;
        m_capsuleCol.enabled = false;
        if(Managers.Game.IsPlay)Managers.Game.GmaeEnd();
    }

    private void Alive()
    {
        Managers.Game.IsPopUpUI = false;
        Managers.Game.IsPlay = true;
        m_playerHpUI.m_currentPlayerShield = m_playerHpUI.m_playerMaxShield;
        m_playerHpUI.m_playerShieldBar.fillAmount = 1;
        m_playerHpUI.m_playerShieldText.text = (int)m_playerHpUI.m_currentPlayerShield + " / " + m_playerHpUI.m_playerMaxShield;
        m_playerHpUI.m_currentPlayerHp = m_playerHpUI.m_playerMaxHp;
        m_playerHpUI.m_playerHpBar.fillAmount = 1;
        m_playerHpUI.m_playerHpText.text = (int)m_playerHpUI.m_currentPlayerHp + " / " + m_playerHpUI.m_playerMaxHp;

        m_playerSMR.enabled = false;
        m_anim.SetBool("Death", false);
        m_hand.gameObject.SetActive(true);
        m_cam.transform.localPosition = m_originCamPos.localPosition;
        m_cam.transform.localRotation = m_originCamPos.localRotation;
        m_isDie = false;
        m_rigid.isKinematic = false;
        m_capsuleCol.enabled = true;
    }
    
    public void KnockBack(Vector3 direction)
    {
        m_rigid.AddForce(direction * m_knockbackForce, ForceMode.Impulse);
    }

    void CrosshairHitEnemy()
    {
        int monsterLayer = LayerMask.GetMask("Monster");
        if (Physics.Raycast(m_cam.transform.position, m_cam.transform.forward, 1000f, monsterLayer))
        {
            WeaponManager.m_current.m_gunCon.m_crosshair.HitEnemy();
        }
        else
        {
            WeaponManager.m_current.m_gunCon.m_crosshair.OriginCrosshair();
        }
    }
}
