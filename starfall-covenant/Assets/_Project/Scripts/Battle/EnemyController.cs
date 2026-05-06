using UnityEngine;
using StarfallCovenant.Data;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Battle
{
    public class EnemyController : MonoBehaviour
    {
        private Stats stats;
        private int currentHp;
        private bool isBoss;
        private Animator animator;
        private EnemyAI ai;
        private AbilitySO[] abilities;

        public int CurrentHp => currentHp;
        public int MaxHp => stats.hp;
        public bool IsBoss => isBoss;
        public bool IsDead => currentHp <= 0;

        public System.Action<int, int> OnHpChanged;
        public System.Action<EnemyController> OnDeath;

        public void Initialize(EnemyConfig config)
        {
            stats = config.stats;
            currentHp = stats.hp;
            isBoss = config.isBoss;
            abilities = config.abilities;
            animator = GetComponent<Animator>();

            if (config.animator != null)
            {
                animator.runtimeAnimatorController = config.animator;
            }

            ai = GetComponent<EnemyAI>();
            ai.Initialize(this, config.attackInterval);
        }

        public void PerformAttack(Transform target)
        {
            if (abilities == null || abilities.Length == 0) return;

            AbilitySO ability = abilities[Random.Range(0, abilities.Length)];
            Vector2 direction = (target.position - transform.position).normalized;

            animator.SetTrigger("Attack");
            BattleManager.Instance.SpawnEnemyAbilityEffect(
                transform.position,
                direction,
                ability,
                stats.attack
            );
        }

        public void TakeDamage(int amount)
        {
            currentHp = Mathf.Max(0, currentHp - amount);
            animator.SetTrigger("Hurt");
            OnHpChanged?.Invoke(currentHp, stats.hp);

            if (currentHp <= 0)
            {
                animator.SetTrigger("Death");
                OnDeath?.Invoke(this);
            }
        }
    }
}
