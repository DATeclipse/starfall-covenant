using UnityEngine;
using System.Collections.Generic;
using StarfallCovenant.Core;
using StarfallCovenant.Data;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Progression
{
    public class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        [SerializeField] private LevelSO[] allLevels;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool IsLevelUnlocked(string levelId)
        {
            LevelSO level = GetLevel(levelId);
            if (level == null) return false;

            if (string.IsNullOrEmpty(level.unlockAfterLevelId))
                return true;

            return GameManager.Instance.PlayerData.IsLevelCompleted(level.unlockAfterLevelId);
        }

        public List<LevelSO> GetChapterLevels(int chapterIndex)
        {
            var levels = new List<LevelSO>();
            foreach (var level in allLevels)
            {
                if (level.chapterIndex == chapterIndex)
                    levels.Add(level);
            }
            levels.Sort((a, b) => a.levelIndex.CompareTo(b.levelIndex));
            return levels;
        }

        public LevelSO GetLevel(string levelId)
        {
            foreach (var level in allLevels)
            {
                if (level.levelId == levelId) return level;
            }
            return null;
        }

        public LevelSO GetNextLevel(string currentLevelId)
        {
            LevelSO current = GetLevel(currentLevelId);
            if (current == null) return null;

            foreach (var level in allLevels)
            {
                if (level.unlockAfterLevelId == currentLevelId)
                    return level;
            }
            return null;
        }
    }
}
