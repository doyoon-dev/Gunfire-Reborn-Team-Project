using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager 
{
    int m_order = 10;

    Stack<UI_Popup> m_stackPopup = new Stack<UI_Popup>();
    List<UI_Scene> m_sceneUIList = new List<UI_Scene>();

    GameObject m_root;
    public GameObject Root 
    {
        get 
        {
            if (m_root == null) m_root = new GameObject { name = "@UI_Root" };
            return m_root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)  canvas.sortingOrder = m_order++; 
        else  canvas.sortingOrder = 0; 
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        go.transform.SetParent(Root.transform);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
        go.transform.SetParent(Root.transform);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowUI<T>(string name = null) where T : Object
    {
        if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

        T go = Managers.Resource.Instantiate<T>($"UI/{name}", Root.transform);
        return go;
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        m_sceneUIList.Add(sceneUI);
        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name)) { name = typeof(T).Name; }

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        m_stackPopup.Push(popup);
        go.transform.SetParent(Root.transform);

        return popup;
    }
    public void CloseUI<T>(T ui) where T : Object
    {
        Managers.Resource.Destroy(ui.GameObject());
    }

    public void CloseSceneUI(UI_Scene scene)
    {
        m_sceneUIList.Remove(scene);
        Managers.Resource.Destroy(scene.gameObject);
    }

    public void CloseAllSceneUI()
    {
        foreach (var item in m_sceneUIList)
        {
            Managers.Resource.Destroy(item.gameObject);
        }
        m_sceneUIList.Clear();
    } 
    
    //public void SetActiveAllSceneUI(bool flag)
    //{
    //    Managers.Game.IsPopUpUI = !flag;
    //    //foreach (var item in m_sceneUIList)
    //    //{
    //    //    item.gameObject.SetActive(flag);
    //    //}
    //}

    public void ClosePopupUI(UI_Popup popup)
    {
        if (m_stackPopup.Count == 0) { return; }
        if (m_stackPopup.Peek() != popup) 
        {
            Debug.LogError("Close Popup Failed!");
            return;
        }
        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (m_stackPopup.Count == 0) {
            return;
        }

        UI_Popup popup = m_stackPopup.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        m_order--;
    }

    public void CloseAllPopupUI()
    {
        while (m_stackPopup.Count > 0) {
            ClosePopupUI();
        }
    }

    public void Clear()
    {
        CloseAllPopupUI();
        m_order = 10;
    }
}