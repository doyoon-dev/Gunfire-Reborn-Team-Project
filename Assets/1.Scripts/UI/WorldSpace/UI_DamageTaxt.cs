using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_DamageTaxt : UI_WorldSpace
{
    [SerializeField] UI_NumberString m_number;

    const float m_speed = 1.2f;

    private float m_moveSpeed = m_speed;
    private float m_alphaTime = 1.5f;
    private float m_destroyTime = 0.9f;

    void Start()
    {
    }
    void Update()
    {
        if (gameObject.activeSelf)
        {
            LookAtCamera();
        }
    }

    public void OnDamage(int damage, Vector3 pos)
    {
        m_number.ChangeNumber(damage);
        //m_text.text = damage.ToString();
        Vector3 moveVecter = Random.insideUnitCircle;
        transform.position = pos+ moveVecter;
        gameObject.SetActive(true);
        StartCoroutine(PositionUpdate(moveVecter));
    }

    IEnumerator PositionUpdate(Vector3 moveVecter)
    {
        float time=0;
        Color alpha = Color.white;

        while (m_destroyTime> time)
        {
            time += Time.deltaTime;
            if (m_moveSpeed > 0)
            { 
                transform.position += moveVecter * m_moveSpeed * Time.deltaTime; // 텍스트 위치
                m_moveSpeed -= Time.deltaTime;
            }
            
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * m_alphaTime); // 텍스트 알파값
            m_number.SetStringColor(alpha);
            //m_text.color = alpha;

            yield return null;
        }
        Clear();
    }

    void Clear()
    {
        m_moveSpeed = m_speed;
        Managers.Pool.Push(gameObject);
    }
}
