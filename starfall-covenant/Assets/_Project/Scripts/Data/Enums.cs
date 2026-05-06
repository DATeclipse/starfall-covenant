namespace StarfallCovenant.Data
{
    public enum Rarity
    {
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
        Mythic = 5
    }

    public enum Race
    {
        Dragon,
        Elf,
        Daemon,
        Yokai,
        SpaceBeast
    }

    public enum AbilityType
    {
        BasicAttack,
        Dodge,
        Special,
        Ultimate
    }

    public enum BattleState
    {
        Intro,
        Active,
        Victory,
        Defeat,
        Paused
    }

    public enum EnemyState
    {
        Idle,
        Telegraph,
        Attack,
        Cooldown,
        Stunned,
        Dead
    }
}
