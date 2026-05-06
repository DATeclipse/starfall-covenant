using UnityEngine;
using StarfallCovenant.Core;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Progression
{
    public class LevelUnlocker : MonoBehaviour
    {
        public bool CanAccessLevel(LevelSO level)
        {
            if (level == null) return false;
            return ProgressionManager.Instance.IsLevelUnlocked(level.levelId);
        }

        public void OnLevelCompleted(string levelId, LevelRewards rewards)
        {
            var playerData = GameManager.Instance.PlayerData;
            playerData.CompleteLevel(levelId);

            foreach (var character in playerData.ownedCharacters)
            {
                if (playerData.activeParty.Contains(character.id))
                {
                    character.AddExperience(rewards.experienceReward);
                }
            }

            GameManager.Instance.SaveGame();
        }
    }
}
