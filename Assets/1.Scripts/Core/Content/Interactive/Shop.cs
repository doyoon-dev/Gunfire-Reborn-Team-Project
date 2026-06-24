using System.Collections.Generic;
using UnityEngine;

public class Shop : InteractiveObject
{
    UI_Shop m_ui;
    List<ShopItem> m_itemList = new List<ShopItem>();

    int m_maxRefreshChance = 1;
    int m_curRefreshChance = 0;

    public List<ShopItem> ItemList { get => m_itemList; }
    public int CurRefreshChance => m_curRefreshChance;
    public string RefreshChanceCount { get => m_curRefreshChance + "/" + m_maxRefreshChance; }

    private void Start()
    {
        m_interactiveString = "행상인 방문";
        Init();
        BasicItemCreate();
        
        m_itemList.Add(Managers.Resource.Load<ShopItem>(Define.Path.Dropltem_Gun+ Random.Range(1, 4)));
        m_curRefreshChance = m_maxRefreshChance;

        foreach (ShopItem item in m_itemList)
        {
            item.SoldOut = false;
        }
    }
    private void Update()
    {
            OnUpdate();
    }
    public override void Interactive()
    {
        Managers.Game.IsPopUpUI = true;
        m_ui = Managers.UI.ShowPopupUI<UI_Shop>();
        m_ui.Initialize(this);
    }

    public void BasicItemCreate()
    {
        m_itemList.Add(Managers.Resource.Load<ShopItem>(Define.Path.Shop_Item_Heal));
        m_itemList.Add(Managers.Resource.Load<ShopItem>(Define.Path.Shop_Item_Bullet));
        m_itemList.Add(Managers.Resource.Load<ShopItem>(Define.Path.Shop_Item_Granade));
    }


    public void Refresh()
    {
        if(m_curRefreshChance>0)
        {
            m_curRefreshChance--;

            m_itemList.Clear();
            BasicItemCreate();
            m_itemList.Add(Managers.Resource.Load<ShopItem>(Define.Path.Dropltem_Gun + Random.Range(1, 4)));
           
            m_ui.Initialize(this);
        }
    }
}
