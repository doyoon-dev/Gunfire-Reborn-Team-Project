using UnityEngine;

public class GameManager
{
    PlayerController m_player;

    const int m_basicGold = 300;
    int m_gold = 0;

    float m_startTime = 0;
    float m_curruntTime = 0;

    bool m_isReady = false;
    bool m_isPlay = false;
    bool m_isPopUpUI = false;

    UI_MiniMap m_miniMap;
    UI_Wallet m_uiWallet;

    private ItemManager m_item = null;
    private MonsterManager m_monster = null;
    private RoomManager m_room = null;
    public ItemManager Item { get => m_item; }
    public MonsterManager Monster { get => m_monster; }
    public RoomManager Room { get => m_room; }

    public PlayerController Player { get => m_player; }
    public UI_MiniMap MiniMap { get => m_miniMap; set => m_miniMap = value; }

    public bool IsReady { get => m_isReady; set => m_isReady = value; }
    public bool IsPlay { get => m_isPlay; set => m_isPlay = value; }
    public bool IsPopUpUI { get => m_isPopUpUI; set => m_isPopUpUI = value; }

    public float GetGameTime { get=>m_curruntTime-m_startTime; }

    public int Gold { get => m_gold; set { m_gold = value; m_uiWallet.ChangeCount(m_gold); } }

    public void Init() { }
    public void OnUpdate()
    {
        if(m_isPlay)
        {
            m_monster.OnUpdate();
            m_curruntTime = Time.time;
        }
        if (IsPopUpUI)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            if (m_isPlay)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    public void StartGame()
    {
        m_item = Util.FindGetOrAddGameObject<ItemManager>("ItemManager");
        m_monster = Util.FindGetOrAddGameObject<MonsterManager>("MonsterManager");
        m_room = Util.FindGetOrAddGameObject<RoomManager>("RoomManager");

        m_monster.Init();
        m_room.Init();

        m_miniMap = Managers.UI.ShowSceneUI<UI_MiniMap>();
        m_uiWallet= Managers.UI.ShowSceneUI<UI_Wallet>();
        m_gold = m_basicGold;
        m_uiWallet.ChangeCount(m_gold);

        m_player = Util.FindGetOrAddGameObject<PlayerController>("Player");
        m_startTime = Time.time;

        Managers.Game.IsPlay = true;
        m_isPopUpUI = false;
    }
    public void PickUpItem(DropItem item)
    {
        switch (item.Type)
        {
            case Define.DropItemType.None:
                Debug.LogError($"{item.name} type is None");
                break;
            case Define.DropItemType.Gold:
                Gold += 5;
                break;
            case Define.DropItemType.Bullet_Green:
            case Define.DropItemType.Bullet_Blue:
            case Define.DropItemType.Bullet_Yellow:
                WeaponManager.m_current.GetBulletItem(item.Type);
                break;
            case Define.DropItemType.Grenade:
                WeaponManager.m_current.PickUPGrenade(1);
                break;
        }
    }
    public void GmaeEnd()
    {
        Managers.UI.ShowPopupUI<UI_GameResult>();
        m_isPopUpUI = true;
        m_isPlay = false;
    }
    public void Clear()
    {
        Managers.Game.IsPlay = false;
    }
}
