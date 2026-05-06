using System;

namespace StarfallCovenant.Data
{
    [Serializable]
    public class Stats
    {
        public int hp;
        public int attack;
        public int defense;
        public int speed;
        public int specialPower;

        public Stats() { }

        public Stats(int hp, int attack, int defense, int speed, int specialPower)
        {
            this.hp = hp;
            this.attack = attack;
            this.defense = defense;
            this.speed = speed;
            this.specialPower = specialPower;
        }

        public Stats ScaledByLevel(int level)
        {
            float multiplier = 1f + (level - 1) * 0.1f;
            return new Stats(
                (int)(hp * multiplier),
                (int)(attack * multiplier),
                (int)(defense * multiplier),
                (int)(speed * multiplier),
                (int)(specialPower * multiplier)
            );
        }
    }
}
