using UnityEngine;
using UnityEngine.UI;
using StarfallCovenant.Core;

namespace StarfallCovenant.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button gachaButton;
        [SerializeField] private Button rosterButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject rosterPanel;

        private void Start()
        {
            playButton.onClick.AddListener(OnPlayPressed);
            gachaButton.onClick.AddListener(OnGachaPressed);
            rosterButton.onClick.AddListener(OnRosterPressed);
            settingsButton.onClick.AddListener(OnSettingsPressed);
        }

        private void OnPlayPressed()
        {
            GameManager.Instance.LoadWorldMap();
        }

        private void OnGachaPressed()
        {
            GameManager.Instance.LoadGacha();
        }

        private void OnRosterPressed()
        {
            if (rosterPanel != null)
                rosterPanel.SetActive(!rosterPanel.activeSelf);
        }

        private void OnSettingsPressed()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}
