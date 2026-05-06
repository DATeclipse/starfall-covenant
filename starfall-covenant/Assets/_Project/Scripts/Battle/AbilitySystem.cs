using UnityEngine;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Battle
{
    public class AbilitySystem : MonoBehaviour
    {
        private PlayerController player;
        private Camera mainCamera;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            mainCamera = Camera.main;
        }

        public void UseAbility(int index)
        {
            Vector2 aimDirection = GetAimDirection();
            player.TryUseAbility(index, aimDirection);
        }

        public void Dodge()
        {
            Vector2 dodgeDirection = GetAimDirection();
            if (dodgeDirection == Vector2.zero)
                dodgeDirection = Vector2.right;

            player.TryDodge(dodgeDirection);
        }

        private Vector2 GetAimDirection()
        {
            if (Input.touchCount > 0)
            {
                Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
                return ((Vector2)worldPos - (Vector2)transform.position).normalized;
            }

            return transform.right;
        }
    }
}
