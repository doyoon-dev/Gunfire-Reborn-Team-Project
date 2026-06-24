using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrenade : MonoBehaviour
{

    // 폭탁 궤적 변수
    [SerializeField]
    Transform m_handPos;
    Vector3 m_startPos;
    Vector3 m_endPos;

    LineRenderer m_grenadeLine;

    [SerializeField]
    float m_grenadeRange;

    Vector3 m_dir;

    [SerializeField]
    Camera m_cam;

    // Start is called before the first frame update
    void Start()
    {
        m_grenadeLine = GetComponent<LineRenderer>();
        m_grenadeLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        m_startPos = m_handPos.transform.position;
        m_dir = m_cam.transform.forward - m_handPos.position;
        m_dir.Normalize();

        GrenadeAlpha();

        if (!Managers.Game.IsPopUpUI)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                if (WeaponManager.m_current.m_curGrenade > 0)
                {
                    m_grenadeLine.enabled = true;
                    PredictTrajectory(m_handPos.position, m_handPos.transform.forward * 30f);
                }
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                StartCoroutine("GrenadeUIScale");
                m_grenadeLine.enabled = false;
                if (WeaponManager.m_current.m_curGrenade > 0)
                {
                    ShootGrenade(WeaponManager.m_current.CreateGrenade());
                }
            }
        }
    }

    // 폭탄 궤적 보여주는 함수
    void PredictTrajectory(Vector3 startPos, Vector3 velocity)
    {
        int step = 60;
        float deltaTime = Time.deltaTime;
        Vector3 gravity = Physics.gravity;

        Vector3 position = startPos;

        for (int i = 0; i < step; i++)
        {
            position += velocity * deltaTime + 0.5f * gravity * deltaTime * deltaTime;
            velocity += gravity * deltaTime;
            m_grenadeLine.SetPosition(i, position);
        }
    }

    void ShootGrenade(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().isKinematic = false;
        obj.GetComponent<Rigidbody>().velocity = m_handPos.transform.forward * 30f;
        WeaponManager.m_current.m_curGrenade--;
    }
    void GrenadeAlpha()
    {
        if(WeaponManager.m_current.m_curGrenade > 0)
        {
            WeaponManager.m_current.m_weaponUI.m_grenadeAlpha.color = new Color(1, 1, 1, 1);
        }
    }
    public IEnumerator GrenadeUIScale()
    {
        WeaponManager.m_current.m_weaponUI.m_grenadeImage.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        yield return new WaitForSeconds(0.1f);
        WeaponManager.m_current.m_weaponUI.m_grenadeImage.localScale = new Vector3(1f, 1f, 1f);
        if (WeaponManager.m_current.m_curGrenade < 1)
        {
            WeaponManager.m_current.m_weaponUI.m_grenadeAlpha.color = new Color(1, 1, 1, 0.5f);
        }
    }


}
