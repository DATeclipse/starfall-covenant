using UnityEngine;
using UnityEngine.SceneManagement;

namespace StarfallCovenant.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private string mainMenuScene = "MainMenu";
        [SerializeField] private string worldMapScene = "WorldMap";
        [SerializeField] private string battleScene = "Battle";
        [SerializeField] private string gachaScene = "Gacha";

        public PlayerSaveData PlayerData { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        private void Initialize()
        {
            PlayerData = SaveManager.Instance.LoadGame();
            if (PlayerData == null)
            {
                PlayerData = new PlayerSaveData();
            }
        }

        public void SaveGame()
        {
            SaveManager.Instance.SaveGame(PlayerData);
        }

        public void AddCurrency(int amount)
        {
            PlayerData.currency += amount;
            SaveGame();
        }

        public void AddSoftCurrency(int amount)
        {
            PlayerData.softCurrency += amount;
            SaveGame();
        }

        public bool SpendCurrency(int amount)
        {
            if (PlayerData.currency < amount) return false;
            PlayerData.currency -= amount;
            SaveGame();
            return true;
        }

        public bool SpendSoftCurrency(int amount)
        {
            if (PlayerData.softCurrency < amount) return false;
            PlayerData.softCurrency -= amount;
            SaveGame();
            return true;
        }

        public void LoadMainMenu() => SceneLoader.LoadScene(mainMenuScene);
        public void LoadWorldMap() => SceneLoader.LoadScene(worldMapScene);
        public void LoadBattle() => SceneLoader.LoadScene(battleScene);
        public void LoadGacha() => SceneLoader.LoadScene(gachaScene);
    }
}
