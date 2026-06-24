using UnityEngine;

public class MonsterCircleRangeView : MonoBehaviour
{      
    float m_lifeTime = 2f;
    Vector3 m_originScale;
    public void OnEnable()
    {
        m_originScale = transform.localScale;
        Invoke("Die", m_lifeTime);        
    }
   
    private void OnDisable()
    {
        CancelInvoke("Die");
        transform.localScale = m_originScale;
    }
    public void Die()
    {
        //풀에 담아놓는다.        
        Managers.Pool.Push(gameObject);
    }
}
