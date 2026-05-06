using UnityEngine;
using StarfallCovenant.Data;

namespace StarfallCovenant.Battle
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 3f;

        private Vector2 direction;
        private int abilityDamage;
        private int attackStat;
        private bool isPlayerProjectile;

        public void Initialize(Vector2 direction, int abilityDamage, int attackStat, bool isPlayerProjectile)
        {
            this.direction = direction.normalized;
            this.abilityDamage = abilityDamage;
            this.attackStat = attackStat;
            this.isPlayerProjectile = isPlayerProjectile;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isPlayerProjectile)
            {
                var enemy = other.GetComponent<EnemyController>();
                if (enemy != null && !enemy.IsDead)
                {
                    var result = DamageCalculator.Calculate(attackStat, abilityDamage, new Stats { defense = 0 });
                    enemy.TakeDamage(result.damage);
                    Destroy(gameObject);
                }
            }
            else
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    var result = DamageCalculator.Calculate(attackStat, abilityDamage, new Stats { defense = 0 });
                    player.TakeDamage(result.damage);
                    Destroy(gameObject);
                }
            }
        }
    }
}
