using UnityEngine;
using System.Collections.Generic;
using StarfallCovenant.Core;
using StarfallCovenant.Data;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform[] enemySpawnPoints;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject projectilePrefab;

        private LevelSO currentLevel;
        private PlayerController player;
        private List<EnemyController> activeEnemies = new();
        private int currentWaveIndex;
        private BattleState state;

        public BattleState State => state;
        public PlayerController Player => player;

        public System.Action<BattleState> OnStateChanged;
        public System.Action<LevelRewards> OnVictory;
        public System.Action OnDefeat;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            string levelId = SceneLoader.CurrentLevelId;
            currentLevel = Resources.Load<LevelSO>($"Levels/{levelId}");
            if (currentLevel != null)
            {
                StartBattle();
            }
        }

        public void StartBattle()
        {
            state = BattleState.Intro;
            OnStateChanged?.Invoke(state);

            SpawnPlayer();
            currentWaveIndex = 0;
            SpawnWave(currentWaveIndex);

            state = BattleState.Active;
            OnStateChanged?.Invoke(state);
        }

        private void SpawnPlayer()
        {
            GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            player = playerObj.GetComponent<PlayerController>();

            var gameData = GameManager.Instance.PlayerData;
            if (gameData.activeParty.Count > 0)
            {
                string charId = gameData.activeParty[0];
                var ownedChar = gameData.ownedCharacters.Find(c => c.id == charId);
                if (ownedChar != null)
                {
                    var template = Resources.Load<CharacterSO>($"Characters/{ownedChar.templateId}");
                    player.Initialize(template, ownedChar.level);
                }
            }

            player.OnDeath += HandlePlayerDeath;
        }

        private void SpawnWave(int waveIndex)
        {
            if (waveIndex >= currentLevel.enemyWaves.Length)
            {
                SpawnBoss();
                return;
            }

            EnemyWave wave = currentLevel.enemyWaves[waveIndex];
            for (int i = 0; i < wave.enemies.Length; i++)
            {
                Vector3 spawnPos = enemySpawnPoints[i % enemySpawnPoints.Length].position;
                SpawnEnemy(wave.enemies[i], spawnPos);
            }
        }

        private void SpawnBoss()
        {
            Vector3 spawnPos = enemySpawnPoints[0].position;
            SpawnEnemy(currentLevel.bossConfig, spawnPos);
        }

        private void SpawnEnemy(EnemyConfig config, Vector3 position)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
            EnemyController enemy = enemyObj.GetComponent<EnemyController>();
            enemy.Initialize(config);
            enemy.OnDeath += HandleEnemyDeath;
            activeEnemies.Add(enemy);
        }

        private void HandleEnemyDeath(EnemyController enemy)
        {
            activeEnemies.Remove(enemy);
            Destroy(enemy.gameObject, 1f);

            if (activeEnemies.Count == 0)
            {
                currentWaveIndex++;
                if (currentWaveIndex > currentLevel.enemyWaves.Length)
                {
                    EndBattle(true);
                }
                else
                {
                    SpawnWave(currentWaveIndex);
                }
            }
        }

        private void HandlePlayerDeath()
        {
            EndBattle(false);
        }

        private void EndBattle(bool victory)
        {
            if (victory)
            {
                state = BattleState.Victory;
                GrantRewards();
                OnVictory?.Invoke(currentLevel.rewards);
            }
            else
            {
                state = BattleState.Defeat;
                OnDefeat?.Invoke();
            }
            OnStateChanged?.Invoke(state);
        }

        private void GrantRewards()
        {
            var rewards = currentLevel.rewards;
            var gameManager = GameManager.Instance;

            gameManager.AddCurrency(rewards.currencyReward);
            gameManager.AddSoftCurrency(rewards.softCurrencyReward);

            bool firstClear = !gameManager.PlayerData.IsLevelCompleted(currentLevel.levelId);
            if (firstClear)
            {
                gameManager.AddCurrency(rewards.firstClearBonus);
                gameManager.PlayerData.CompleteLevel(currentLevel.levelId);
            }

            gameManager.SaveGame();
        }

        public void SpawnAbilityEffect(Vector3 origin, Vector2 direction, AbilitySO ability, int attackStat)
        {
            if (ability.vfxPrefab != null)
            {
                GameObject vfx = Instantiate(ability.vfxPrefab, origin, Quaternion.identity);
                var projectile = vfx.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(direction, ability.damage, attackStat, true);
                }
            }
        }

        public void SpawnEnemyAbilityEffect(Vector3 origin, Vector2 direction, AbilitySO ability, int attackStat)
        {
            if (ability.vfxPrefab != null)
            {
                GameObject vfx = Instantiate(ability.vfxPrefab, origin, Quaternion.identity);
                var projectile = vfx.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(direction, ability.damage, attackStat, false);
                }
            }
        }
    }
}
