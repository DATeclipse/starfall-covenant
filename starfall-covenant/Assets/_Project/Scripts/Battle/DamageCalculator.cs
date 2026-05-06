using StarfallCovenant.Data;

namespace StarfallCovenant.Battle
{
    public static class DamageCalculator
    {
        private const float DEFENSE_SCALAR = 100f;
        private const float CRIT_CHANCE = 0.15f;
        private const float CRIT_MULTIPLIER = 1.5f;
        private const float VARIANCE = 0.1f;

        public static DamageResult Calculate(int attackPower, int abilityDamage, Stats defenderStats)
        {
            float rawDamage = attackPower + abilityDamage;
            float defenseReduction = DEFENSE_SCALAR / (DEFENSE_SCALAR + defenderStats.defense);
            float damage = rawDamage * defenseReduction;

            bool isCrit = UnityEngine.Random.value < CRIT_CHANCE;
            if (isCrit)
            {
                damage *= CRIT_MULTIPLIER;
            }

            float variance = 1f + UnityEngine.Random.Range(-VARIANCE, VARIANCE);
            damage *= variance;

            int finalDamage = UnityEngine.Mathf.Max(1, UnityEngine.Mathf.RoundToInt(damage));

            return new DamageResult
            {
                damage = finalDamage,
                isCritical = isCrit
            };
        }
    }

    public struct DamageResult
    {
        public int damage;
        public bool isCritical;
    }
}
