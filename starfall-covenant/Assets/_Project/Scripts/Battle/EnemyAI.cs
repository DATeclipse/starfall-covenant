using UnityEngine;
using StarfallCovenant.Data;

namespace StarfallCovenant.Battle
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private float telegraphDuration = 0.5f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float attackRange = 2f;

        private EnemyController controller;
        private EnemyState state;
        private float stateTimer;
        private float attackInterval;
        private float attackTimer;
        private Transform playerTarget;

        public EnemyState CurrentState => state;

        public void Initialize(EnemyController controller, float attackInterval)
        {
            this.controller = controller;
            this.attackInterval = attackInterval;
            state = EnemyState.Idle;
            attackTimer = attackInterval;
            playerTarget = FindAnyObjectByType<PlayerController>()?.transform;
        }

        private void Update()
        {
            if (controller.IsDead || playerTarget == null) return;

            switch (state)
            {
                case EnemyState.Idle:
                    UpdateIdle();
                    break;
                case EnemyState.Telegraph:
                    UpdateTelegraph();
                    break;
                case EnemyState.Attack:
                    UpdateAttack();
                    break;
                case EnemyState.Cooldown:
                    UpdateCooldown();
                    break;
            }
        }

        private void UpdateIdle()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer > attackRange)
            {
                Vector2 direction = (playerTarget.position - transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }

            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0 && distanceToPlayer <= attackRange)
            {
                TransitionTo(EnemyState.Telegraph);
            }
        }

        private void UpdateTelegraph()
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                TransitionTo(EnemyState.Attack);
            }
        }

        private void UpdateAttack()
        {
            controller.PerformAttack(playerTarget);
            TransitionTo(EnemyState.Cooldown);
        }

        private void UpdateCooldown()
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                TransitionTo(EnemyState.Idle);
            }
        }

        private void TransitionTo(EnemyState newState)
        {
            state = newState;
            switch (newState)
            {
                case EnemyState.Telegraph:
                    stateTimer = telegraphDuration;
                    break;
                case EnemyState.Cooldown:
                    stateTimer = attackInterval * 0.5f;
                    attackTimer = attackInterval;
                    break;
            }
        }
    }
}
