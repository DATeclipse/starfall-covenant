using UnityEngine;
using StarfallCovenant.Data;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Battle
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float dodgeSpeed = 12f;
        [SerializeField] private float dodgeDuration = 0.3f;
        [SerializeField] private float dodgeCooldown = 1f;
        [SerializeField] private float iFrameDuration = 0.25f;

        private Stats currentStats;
        private int currentHp;
        private Animator animator;
        private Rigidbody2D rb;
        private bool isDodging;
        private bool isInvincible;
        private float dodgeCooldownTimer;
        private float[] abilityCooldowns;
        private AbilitySO[] abilities;

        public int CurrentHp => currentHp;
        public int MaxHp => currentStats.hp;
        public bool IsInvincible => isInvincible;
        public float[] AbilityCooldowns => abilityCooldowns;

        public System.Action<int, int> OnHpChanged;
        public System.Action OnDeath;

        public void Initialize(CharacterSO template, int level)
        {
            currentStats = template.baseStats.ScaledByLevel(level);
            currentHp = currentStats.hp;
            abilities = template.abilities;
            abilityCooldowns = new float[abilities.Length];
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();

            if (template.animatorController != null)
            {
                animator.runtimeAnimatorController = template.animatorController;
            }
        }

        private void Update()
        {
            if (isDodging) return;

            HandleMovement();
            UpdateCooldowns();

            if (dodgeCooldownTimer > 0)
                dodgeCooldownTimer -= Time.deltaTime;
        }

        private void HandleMovement()
        {
            Vector2 input = Vector2.zero;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                    Vector2 direction = ((Vector2)worldPos - rb.position).normalized;
                    input = direction;
                }
            }

            rb.linearVelocity = input * moveSpeed;
            animator.SetFloat("Speed", input.magnitude);
        }

        private void UpdateCooldowns()
        {
            for (int i = 0; i < abilityCooldowns.Length; i++)
            {
                if (abilityCooldowns[i] > 0)
                    abilityCooldowns[i] -= Time.deltaTime;
            }
        }

        public void TryDodge(Vector2 direction)
        {
            if (isDodging || dodgeCooldownTimer > 0) return;

            StartCoroutine(DodgeRoutine(direction));
        }

        private System.Collections.IEnumerator DodgeRoutine(Vector2 direction)
        {
            isDodging = true;
            isInvincible = true;
            animator.SetTrigger("Dodge");

            rb.linearVelocity = direction.normalized * dodgeSpeed;

            yield return new WaitForSeconds(iFrameDuration);
            isInvincible = false;

            yield return new WaitForSeconds(dodgeDuration - iFrameDuration);
            rb.linearVelocity = Vector2.zero;
            isDodging = false;
            dodgeCooldownTimer = dodgeCooldown;
        }

        public bool TryUseAbility(int index, Vector2 targetDirection)
        {
            if (index >= abilities.Length || abilityCooldowns[index] > 0) return false;

            AbilitySO ability = abilities[index];
            abilityCooldowns[index] = ability.cooldown;
            animator.SetTrigger("Attack");

            BattleManager.Instance.SpawnAbilityEffect(
                transform.position,
                targetDirection,
                ability,
                currentStats.attack
            );

            return true;
        }

        public void TakeDamage(int amount)
        {
            if (isInvincible) return;

            currentHp = Mathf.Max(0, currentHp - amount);
            animator.SetTrigger("Hurt");
            OnHpChanged?.Invoke(currentHp, currentStats.hp);

            if (currentHp <= 0)
            {
                animator.SetTrigger("Death");
                OnDeath?.Invoke();
            }
        }
    }
}
