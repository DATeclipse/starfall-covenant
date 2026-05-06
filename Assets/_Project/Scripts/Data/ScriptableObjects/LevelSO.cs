using UnityEngine;
using StarfallCovenant.Data;

namespace StarfallCovenant.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Starfall Covenant/Level")]
    public class LevelSO : ScriptableObject
    {
        public string levelId;
        public string levelName;
        public int chapterIndex;
        public int levelIndex;
        public DialogueLine[] storyDialogue;
        public EnemyWave[] enemyWaves;
        public EnemyConfig bossConfig;
        public LevelRewards rewards;
        public string unlockAfterLevelId;
        public Sprite backgroundSprite;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite speakerPortrait;
        [TextArea] public string text;
    }

    [System.Serializable]
    public class EnemyWave
    {
        public EnemyConfig[] enemies;
        public float spawnDelay;
    }

    [System.Serializable]
    public class EnemyConfig
    {
        public string enemyName;
        public Stats stats;
        public Sprite sprite;
        public RuntimeAnimatorController animator;
        public AbilitySO[] abilities;
        public bool isBoss;
        public float attackInterval;
    }

    [System.Serializable]
    public class LevelRewards
    {
        public int currencyReward;
        public int softCurrencyReward;
        public int experienceReward;
        public int firstClearBonus;
    }
}
