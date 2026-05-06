using UnityEngine;
using System.Collections.Generic;
using StarfallCovenant.Core;
using StarfallCovenant.Data;
using StarfallCovenant.ScriptableObjects;

namespace StarfallCovenant.Gacha
{
    public class GachaManager : MonoBehaviour
    {
        public static GachaManager Instance { get; private set; }

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

        public GachaPullResult Pull(GachaBannerSO banner)
        {
            var playerData = GameManager.Instance.PlayerData;

            if (playerData.currency < banner.costPerPull)
                return null;

            GameManager.Instance.SpendCurrency(banner.costPerPull);

            int pityCount = GetPityCount(banner.bannerId);
            pityCount++;

            Rarity rolledRarity = RollRarity(banner, pityCount);

            if (pityCount >= banner.pityThreshold)
            {
                rolledRarity = banner.guaranteedPityRarity;
                pityCount = 0;
            }

            SetPityCount(banner.bannerId, pityCount);

            CharacterSO rolledCharacter = PickCharacter(banner, rolledRarity);
            OwnedCharacter newChar = new OwnedCharacter(rolledCharacter.characterId, rolledRarity);
            playerData.ownedCharacters.Add(newChar);
            GameManager.Instance.SaveGame();

            return new GachaPullResult
            {
                character = rolledCharacter,
                ownedCharacter = newChar,
                rarity = rolledRarity,
                pityCount = pityCount
            };
        }

        public List<GachaPullResult> PullTen(GachaBannerSO banner)
        {
            int totalCost = banner.costPerPull * 10;
            if (GameManager.Instance.PlayerData.currency < totalCost)
                return null;

            var results = new List<GachaPullResult>();
            for (int i = 0; i < 10; i++)
            {
                var result = Pull(banner);
                if (result != null) results.Add(result);
            }
            return results;
        }

        private Rarity RollRarity(GachaBannerSO banner, int pityCount)
        {
            float softPityBonus = 0f;
            if (pityCount > banner.pityThreshold * 0.75f)
            {
                softPityBonus = (pityCount - banner.pityThreshold * 0.75f) * 0.05f;
            }

            float roll = Random.value;
            float cumulative = 0f;

            for (int i = banner.rateTable.Length - 1; i >= 0; i--)
            {
                float rate = banner.rateTable[i].rate;
                if (banner.rateTable[i].rarity >= Rarity.Legendary)
                {
                    rate += softPityBonus;
                }

                cumulative += rate;
                if (roll <= cumulative)
                {
                    return banner.rateTable[i].rarity;
                }
            }

            return Rarity.Common;
        }

        private CharacterSO PickCharacter(GachaBannerSO banner, Rarity rarity)
        {
            List<CharacterSO> candidates = new();

            foreach (var c in banner.featuredCharacters)
            {
                if (c.baseRarity == rarity) candidates.Add(c);
            }

            if (candidates.Count == 0)
            {
                foreach (var c in banner.characterPool)
                {
                    if (c.baseRarity == rarity) candidates.Add(c);
                }
            }

            if (candidates.Count == 0 && banner.characterPool.Length > 0)
            {
                return banner.characterPool[Random.Range(0, banner.characterPool.Length)];
            }

            return candidates[Random.Range(0, candidates.Count)];
        }

        private int GetPityCount(string bannerId)
        {
            var counters = GameManager.Instance.PlayerData.bannerPityCounters;
            return counters.ContainsKey(bannerId) ? counters[bannerId] : 0;
        }

        private void SetPityCount(string bannerId, int count)
        {
            GameManager.Instance.PlayerData.bannerPityCounters[bannerId] = count;
        }
    }

    public class GachaPullResult
    {
        public CharacterSO character;
        public OwnedCharacter ownedCharacter;
        public Rarity rarity;
        public int pityCount;
    }
}
