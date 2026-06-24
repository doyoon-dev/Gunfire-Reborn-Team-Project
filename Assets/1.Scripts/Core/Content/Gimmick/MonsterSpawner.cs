using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : Gimmick
{
    List<MonsterSpawnData> m_spawnDataList = new List<MonsterSpawnData>();
    bool m_isSpawn=false;
    public List<MonsterSpawnData> SpawnDatas { get => m_spawnDataList; }
    public bool IsSpawn { get => m_isSpawn; set => m_isSpawn = value; }


    private List<GameObject> m_listMonster = new List<GameObject>();

    public void Start() { Init(); }

    public void Init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf)
                m_spawnDataList.Add(transform.GetChild(i)?.GetComponent<MonsterSpawnData>());
        }
    }

    public bool CheckAllMonstersDie()
    {
        //스폰 상태 확인
        if (!m_isSpawn|| 
            m_listMonster.Count == 0)
        {
            return false;
        }

        int l_count = 0;
        //몬스터 제거 확인
        for (int i = 0; i < m_listMonster.Count; i++)
        {
            if (m_listMonster[i].gameObject.activeSelf) return false;
            else l_count++;
        }
        if (m_listMonster.Count == l_count) return true;

        return false;
    }

    public override void WorkGimmick()
    {
        StartSpawn();
    }

    public void StartSpawn()
    {
        if (m_spawnDataList.Count == 0)
        {
            return;
        }
        int l_spawnSize = m_spawnDataList.Count;
        for (int i = 0; i < l_spawnSize; ++i)
        {
            GameObject go = Managers.Game.Monster.Spawn(m_spawnDataList[i]);
            m_listMonster.Add(go);
        }
        IsSpawn = true;
    }
}
