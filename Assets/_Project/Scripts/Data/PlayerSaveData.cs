using System;
using System.Collections.Generic;

namespace StarfallCovenant.Core
{
    [Serializable]
    public class PlayerSaveData
    {
        public int currency;
        public int softCurrency;
        public List<StarfallCovenant.Data.OwnedCharacter> ownedCharacters;
        public List<string> activeParty;
        public List<string> completedLevels;
        public int currentChapter;
        public Dictionary<string, int> bannerPityCounters;

        public PlayerSaveData()
        {
            currency = 1600;
            softCurrency = 5000;
            ownedCharacters = new List<StarfallCovenant.Data.OwnedCharacter>();
            activeParty = new List<string>();
            completedLevels = new List<string>();
            currentChapter = 0;
            bannerPityCounters = new Dictionary<string, int>();
        }

        public bool IsLevelCompleted(string levelId) => completedLevels.Contains(levelId);

        public void CompleteLevel(string levelId)
        {
            if (!completedLevels.Contains(levelId))
            {
                completedLevels.Add(levelId);
            }
        }
    }
}
