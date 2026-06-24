using UnityEngine;

public class ResourceManager
{
    //public T Load<T>(string path) where T : Object
    //{
    //    return Resources.Load<T>(path);
    //}
    public T Load<T>(string _path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = _path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }
            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
            {
                return go as T;
            }
        }
        return Resources.Load<T>(_path);
    }
    public GameObject NewPrefab(string _path, Transform _parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{_path}");
        if (original == null)
        {
            Debug.LogError($"Failed to Load prefab : {_path}");
            return null;
        }
        if (Managers.Pool.IsPoolObject(original))
        {
            return Managers.Pool.Pop(original, _parent).gameObject;
        }

        GameObject go = Object.Instantiate(original, _parent);
        go.name = original.name;
        return go;
    }
    public void DelPrefab(GameObject _go)
    {
        if (_go == null)
        {
            return;
        }

        if (Managers.Pool.IsPoolObject(_go))
        {
            Managers.Pool.Push(_go);
            return;
        }

        Object.Destroy(_go);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject obj =Managers.Pool.GetOriginal($"Prefabs/{path}");

        if(obj==null)
        {
            obj = Load<GameObject>($"Prefabs/{path}");
            if (obj == null)
            {
                Debug.LogError($"Filed to load prefab : {path}");
                return null;
            }
        }

        return Managers.Pool.Pop(obj, parent);
    }    
    
    public T Instantiate<T>(string path, Transform parent = null) where T : Object
    {
        T prefab = Load<T>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.LogError($"Filed to load prefab : {path}");
            return null;
        }

        return Object.Instantiate<T>(prefab, parent);
    }

    public void Destroy(GameObject go, float time = 0.0f)
    {
        if (go == null) return;
        Object.Destroy(go, time);
    }
}
