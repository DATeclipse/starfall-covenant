using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.UI
{
    public class LevelNodeUI : MonoBehaviour
    {
        [SerializeField] private Button nodeButton;
        [SerializeField] private TextMeshProUGUI levelNameText;
        [SerializeField] private Image lockIcon;
        [SerializeField] private Image completedIcon;
        [SerializeField] private Image nodeBackground;
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = Color.gray;
        [SerializeField] private Color completedColor = Color.green;

        private LevelSO level;

        public System.Action<LevelSO> OnNodeSelected;

        public void Setup(LevelSO level, bool unlocked, bool completed)
        {
            this.level = level;
            levelNameText.text = level.levelName;

            lockIcon.gameObject.SetActive(!unlocked);
            completedIcon.gameObject.SetActive(completed);
            nodeButton.interactable = unlocked;

            if (completed)
                nodeBackground.color = completedColor;
            else if (unlocked)
                nodeBackground.color = unlockedColor;
            else
                nodeBackground.color = lockedColor;

            nodeButton.onClick.AddListener(() => OnNodeSelected?.Invoke(level));
        }
    }
}
