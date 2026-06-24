using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    List<GameObject>  m_monsterList = new List<GameObject>();

    [SerializeField] Transform m_monsterSpawnerRoot = null;
    List<MonsterSpawner> m_spawnerList = new List<MonsterSpawner>();

    [SerializeField]Transform m_monsterRoot = null;

    private int m_killCount=0;

    public int KillCount => m_killCount;


    public void Init()
    {
        if (m_monsterSpawnerRoot == null) return;

        int size = m_monsterSpawnerRoot.childCount;
        for (int i = 0; i < size; ++i)
        {
            MonsterSpawner spawner = m_monsterSpawnerRoot.GetChild(i).GetComponent<MonsterSpawner>();
            if(spawner) m_spawnerList.Add(spawner);
        }
    }
    public void OnUpdate()
    {
        if(AllMonstersClear()&& Managers.Game.IsPlay) Managers.Game.GmaeEnd();
    }

    public bool CurruntRoomMonstersClear()
    {
        return m_monsterList.Count == 0;
    }
    public bool AllMonstersClear()
    {
        foreach (var spawner in m_spawnerList)
        {
            if (spawner.IsSpawn == false) return false;
        }
        if (!Managers.Game.Room.IsAllRoomClear) return false;
        return m_monsterList.Count == 0;
    }

    //동기화 문제있음
    public List<Monster> OverlapSphere(Monster monster, float radius)
    {
        List<Monster> monsters = new List<Monster>();
        foreach (var item in m_monsterList)
        {
            if(item!=monster)
            {
                if(radius<= Vector3.Distance(monster.transform.position, item.transform.position))
                {
                    monsters.Add(item.GetComponent<Monster>());
                }
            }
        }
        return monsters;
    }

    public GameObject Spawn(MonsterSpawnData spawnData)
    {
        if (spawnData.Prefab == null) return null;
        GameObject spawnObj = Managers.Pool.Pop(spawnData.Prefab, m_monsterRoot);
        spawnObj.transform.position = spawnData.transform.position;
        //if(spawnData.DropItemList.Count != 0) spawnObj.GetOrAddComponent<Monster>().SetDropItemList = spawnData.DropItemList;
        m_monsterList.Add(spawnObj);
        if(!spawnObj.GetComponent<Monster>().m_isBoss) Managers.Game.MiniMap.PushMiniMapSprite(Util.FindChild<MiniMapSprite>(spawnObj));
        return spawnObj;
    }
    public void DeSpawn(GameObject obj)
    {
        if (obj == null) return;
        Managers.Pool.Push(obj);
        m_killCount++;
    }

    public void DeSpawnPreprocessing(GameObject obj)
    {
        m_monsterList.Remove(obj);
        Managers.Game.Room.CurruntRoom.ClearRoom();
    }

    public void Clear() { }
}