using System;
using System.Collections.Generic;

namespace StarfallCovenant.Data
{
    [Serializable]
    public class OwnedCharacter
    {
        public string id;
        public string templateId;
        public int level;
        public int experience;
        public Rarity rarity;
        public List<string> equippedAbilities;

        public OwnedCharacter()
        {
            id = Guid.NewGuid().ToString();
            level = 1;
            experience = 0;
            equippedAbilities = new List<string>();
        }

        public OwnedCharacter(string templateId, Rarity rarity) : this()
        {
            this.templateId = templateId;
            this.rarity = rarity;
        }

        public int ExperienceToNextLevel => level * 100;

        public bool AddExperience(int amount)
        {
            experience += amount;
            bool leveledUp = false;
            while (experience >= ExperienceToNextLevel)
            {
                experience -= ExperienceToNextLevel;
                level++;
                leveledUp = true;
            }
            return leveledUp;
        }
    }
}
