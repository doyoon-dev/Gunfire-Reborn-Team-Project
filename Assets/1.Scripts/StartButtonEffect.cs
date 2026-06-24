using UnityEngine;

public class StartButtonEffect : MonoBehaviour
{
    public Animator m_animator;
    public UnityEngine.UI.Toggle m_toggle;    
    void OnEnable()
    {
        m_animator = GetComponent<Animator>();
        m_toggle = GetComponentInParent<UnityEngine.UI.Toggle>();      
    } 

    void Update()
    {
        if (m_animator == null) m_animator = GetComponent<Animator>();
        if (m_toggle.isOn == true)
            m_animator.SetTrigger("CheckMarkEffectTrig");
        else
            m_animator.SetTrigger("CheckMarkEffectOffTrig");
    }
}
