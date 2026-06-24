using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : UI_Popup
{
    Shop m_shop;

    [SerializeField] UI_NumberString m_refreshString;
    [SerializeField] UI_NumberString m_goldString;
    [SerializeField] Transform m_itemSlotRoot;
    List<UI_ShopItemSlot> m_itemList = new List<UI_ShopItemSlot>();
    [SerializeField] Button m_refreshButton;
    [SerializeField] UI_ShopItemInfo m_itemInfo;
    [SerializeField] WeaponInfo m_weaponnInfo;

    [SerializeField] GameObject m_buyable;
    [SerializeField] GameObject m_unbuyable;

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI(this);
            Managers.Game.IsPopUpUI = false;
        }
    }

    public void Initialize(Shop peddier)
    {
        Clear();
        m_shop = peddier;
        m_itemInfo.gameObject.SetActive(false);
        m_weaponnInfo.gameObject.SetActive(false);

        int count = m_shop.ItemList.Count;
        for (int i = 0; i < count; i++)
        {
            UI_ShopItemSlot temp = Managers.Resource.Instantiate(Define.Path.Shop_Item_Slot).GetComponent<UI_ShopItemSlot>();
            temp.Init(this, m_shop.ItemList[i], m_itemSlotRoot);
            m_itemList.Add(temp);
        }
        UpdateAllSlot();
        m_refreshString.ChangeString(m_shop.RefreshChanceCount);
        m_refreshButton.interactable = m_shop.CurRefreshChance <= 0 ? false : true;
    }

    public void ShowItemInfo(ShopItem item, bool show)
    {
        ShowBuyable(show, item.Price);

        if (item.Type==Define.ShopItemType.Weapon)
        {
            m_weaponnInfo.gameObject.SetActive(show);
            if (show)  m_weaponnInfo.ShowGunText(item.Gun); 
        }
        else
        {
            m_itemInfo.gameObject.SetActive(show);
            if (show) m_itemInfo.SetInfo(item);
        }

    }

    public void ShowBuyable(bool show,int price)
    {
        if (show == false)
        {
            m_buyable.SetActive(show);
            m_unbuyable.SetActive(show);
        }
        else if (Managers.Game.Gold < price)
        {
            m_buyable.SetActive(false);
            m_unbuyable.SetActive(true);
        }
        else
        {
            m_buyable.SetActive(true);
            m_unbuyable.SetActive(false);
        }
    }

    public void UpdateAllSlot()
    {
        foreach (var item in m_itemList)
        {
            item.UpdateItemState();
        }
        m_goldString.ChangeNumber(Managers.Game.Gold);
    }
    

    public void TryToBuyItem(UI_ShopItemSlot slot)
    {
        if (slot.CheckBuyable())
        {
            Managers.Game.Gold -= slot.Item.Price;
            ShowItemInfo(slot.Item, true);
            slot.Item.UseItem();
            slot.SoldOut = true;
            UpdateAllSlot();
        }
    }

    public void OnClickRefresh() 
    { 
        m_shop.Refresh();
        int count = m_shop.ItemList.Count;
    }

    public void Clear()
    {
        foreach (var item in m_itemList)
        {
            item.Refresh();
            Managers.Pool.Push(item.gameObject);
        }
        m_itemList.Clear();
    }
}