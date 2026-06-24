

using System;

namespace Define
{
    static class Path
    {
        #region Dropltem
        public const string Dropltem_Gold = "Item/Dropltem_Gold";
        public const string Dropltem_Bullet_Green = "Item/Dropltem_Bullet_Green";
        public const string Dropltem_Bullet_Blue = "Item/Dropltem_Bullet_Blue";
        public const string Dropltem_Bullet_Yellow = "Item/Dropltem_Bullet_Yellow";
        public const string Dropltem_Grenade = "Item/Dropltem_Grenade";
        public const string Dropltem_Gun = "Data/ShopItems/Gun";
        #endregion

        #region NPC
        public const string Smith_TestWeaponInfo = "UI/Popup/TestWeaponInfo";

        public const string Shop_Item_Slot = "UI/Part/UI_ShopItemSlot";
        
        public const string Shop_Item_Heal = "Data/ShopItems/Heal";
        public const string Shop_Item_Bullet = "Data/ShopItems/Bullet";
        public const string Shop_Item_Granade = "Data/ShopItems/Granade";
        public const string Shop_Item_TestGun = "Data/ShopItems/TestGun";
        #endregion        
        
        #region Monster
        public const string Monster_bossmodel = "Prefabs/Monster/bossmodel";
        public const string Monster_Monster_Melee_Beatle = "Prefabs/Monster/Monster_Melee_Beatle";
        public const string Monster_Monster_Melee_Spear = "Prefabs/Monster/Monster_Melee_Spear";
        public const string Monster_Monster_Ranged_Crossbow = "Prefabs/Monster/Monster_Ranged_Crossbow";
        #endregion

        #region UI
        public const string UI_Number = "UI/Part/UI_Number";
        public const string TextSprite_Number = "Sprite/Text/";
        public const string TextSprite_Number_NoneOutLine = "Sprite/Text/NoneOutLine/";

        public const string UI_DamageTaxt = "UI/UI_DamageTaxt";

        public const string MiniMapCamera = "MiniMapCamera";
        public const string MiniMapIndicator_Enemy = "UI/Part/MiniMapIndicator";
        public const string MiniMapIndicator_Sprite = "Sprite/UI_MiniMap/arrow_map";

        public const string UI_MonsterAtkDir = "UI/UI_MonsterAtkDir";
        
        public const string Arttext_Win = "Sprite/arttext_Win1";
        public const string Arttext_Fail = "Sprite/arttext_Fail1";
        #endregion

        #region SOUND
        public const string SoundMixer = "SoundMixer";
        public const string SOUND_BGM_Boss1 = "bgm_boss1";
        public const string SOUND_BGM_MainMenu = "bgm_Main Menu";
        public const string SOUND_Effect_Gun = "AutoGun_1p_01";
        public const string SOUND_Effect_Grenade = "Grenade8Short";
        public const string SOUND_Effect_Reload = "ReloadSound";
        public const string SOUND_Effect_ReloadIn = "ReloadInSound";
        public const string SOUND_Effect_ReloadOut = "ReloadOutSound";
        public const string SOUND_Effect_Dash = "Dash";
        #endregion
    }

    public enum Layer
    {
        Ground = 6,
        Monster = 7,
        Player = 8,
        Item,
        MiniMap,
        EnemyBullet,
        Weapon,
        Door
    }

    public enum Scene
    {
        Unknown = -1,
        StartScene,
        gunfire,
        Main,
    }
    public enum UIEvent
    {
        Click,
        Drag,
        Enter,
        Stay,
        Exit,
    }
    public enum MouseEvent
    {
        Press,
        Click,
    }

    public enum Sound
    {
        Master,
        BGM,
        Effect,
        UI,
        MaxCount,
    }

    public enum UserKey
    {
        Forward,
        Backward,
        Right,
        Left,
        Jump,
        ActiveSkill,
        Grenade,
        GunInput1,
        GunInput2,
        Esc,
        End
    }

    public enum Mouse
    {
        PointerDown,
        Click,
        Press,
        PointerUp
    }

    public enum BulletType
    {
        Generalm,
        Heavy,
        Special
    }

    public enum DropItemType
    {
        None = 0,
        Gold,
        Bullet_Green=2,
        Bullet_Blue,
        Bullet_Yellow,
        Grenade,
        SoulEssence,
    }

    public enum ShopItemType
    {
        Heal,
        Bullet,
        Granade,
        Weapon,
        Scroll
    }

    public enum MiniMapObjectType
    {
        Player,
        Monster,
        Boss,
        Shop,
        Smith,
        Door
    }

    [Flags]
    public enum ElementalType
    {
        None =0,
        Fire = 1 << 0,
        Lightning = 1 << 1,
        Corrosion = 1 << 2,//산성
        Combustion = Fire | Corrosion,
        Manipulation = Lightning|Fire,
        Miasma = Lightning | Corrosion,
        MaxCount = int.MaxValue
    }

    public enum TextUIType
    {
        OutLine,
        NoneOutLine,
    }

    public enum ChestType//기본적으로 탄약 및 골드 드랍
    {
        Blue, //고블렛 or 스크롤 1개
        Green,//스크롤 3개 중 1택
        Peculiar,
    }
    public enum TalentGroup
    {
        Expedition,
        Battle,
        Skill,
        Survival,
        Weapon,
        Hero,
    }
}
