using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using StarfallCovenant.Core;
using StarfallCovenant.Progression;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.UI
{
    public class WorldMapUI : MonoBehaviour
    {
        [SerializeField] private Transform levelNodeContainer;
        [SerializeField] private GameObject levelNodePrefab;
        [SerializeField] private Button backButton;

        private List<LevelNodeUI> levelNodes = new();

        private void Start()
        {
            backButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
            PopulateLevels();
        }

        private void PopulateLevels()
        {
            int chapter = GameManager.Instance.PlayerData.currentChapter;
            var levels = ProgressionManager.Instance.GetChapterLevels(chapter);

            foreach (var level in levels)
            {
                GameObject nodeObj = Instantiate(levelNodePrefab, levelNodeContainer);
                LevelNodeUI node = nodeObj.GetComponent<LevelNodeUI>();
                bool unlocked = ProgressionManager.Instance.IsLevelUnlocked(level.levelId);
                bool completed = GameManager.Instance.PlayerData.IsLevelCompleted(level.levelId);
                node.Setup(level, unlocked, completed);
                node.OnNodeSelected += OnLevelSelected;
                levelNodes.Add(node);
            }
        }

        private void OnLevelSelected(LevelSO level)
        {
            SceneLoader.LoadBattleWithLevel(level.levelId);
        }
    }
}
