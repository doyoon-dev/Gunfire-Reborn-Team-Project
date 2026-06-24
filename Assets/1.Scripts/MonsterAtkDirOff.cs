using UnityEngine;

public class MonsterAtkDirOff : MonoBehaviour
{
    float m_lifeTime = 2f;
    void OnEnable()
    {
        Invoke("Die", m_lifeTime);
    }
    void OnDisable()
    {
        CancelInvoke("Die");
    }
    void Die()
    {
        //풀에 담아놓는다.        
        Managers.Pool.Push(gameObject);
    }
}
