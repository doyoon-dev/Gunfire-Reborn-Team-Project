using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class ItemManager : MonoBehaviour
{
    List<DropItem> m_dropItemList = new List<DropItem>();
    Transform m_dropItemRoot;

    void Start()
    {
        if (m_dropItemRoot == null)
        {
            m_dropItemRoot = new GameObject { name = "Item_Root" }.transform;
            m_dropItemRoot.parent = transform;
        }
    }

    public GameObject SpawnItem(GameObject item, Transform transform=null)
    {
        GameObject spawn = Managers.Pool.Pop(item, transform);
        spawn.transform.localPosition = Vector3.zero;
        spawn.transform.parent = m_dropItemRoot;
        return spawn;
    }

    public void DropRandomBullet(Transform transform)
    {
        int rand= Random.Range((int)Define.DropItemType.Bullet_Green, (int)Define.DropItemType.Bullet_Yellow + 1);
        Spawn((Define.DropItemType)rand, transform);
    }    
    
    public void DropAllBullet(Transform transform)
    {
        Define.DropItemType type = Define.DropItemType.Bullet_Green;
        Spawn(type++, transform);
        Spawn(type++, transform);
        Spawn(type++, transform);
    }

    public void RandomDrop(Transform transform)//gold / bullets / Grenade
    {
        DropRandomBullet(transform);
        Spawn(Define.DropItemType.Gold, transform, Random.Range(1, 5));
        Spawn(Define.DropItemType.Grenade, transform, Random.Range(-2,2));
    }

    public void Spawn(Define.DropItemType m_type, Transform transform, int value =1)
    {
        if (value < 1) return;
        string path = "";
        switch (m_type)
        {
            case Define.DropItemType.None:
                break;
            case Define.DropItemType.Gold:
                path = Define.Path.Dropltem_Gold;
                break;
            case Define.DropItemType.Bullet_Green:
                path = Define.Path.Dropltem_Bullet_Green;
                break;
            case Define.DropItemType.Bullet_Blue:
                path = Define.Path.Dropltem_Bullet_Blue;
                break;
            case Define.DropItemType.Bullet_Yellow:
                path = Define.Path.Dropltem_Bullet_Yellow;
                break;           
            case Define.DropItemType.Grenade:
                path = Define.Path.Dropltem_Grenade;
                break;
        }
        Spawn(path, transform, value);
    }

    public void Spawn(string path, Transform transform, int value)
    {
        DropItem item;
        for (int i = 0; i < value; i++)
        {
            item = Managers.Resource.Instantiate(path, transform).GetOrAddComponent<DropItem>();
            item.Init(m_dropItemRoot);
            item.Spread();
            m_dropItemList.Add(item);
        }
    }

    public void DeSpawn(DropItem item)
    {
        m_dropItemList.Remove(item);
        Managers.Resource.Destroy(item.gameObject);
    }
}
