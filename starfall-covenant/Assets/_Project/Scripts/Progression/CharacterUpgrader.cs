using UnityEngine;
using StarfallCovenant.Core;
using StarfallCovenant.Data;

namespace StarfallCovenant.Progression
{
    public class CharacterUpgrader : MonoBehaviour
    {
        private const int BASE_UPGRADE_COST = 100;
        private const float COST_SCALING = 1.5f;

        public int GetUpgradeCost(OwnedCharacter character)
        {
            return Mathf.RoundToInt(BASE_UPGRADE_COST * Mathf.Pow(COST_SCALING, character.level - 1));
        }

        public bool CanUpgrade(OwnedCharacter character)
        {
            int cost = GetUpgradeCost(character);
            return GameManager.Instance.PlayerData.softCurrency >= cost;
        }

        public bool TryUpgrade(OwnedCharacter character)
        {
            if (!CanUpgrade(character)) return false;

            int cost = GetUpgradeCost(character);
            GameManager.Instance.SpendSoftCurrency(cost);

            character.level++;
            GameManager.Instance.SaveGame();
            return true;
        }
    }
}
