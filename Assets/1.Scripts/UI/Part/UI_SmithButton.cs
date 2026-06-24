using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SmithButton : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    [SerializeField] int m_index=-1;
    [SerializeField] Text m_buttonText;
    [SerializeField] Text m_descriptText;
    [SerializeField] UI_NumberString m_numberString;

    Button m_button;
    UI_Smith m_smith;
    bool m_isOnMouse;
    private void Update()
    {
    }

    public void Init(UI_Smith smith)
    {
        m_smith = smith;
        m_button =GetComponent<Button>();
        m_button.interactable = m_smith.IsPossibleUpgrade(m_index);
    }

    public void UpdateState(int upgradeChance, int price)
    {
        m_numberString.ChangeNumber(price);

        if (upgradeChance <= 0)//남은 강화 횟수 0
        {
            m_numberString.gameObject.SetActive(false);
            m_buttonText.transform.localPosition = Vector3.zero;
            //블러처리
            return;
        }
        else if(Managers.Game.Gold < price)
        {
            m_numberString.SetStringColor(Color.red);
            //블러처리
            return;
        }
        else
        {
            m_numberString.gameObject.SetActive(true);
            m_numberString.SetStringColor(Color.white);
        }
    }


    public void OnClickButton() 
    { 
        if (m_index == -1) Debug.LogError("UI_SmithButton not set index :" + name); 
        m_smith.TryUpgradeGun(m_index); 
    }

    public void OnPointerEnter(PointerEventData eventData) => m_smith.ShowGunInfo(m_index, true);
    public void OnPointerExit(PointerEventData eventData) => m_smith.ShowGunInfo(m_index, false);
}
