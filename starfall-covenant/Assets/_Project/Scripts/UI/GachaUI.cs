using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarfallCovenant.Core;
using StarfallCovenant.Gacha;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.UI
{
    public class GachaUI : MonoBehaviour
    {
        [SerializeField] private GachaBannerSO currentBanner;
        [SerializeField] private GachaAnimator animator;

        [Header("Banner Display")]
        [SerializeField] private Image bannerArtImage;
        [SerializeField] private TextMeshProUGUI bannerNameText;
        [SerializeField] private TextMeshProUGUI bannerDescText;

        [Header("Buttons")]
        [SerializeField] private Button pullOneButton;
        [SerializeField] private Button pullTenButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI pullOneCostText;
        [SerializeField] private TextMeshProUGUI pullTenCostText;

        [Header("Currency Display")]
        [SerializeField] private TextMeshProUGUI currencyText;

        [Header("Result")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Image characterPortrait;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI rarityText;
        [SerializeField] private Button resultCloseButton;

        private void Start()
        {
            pullOneButton.onClick.AddListener(OnPullOne);
            pullTenButton.onClick.AddListener(OnPullTen);
            backButton.onClick.AddListener(() => GameManager.Instance.LoadMainMenu());
            resultCloseButton.onClick.AddListener(() => resultPanel.SetActive(false));

            animator.OnRevealComplete += ShowResult;
            resultPanel.SetActive(false);

            SetupBanner();
            UpdateCurrencyDisplay();
        }

        private void SetupBanner()
        {
            if (currentBanner == null) return;

            bannerNameText.text = currentBanner.bannerName;
            bannerDescText.text = currentBanner.description;
            pullOneCostText.text = currentBanner.costPerPull.ToString();
            pullTenCostText.text = (currentBanner.costPerPull * 10).ToString();

            if (currentBanner.bannerArt != null)
                bannerArtImage.sprite = currentBanner.bannerArt;
        }

        private void OnPullOne()
        {
            var result = GachaManager.Instance.Pull(currentBanner);
            if (result == null) return;

            UpdateCurrencyDisplay();
            animator.PlayPullAnimation(result);
        }

        private void OnPullTen()
        {
            var results = GachaManager.Instance.PullTen(currentBanner);
            if (results == null) return;

            UpdateCurrencyDisplay();
            animator.PlayMultiPullAnimation(results);
        }

        private void ShowResult(GachaPullResult result)
        {
            resultPanel.SetActive(true);
            characterNameText.text = result.character.characterName;
            rarityText.text = result.rarity.ToString().ToUpper();

            if (result.character.portrait != null)
                characterPortrait.sprite = result.character.portrait;
        }

        private void UpdateCurrencyDisplay()
        {
            currencyText.text = GameManager.Instance.PlayerData.currency.ToString();
        }
    }
}
