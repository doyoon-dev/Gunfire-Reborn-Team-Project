using UnityEngine;

public class UI_MonsterAtkDir : MonoBehaviour
{
    [SerializeField] GameObject tset;
    Vector3 m_HitDir;
    float m_lifeTime = 2f;

    public void Init(Vector3 _HitDir)
    {
        m_lifeTime = 2f;
        m_HitDir = _HitDir;
        tset.transform.rotation = Quaternion.Euler(0, 0, Angle());
        m_HitDir.Normalize();
    } 

    void Update()
    {        
        tset.transform.rotation = Quaternion.Euler(0, 0, Angle());
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime < 0) Managers.Resource.Destroy(gameObject);
    }

    float Angle()
    {
        float angle = Vector3.Angle(Managers.Game.Player.transform.forward, m_HitDir);
        if (Vector3.Dot(Managers.Game.Player.transform.right, m_HitDir) > 0)
        {
            angle = 360 - angle;
        }
        return angle;
    }
}
