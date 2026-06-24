using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI m_current = null;

    public InventoryCenter m_inventoryCenter;

    public GameObject m_inventoryBg;
    GameObject m_top;
    RectTransform[] m_ascensionBtn;
    RectTransform[] m_weaponBtn;
    RectTransform[] m_scrollBtn;
    Color m_initColor = new Color32(204, 141, 0, 255);
    Color m_colorAlpha = new Color32(204, 141, 0, 80);
    // Start is called before the first frame update
    void Awake()
    {
        if (m_current == null)
        {
            m_current = this;
        }
        else
        {
            Debug.LogError("Not Single InventoryUI");
            Destroy(gameObject);
        }

        m_inventoryBg = transform.GetChild(0).gameObject;
        m_top = m_inventoryBg.transform.GetChild(0).gameObject;
        m_ascensionBtn = m_top.transform.Find("Ascension").GetComponentsInChildren<RectTransform>();
        m_weaponBtn = m_top.transform.Find("Weapon").GetComponentsInChildren<RectTransform>();
        m_scrollBtn = m_top.transform.Find("Scroll").GetComponentsInChildren<RectTransform>();

        InitSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TextColor(Color weapon, Color ascension, Color scroll)
    {
        m_weaponBtn[1].gameObject.GetComponent<UnityEngine.UI.Text>().color = weapon;
        m_ascensionBtn[1].gameObject.GetComponent<UnityEngine.UI.Text>().color = ascension;
        m_scrollBtn[1].gameObject.GetComponent<UnityEngine.UI.Text>().color = scroll;
    }

    void SelectedBarActive(bool weapon, bool ascension, bool scroll)
    {
        m_weaponBtn[2].gameObject.SetActive(weapon);
        m_ascensionBtn[2].gameObject.SetActive(ascension);
        m_scrollBtn[2].gameObject.SetActive(scroll);
    }

    void InitSet()
    {
        TextColor(m_initColor, m_colorAlpha, m_colorAlpha);
        SelectedBarActive(true, false, false);
    }

    public void ClickButton(int number)
    {
        if (number == 1)
        {
            TextColor(m_initColor, m_colorAlpha, m_colorAlpha);
            SelectedBarActive(true, false, false);
        }
        else if (number == 2)
        {
            TextColor(m_colorAlpha, m_initColor, m_colorAlpha);
            SelectedBarActive(false, true, false);
        }
        else
        {
            TextColor(m_colorAlpha, m_colorAlpha, m_initColor);
            SelectedBarActive(false, false, true);
        }
    }
}
