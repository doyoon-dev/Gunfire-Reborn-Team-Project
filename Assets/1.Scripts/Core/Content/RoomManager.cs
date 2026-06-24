using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    List<Room> m_roomList = new List<Room>();
    Door[] m_doorList;
    Room m_curruntRoom;

    public bool IsCircuit//모든 방을 순회 했는가
    {
        get
        {
            foreach (var item in m_roomList)
            {
                if (!item.IsSpawn) return false;
            }
            return true;
        }
    }

    public Room CurruntRoom { get => m_curruntRoom; set => m_curruntRoom = value; }

    public bool IsAllRoomClear
    {
        get
        {
            foreach (var item in m_roomList)
            {
                if (!item.IsClear) return false;
            }
            return true;
        }
    }

    public void Init()
    {
        m_doorList = GameObject.FindObjectsOfType<Door>();
       // OpenAllDoor();
        if (m_roomList.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                m_roomList.Add(transform.GetChild(i).GetComponent<Room>());
                m_roomList[i].Init();
            }
        }
    }

    public void CloseAllDoor()
    {
        foreach (var item in m_doorList)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void OpenAllDoor()
    {
        foreach (var item in m_doorList)
        {
            item.gameObject.SetActive(false);
        }
    }
}
