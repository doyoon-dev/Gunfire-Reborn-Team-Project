using Define;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem Data", menuName = "New ShopItem", order = int.MaxValue)]
public class ShopItem : ScriptableObject
{
    [SerializeField] ShopItemType m_type;
    [SerializeField] Sprite m_icon;
    [SerializeField] string m_name;
    [SerializeField] string m_note;
    [SerializeField] int m_price;
    [SerializeField] Gun m_gun;

    bool m_isSoldOut = false;

    public ShopItemType Type => m_type; 
    public Sprite Icon => m_icon;
    public string Name => m_name;
    public string Note => m_note;
    public int Price => m_price;
    public Gun Gun => m_gun;
    public bool IsAffordable { get => Managers.Game.Gold >= m_price; }

    public bool SoldOut { get => m_isSoldOut; set=>m_isSoldOut = value; }

    public void UseItem()
    {
        switch (m_type)
        {
            case ShopItemType.Heal:
                Managers.Game.Player.Heal();
                break;
            case ShopItemType.Bullet:
                WeaponManager.m_current.FullBullet();
                break;
            case ShopItemType.Granade:
                WeaponManager.m_current.FullGrenade();
                break;
            case ShopItemType.Weapon:
                
                WeaponManager.m_current.DropWeaponChange(Instantiate(m_gun.gameObject));
                //총 사면 인벤에 넣는 함수
                break;
            case ShopItemType.Scroll:
                break;
        }
    }

    public void Refresh()
    {
        m_isSoldOut = false;
    }
}
