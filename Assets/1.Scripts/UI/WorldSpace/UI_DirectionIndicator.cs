using System.Collections.Generic;
using UnityEngine;

public enum IndicatorType
{
    Chest,
    NextDirection
}

public class DirectionIndicator
{
    public IndicatorType m_type;
    public GameObject m_indicatorTarget;
}

public class UI_DirectionIndicator : UI_Scene
{
    Camera m_Camera;
    List<DirectionIndicator> m_indicatorTarget = new List<DirectionIndicator>();

    private void Start()
    {
        m_Camera = Camera.main;
    }
    private void Update()
    {
        Vector3 viewPos = m_Camera.WorldToViewportPoint(transform.position);
        if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
        {
            //보일때 처리
        }
        else
        {
            m_Camera.ViewportToWorldPoint(viewPos);

            //안보일때 처리
        }
    }

    public void AddTarget(DirectionIndicator target)
    {
        m_indicatorTarget.Add(target);
    }
}
