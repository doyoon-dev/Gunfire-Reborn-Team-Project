using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] MonsterSpawner m_spawner;
    GimmickTrigger m_trigger;
    [SerializeField] List<GameObject> m_clearRewardList;



    public bool IsSpawn { get => m_spawner.IsSpawn; set => m_spawner.IsSpawn = value; }
    public bool IsClear => IsSpawn&& Managers.Game.Monster.CurruntRoomMonstersClear();

    public void Init()
    {
        if (m_spawner.SpawnDatas.Count == 0) return;
        foreach (var item in m_clearRewardList)
        {
            item.SetActive(false);
        }
        m_trigger = transform.GetChild(0)?.GetComponent< GimmickTrigger>();
    }

    public void FirstEnterRoom()
    {
        if (m_spawner.SpawnDatas.Count == 0) return;
        Managers.Game.Room.CloseAllDoor();
        m_spawner.WorkGimmick();
        Managers.Game.Room.CurruntRoom = this;
    }

    public void ClearRoom()
    {
        if (IsSpawn && IsClear)
        {
            foreach (var item in m_clearRewardList)
            {
                item.SetActive(true);
                item.GetComponent<Chest>()?.UnLock();
            }
            Managers.Game.Room.OpenAllDoor();
        }
    }
}
