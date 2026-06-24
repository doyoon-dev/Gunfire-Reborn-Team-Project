using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    class Pool
    {
        public GameObject original { get; private set; }
        private Transform objectRoot = null;
        private Stack<GameObject> poolStack = new Stack<GameObject>();
        public Transform Root { get => objectRoot; set => objectRoot = value; }

        public void Init(GameObject origin, int count = 5)
        {
            original = origin;
            Root = new GameObject().transform;
            Root.name = $"{origin.name}_Root";

            for (int i = 0; i < count; ++i)
            {
                Push(Create());
            }
        }

        GameObject Create()
        {
            GameObject go = Object.Instantiate<GameObject>(original);
            go.name = original.name;
            return go;
        }

        public void Push(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            RectTransform rectTransform = null;
            if (obj.TryGetComponent(out rectTransform) == true)
            {
                rectTransform.SetParent(Root);
            }
            else
            {
                obj.transform.parent = Root;
            }
            obj.SetActive(false);
            poolStack.Push(obj);
        }

        public GameObject Pop(Transform parent)
        {
            GameObject obj;
            if (poolStack.Count > 0)
            {
                obj = poolStack.Pop();
            }
            else
            {
                obj = Create();
            }

            obj.gameObject.SetActive(true);
            obj.transform.SetParent(parent == null ? Root : parent);

            return obj;
        }
    }

    private Dictionary<string, Pool> m_pools = new Dictionary<string, Pool>();
    private Transform m_root;

    public void Init()
    {
        if (m_root == null)
        {
            m_root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(m_root);
        }
    }

    public void CreatePool(GameObject origin, int count = 5)
    {
        if (m_pools.ContainsKey(origin.name)) return;

        Pool pool = new Pool();
        pool.Init(origin, count);
        pool.Root.parent = m_root;

        m_pools.Add(origin.name, pool);
    }

    public void Push(GameObject obj)
    {
        string name = obj.gameObject.name;
        if (m_pools.ContainsKey(name) == false)
        {
            GameObject.Destroy(obj.gameObject);
            return;
        }
        m_pools[name].Push(obj);
    }

    public GameObject Pop(GameObject origin, Transform parent = null)
    {
        if (m_pools.ContainsKey(origin.name) == false)
        {
            CreatePool(origin);
        }
        return m_pools[origin.name].Pop((Transform)parent);
    }

    public bool IsPoolObject(GameObject origin)
    {
        return m_pools.ContainsKey(origin.name);
    }

    public GameObject GetOriginal(string name)
    {
        if (m_pools.ContainsKey(name) == false)
        {
            return null;
        }
        return m_pools[name].original;
    }

    public void Clear()
    {
        foreach (Transform child in m_root)
        {
            GameObject.Destroy(child.gameObject);
        }
        m_pools.Clear();
    }
}
