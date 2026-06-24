using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopItemSlot : MonoBehaviour, IPointerClickHandler,IPointerEnterHandler, IPointerExitHandler
{
    ShopItem m_item;
    [SerializeField] Image m_selectedImg;
    [SerializeField] Image m_itemImage;
    [SerializeField] UI_NumberString m_numberString;
    [SerializeField] Image m_blurPanel;
    [SerializeField] Image m_soldOutImage;
    bool m_isAffordable = false;//살 수 있는가 = 돈이 되는가

    UI_Shop m_shop;

    public ShopItem Item { get =>m_item;  }
    public bool SoldOut
    {
        get => m_item.SoldOut;
        set
        {
            m_item.SoldOut = value;
            if(m_item.SoldOut)
            {
                Color color = Color.black;
                color.a = 0.5f;
                m_blurPanel.color = color;
                m_soldOutImage.gameObject.SetActive(true);
            }
            else
            {
                m_blurPanel.color = Color.clear;
                m_soldOutImage.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        if (m_numberString == null) m_numberString = Util.FindChild<UI_NumberString>(gameObject);
    }

    public bool CheckBuyable()
    {
        return m_item.SoldOut ==false && m_item.IsAffordable;
    }

    public void UpdateItemState()
    {
        if (m_item.SoldOut)
        {
            Color color = Color.black;
            color.a = 0.5f;
            m_blurPanel.color = color;
            m_soldOutImage.gameObject.SetActive(true);
        }
        else if(!m_item.IsAffordable)
        {
            Color color = Color.red;
            color.a = 0.2f;
            m_blurPanel.color = color;
            m_numberString.SetStringColor(Color.red);
            m_soldOutImage.gameObject.SetActive(false);
        }
        else
        {
            m_blurPanel.color = Color.clear;
            m_numberString.SetStringColor(Color.white);
            m_soldOutImage.gameObject.SetActive(false);
        }
    }

    public bool CheckAffordable()
    {
        if (SoldOut) return false;
        m_isAffordable = m_item.IsAffordable;

        if (m_isAffordable)
        {
            m_blurPanel.color = Color.clear;
            m_numberString.SetStringColor(Color.white);
        }
        else
        {
            Color color = Color.red;
            color.a = 0.2f;
            m_blurPanel.color = color;
            m_numberString.SetStringColor(Color.red);
        }

        return m_isAffordable;
    }

    public void Init(UI_Shop uI_Peddler, ShopItem item, Transform parent)
    {
        m_selectedImg.gameObject.SetActive(false );
        m_shop = uI_Peddler;
        transform.parent = parent;
        SetItem(item);
    }

    void SetItem(ShopItem item)
    {
        m_item = item;
        if(item.Gun==null) m_itemImage.sprite = m_item.Icon;
        else m_itemImage.sprite = m_item.Gun.m_currentGunImage.sprite;
        m_numberString.ChangeNumber(m_item.Price);
    }

    public void Refresh()
    {
        if(m_item.SoldOut)
        {
            m_item.SoldOut = false;
            m_numberString.ChangeNumber(m_item.Price);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_shop.TryToBuyItem(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_shop.ShowItemInfo(m_item, true);
        if (CheckBuyable())
        {
            m_selectedImg.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_shop.ShowItemInfo(m_item, false);
        if (CheckBuyable())
        {
            m_selectedImg.gameObject.SetActive(false);
        }
    }
}
