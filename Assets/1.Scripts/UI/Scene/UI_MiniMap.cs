using System.Collections.Generic;
using UnityEngine;

public class UI_MiniMap : UI_Scene
{
    [SerializeField] Transform m_indicatorRoot;
    Camera m_camera;

    List<MiniMapSprite> m_spriteList = new List<MiniMapSprite>();
    List<MiniMapIndicator> m_indicatorList = new List<MiniMapIndicator>();

    float m_indicatorDistance = 120f;
    float m_visibleDistance = 15f;

    float m_miniMapCameraSize = 25f;

    public Camera GetCamera { get { return m_camera; } }

    private void LateUpdate()
    {
        if (Managers.Game.Player != null)
        {
            MoveToPlayer();
            ShowIndicator();
        }
    }

    public override void Init()
    {
        base.Init();
        m_camera = GameObject.Find("MiniMapCamera")?.GetComponent<Camera>();
        if (m_camera == null)
        {
            m_camera = Managers.Resource.Instantiate<Camera>(Define.Path.MiniMapCamera);// Instantiate(Managers.Resource.Load<Camera>(Define.Path.MiniMapCamera), null);
            m_camera.name = "MiniMapCamera";
        }
        m_camera.orthographicSize = m_miniMapCameraSize;
        m_visibleDistance = m_camera.orthographicSize - 2;
    }

    void MoveToPlayer()
    {
        m_camera.transform.position = new Vector3(Managers.Game.Player.transform.position.x, m_camera.transform.position.y, Managers.Game.Player.transform.position.z);
        m_camera.transform.eulerAngles = new Vector3(m_camera.transform.eulerAngles.x, 0, -Managers.Game.Player.transform.eulerAngles.y);
    }

    public void PushMiniMapSprite(MiniMapSprite miniMapSprite)
    {
        m_spriteList.Add(miniMapSprite);
        MiniMapIndicator temp = Managers.Resource.Instantiate(Define.Path.MiniMapIndicator_Enemy, m_indicatorRoot).GetComponent<MiniMapIndicator>();
        temp.Cunnet = miniMapSprite;
        temp.gameObject.SetActive(true);
        m_indicatorList.Add(temp);
    }
    public void PopMiniMapSprite(MiniMapSprite miniMapSprite)
    {
        MiniMapIndicator temp = FindIndicator(miniMapSprite);
        if(temp!=null)
        {
            m_indicatorList.Remove(temp);
            temp.Cunnet = null;
            Managers.Pool.Push(temp.gameObject);
        }
        miniMapSprite.gameObject.SetActive(false);
        m_spriteList.Remove(miniMapSprite);
    }

    public void ShowIndicator()
    {
        Vector3 direction = Vector3.zero;
        float distance;
        foreach (var item in m_spriteList)
        {
            MiniMapIndicator indicator = FindIndicator(item);
            if (indicator == null) return;

            direction = new Vector2(item.transform.position.x, item.transform.position.z) - new Vector2(m_camera.transform.position.x, m_camera.transform.position.z);
            distance = direction.magnitude;

            if (distance >= m_visibleDistance)
            {
                float Angle = m_camera.transform.eulerAngles.y;
                if (indicator != null)
                {
                    indicator.transform.localPosition = Quaternion.AngleAxis(Angle, Vector3.forward) * direction.normalized * m_indicatorDistance;
                    float axis = Vector3.SignedAngle(Vector3.up, direction.normalized, Vector3.forward);
                    indicator.transform.localEulerAngles = new Vector3(0, 0, m_camera.transform.eulerAngles.y + axis);
                }

                if (indicator.gameObject.activeSelf == false ||
                    item.gameObject.activeSelf == true)
                {
                    indicator.SetIndicatorImage();
                    item.gameObject.SetActive(false);
                    indicator.gameObject.SetActive(true);
                }
            }
            else
            {
                if (indicator.gameObject.activeSelf == true ||
                    item.gameObject.activeSelf == false)
                {
                    item.gameObject.SetActive(true);
                    indicator.gameObject.SetActive(false);
                }
            }
        }
    }

    MiniMapIndicator FindIndicator(MiniMapSprite sprite)
    {
        foreach (var item in m_indicatorList)
        {
            if (item.Cunnet == sprite) return item;
        }
        return null;
    }
}
