using UnityEngine;

public class MonsterAtkDir : MonoBehaviour
{
    Transform m_PlayerTarget;
    Vector3 m_HitDir;

    public void Init(Vector3 _HitDir)
    {
        m_HitDir = _HitDir;
        m_HitDir.Normalize();
    }
    void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90f);        
        m_PlayerTarget = GameObject.FindWithTag("Player").GetComponent<Transform>();                
    }    

    void Update()
    {        
        float angle = Vector3.Angle(m_PlayerTarget.forward, m_HitDir);
        if(Vector3.Dot(m_PlayerTarget.right, m_HitDir)>0)
        {
            angle = 360 - angle;
        }
        transform.rotation = Quaternion.Euler(0, 0, angle);       
    }    
}
