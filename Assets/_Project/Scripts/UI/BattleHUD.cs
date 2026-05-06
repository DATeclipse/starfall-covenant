using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarfallCovenant.Battle;

namespace StarfallCovenant.UI
{
    public class BattleHUD : MonoBehaviour
    {
        [Header("Player HP")]
        [SerializeField] private Slider playerHpBar;
        [SerializeField] private TextMeshProUGUI playerHpText;

        [Header("Boss HP")]
        [SerializeField] private GameObject bossHpPanel;
        [SerializeField] private Slider bossHpBar;
        [SerializeField] private TextMeshProUGUI bossNameText;

        [Header("Abilities")]
        [SerializeField] private Button[] abilityButtons;
        [SerializeField] private Image[] abilityCooldownOverlays;
        [SerializeField] private Button dodgeButton;

        [Header("Panels")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button retryButton;

        private PlayerController player;
        private AbilitySystem abilitySystem;

        private void Start()
        {
            BattleManager.Instance.OnStateChanged += OnBattleStateChanged;
            BattleManager.Instance.OnVictory += OnVictory;
            BattleManager.Instance.OnDefeat += OnDefeat;

            dodgeButton.onClick.AddListener(OnDodgePressed);

            for (int i = 0; i < abilityButtons.Length; i++)
            {
                int index = i;
                abilityButtons[i].onClick.AddListener(() => OnAbilityPressed(index));
            }

            continueButton.onClick.AddListener(() => GameManager.Instance.LoadWorldMap());
            retryButton.onClick.AddListener(() => BattleManager.Instance.StartBattle());

            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }

        private void LateUpdate()
        {
            if (player == null)
            {
                player = BattleManager.Instance.Player;
                if (player != null)
                {
                    abilitySystem = player.GetComponent<AbilitySystem>();
                    player.OnHpChanged += UpdatePlayerHp;
                    UpdatePlayerHp(player.CurrentHp, player.MaxHp);
                }
                return;
            }

            UpdateCooldowns();
        }

        private void UpdatePlayerHp(int current, int max)
        {
            playerHpBar.value = (float)current / max;
            playerHpText.text = $"{current}/{max}";
        }

        public void SetBossTarget(EnemyController boss)
        {
            bossHpPanel.SetActive(true);
            bossNameText.text = boss.IsBoss ? "BOSS" : "Enemy";
            boss.OnHpChanged += (current, max) =>
            {
                bossHpBar.value = (float)current / max;
            };
            bossHpBar.value = 1f;
        }

        private void UpdateCooldowns()
        {
            if (player.AbilityCooldowns == null) return;

            for (int i = 0; i < abilityButtons.Length && i < player.AbilityCooldowns.Length; i++)
            {
                float cd = player.AbilityCooldowns[i];
                abilityCooldownOverlays[i].fillAmount = cd > 0 ? cd / 5f : 0f;
                abilityButtons[i].interactable = cd <= 0;
            }
        }

        private void OnAbilityPressed(int index)
        {
            abilitySystem?.UseAbility(index);
        }

        private void OnDodgePressed()
        {
            abilitySystem?.Dodge();
        }

        private void OnBattleStateChanged(Data.BattleState state)
        {
            // Handle state transitions if needed
        }

        private void OnVictory(ScriptableObjects.LevelRewards rewards)
        {
            victoryPanel.SetActive(true);
            rewardText.text = $"+{rewards.currencyReward} Crystals\n+{rewards.softCurrencyReward} Credits\n+{rewards.experienceReward} XP";
        }

        private void OnDefeat()
        {
            defeatPanel.SetActive(true);
        }

        private void OnDestroy()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnStateChanged -= OnBattleStateChanged;
                BattleManager.Instance.OnVictory -= OnVictory;
                BattleManager.Instance.OnDefeat -= OnDefeat;
            }
        }
    }
}
