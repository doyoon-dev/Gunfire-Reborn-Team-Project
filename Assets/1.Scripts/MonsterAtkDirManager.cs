using UnityEngine;

public class MonsterAtkDirManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_AtkDirPrefab;

    public Vector3 m_originPos;

    public static MonsterAtkDirManager current = null;//주소값 저장

    void Start()
    {
        if (current == null)
            current = this;//내 오브젝트를 current에 넣는다.
        else
        {
            Debug.LogError("Not Single GameControlManager");
            Destroy(gameObject); //오브젝트 삭제
        }
    }    
    
    public void OriginMonster(Vector3 HitDir)
    {        
        GameObject obj = Managers.Pool.Pop(m_AtkDirPrefab);
        MonsterAtkDir sc = obj.GetComponentInChildren<MonsterAtkDir>();
        sc.Init(HitDir);
    }   
}
