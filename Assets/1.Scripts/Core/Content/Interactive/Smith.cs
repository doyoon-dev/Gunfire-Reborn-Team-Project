using UnityEngine;

public class Smith : InteractiveObject
{
    UI_Smith m_ui;

    int m_maxUpgradeChance = 2;
    [SerializeField] int m_curUpgradeChance = 0;

    public int CurUpgradeChance { get => m_curUpgradeChance; set => m_curUpgradeChance = value; }

    private void Start()
    {
        m_curUpgradeChance = m_maxUpgradeChance;
        m_interactiveString = "대장장이 방문";
        Init();
    }
    private void Update()
    {
        OnUpdate();
    }
    public override void Interactive()
    {
        Managers.Game.IsPopUpUI = true;
        m_ui = Managers.UI.ShowPopupUI<UI_Smith>();
        m_ui.Initialize(this);
    }

    public int GetUpgradePrice(Gun gun)=> gun.m_gunEnforce == 0 ? 50 : 
        (gun.m_gunEnforce>=6 ? 600:100 * gun.m_gunEnforce);
}
