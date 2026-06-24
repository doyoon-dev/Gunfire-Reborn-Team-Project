using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{   
    Monster m_monster;

    [SerializeField]
    public UnityEngine.UI.Text m_textMonsterName;

    [SerializeField]
    UnityEngine.UI.Text[] m_text;
    [SerializeField]
    UnityEngine.UI.Image[] m_hpBar;

    public void HpSet(Monster monhp)
    {
        m_monster = monhp;
        
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        //Transform nameTextTransform = transform.Find("name_text");
        //if (nameTextTransform != null)
        //{
        //    UnityEngine.UI.Text textComponent = nameTextTransform.GetComponent<UnityEngine.UI.Text>();
        //    if (textComponent != null)
        //    {
        //        m_textMonsterName = textComponent;
        //        m_textMonsterName.text = m_monster.m_monsterNameView;
        //    }
        //}
    }
        
    void Update()
    {
        if(m_monster != null)
        {            
            if (m_monster.m_isBoss != true)
            {                
                UpdateHPBar(m_monster.m_status.m_hp, m_monster.m_maxhp, 0);                
                if (m_monster.m_maxdef != 0)
                {                    
                    UpdateHPBar(m_monster.m_status.m_def, m_monster.m_maxdef, 1);                    
                }
            }
            else if (m_monster.m_isBoss == true)
            {                
                UpdateHPBar(m_monster.m_status.m_hp, m_monster.m_maxhp, 0);                
                m_text[1].text = m_monster.m_status.m_hp + " / " + m_monster.m_maxhp;                
                if (m_monster.m_maxdef != 0)
                {                    
                    UpdateHPBar(m_monster.m_status.m_def, m_monster.m_maxdef, 1);                    
                    m_text[2].text = m_monster.m_status.m_def + " / " + m_monster.m_maxdef;                    
                }
            }
        }                
    }
    void UpdateHPBar(float current, float max, int index)
    {
        if (index == 0)
        {                      
            m_hpBar[index].fillAmount = current / max;            
            StartCoroutine(DelayedFillAmount(m_hpBar[index + 2], current / max, 0.2f));
        }
        else if (index == 1)
        {                      
            m_hpBar[index].fillAmount = current / max;            
            StartCoroutine(DelayedFillAmount(m_hpBar[index + 2], current / max, 0.2f));
        }
    }
    IEnumerator DelayedFillAmount(UnityEngine.UI.Image image, float fillAmount, float delay)
    {
        yield return new WaitForSeconds(delay);
        image.fillAmount = fillAmount;
    }
}
