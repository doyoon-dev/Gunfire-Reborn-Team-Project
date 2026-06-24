using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    protected UI_Interactive m_interactiveUI;
    protected string m_interactiveString="작동";
    float m_interactiveDistance = 10f;

    bool m_inDistance = false;
    bool m_inFocus = false;

    [SerializeField]bool m_isDisposable = true;

    public bool IsInteractive { get { return m_inDistance && m_inFocus; } }


    protected void OnUpdate()
    {
        if (Managers.Game?.IsPlay != true) return;

        if (Vector3.Distance(transform.position, Managers.Game.Player.transform.position) <= m_interactiveDistance) m_inDistance = true;
        else m_inDistance = false;

        if(IsInteractive&& Managers.Game.IsPlay&& !Managers.Game.IsPopUpUI)
        {
            if (m_interactiveUI == null) m_interactiveUI = Managers.UI.ShowUI<UI_Interactive>("UI_TestInteractive"); 
            m_interactiveUI.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.F))
            {
                Interactive();
            }
        }
        else
        {
            m_interactiveUI.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        m_interactiveUI = Managers.UI.ShowUI<UI_Interactive>("UI_TestInteractive");
        m_interactiveUI.SetText(m_interactiveString);
        m_interactiveUI.gameObject.SetActive(false);
    }

    public abstract void Interactive();

    protected void Clear()
    {
        if (m_interactiveUI != null)
        Managers.UI.CloseUI(m_interactiveUI);
        enabled = false;
    }

    private void OnMouseEnter()=>m_inFocus = true;
    private void OnMouseExit()=> m_inFocus = false;
}
