using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using StarfallCovenant.Core;
using StarfallCovenant.Data;
using StarfallCovenant.Progression;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.UI
{
    public class RosterUI : MonoBehaviour
    {
        [SerializeField] private Transform characterGrid;
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private CharacterUpgrader upgrader;

        [Header("Detail Panel")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Image detailPortrait;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailLevel;
        [SerializeField] private TextMeshProUGUI detailStats;
        [SerializeField] private TextMeshProUGUI detailRarity;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TextMeshProUGUI upgradeCostText;
        [SerializeField] private Button setActiveButton;
        [SerializeField] private Button closeButton;

        private OwnedCharacter selectedCharacter;

        private void Start()
        {
            upgradeButton.onClick.AddListener(OnUpgradePressed);
            setActiveButton.onClick.AddListener(OnSetActivePressed);
            if (closeButton != null)
                closeButton.onClick.AddListener(() => gameObject.SetActive(false));
            detailPanel.SetActive(false);
            PopulateRoster();
        }

        private void PopulateRoster()
        {
            foreach (Transform child in characterGrid)
                Destroy(child.gameObject);

            var characters = GameManager.Instance.PlayerData.ownedCharacters;
            foreach (var character in characters)
            {
                GameObject card = Instantiate(characterCardPrefab, characterGrid);
                var template = Resources.Load<CharacterSO>($"Characters/{character.templateId}");

                var portraitImage = card.GetComponentInChildren<Image>();
                if (template != null && template.portrait != null)
                    portraitImage.sprite = template.portrait;

                var nameText = card.GetComponentInChildren<TextMeshProUGUI>();
                if (template != null)
                    nameText.text = template.characterName;

                var button = card.GetComponent<Button>();
                var charRef = character;
                button.onClick.AddListener(() => ShowDetail(charRef, template));
            }
        }

        private void ShowDetail(OwnedCharacter character, CharacterSO template)
        {
            selectedCharacter = character;
            detailPanel.SetActive(true);

            if (template != null)
            {
                detailName.text = template.characterName;
                if (template.portrait != null)
                    detailPortrait.sprite = template.portrait;

                var scaledStats = template.baseStats.ScaledByLevel(character.level);
                detailStats.text = $"HP: {scaledStats.hp}\nATK: {scaledStats.attack}\nDEF: {scaledStats.defense}\nSPD: {scaledStats.speed}\nSP: {scaledStats.specialPower}";
            }

            detailLevel.text = $"Lv. {character.level}";
            detailRarity.text = character.rarity.ToString();

            int cost = upgrader.GetUpgradeCost(character);
            upgradeCostText.text = cost.ToString();
            upgradeButton.interactable = upgrader.CanUpgrade(character);
        }

        private void OnUpgradePressed()
        {
            if (selectedCharacter == null) return;

            if (upgrader.TryUpgrade(selectedCharacter))
            {
                var template = Resources.Load<CharacterSO>($"Characters/{selectedCharacter.templateId}");
                ShowDetail(selectedCharacter, template);
            }
        }

        private void OnSetActivePressed()
        {
            if (selectedCharacter == null) return;

            var party = GameManager.Instance.PlayerData.activeParty;
            if (!party.Contains(selectedCharacter.id))
            {
                if (party.Count >= 3)
                    party.RemoveAt(0);
                party.Add(selectedCharacter.id);
                GameManager.Instance.SaveGame();
            }
        }
    }
}
