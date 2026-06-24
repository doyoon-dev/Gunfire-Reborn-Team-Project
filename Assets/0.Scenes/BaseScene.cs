using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseScene : MonoBehaviour
{
    private bool m_isLoading = false;
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;
    private void Awake()
    {
        gameObject.SetActive(true);
        Screen.SetResolution(1920, 1080, true);
        Begin();
    }
    protected virtual void Begin()
    {
        GameObject obj = GameObject.FindObjectOfType<EventSystem>()?.gameObject;
        if (obj == null)
        {
            obj.name = "@EventSystem";
        }

        LoadGameObject();
        StartCoroutine(Loading());
        //Init();
    }

    IEnumerator Loading()
    {
        Managers.Game.IsReady = true;
        yield return new WaitUntil(() => Managers.Game.IsReady);
        Init();
        yield return null;
        //while (true)
        //{
        //    // 리소스가 다 로딩되었으면 끝
        //    if (Managers.Resource.DataMaxAsyncCount <= Managers.Resource.DataAsyncCount)
        //    {
        //        Managers.Resource.DelPrefab(loading);
        //        Init();
        //        // JJCH ----------------------------------------
        //        if (Managers.Game.IsPlay)
        //        {
        //            Managers.Network.Session.Write((int)E_PROTOCOL.LOAD_COMPLETE);
        //        }
        //        // ---------------------------------------------
        //        m_isLoading = true;
        //        yield break;
        //    }
        //    // 리소스가 로딩중이면
        //    else
        //    {
        //        yield return null;
        //    }
        //}

    }

    protected virtual void LoadGameObject() { }
    protected virtual void Init() { }
    protected virtual void OnUpdate() { }
    //public abstract void Clear();
}
