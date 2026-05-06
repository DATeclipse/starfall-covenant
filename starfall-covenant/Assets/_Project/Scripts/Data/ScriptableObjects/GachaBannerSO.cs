using UnityEngine;
using StarfallCovenant.Data;

namespace StarfallCovenant.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewBanner", menuName = "Starfall Covenant/Gacha Banner")]
    public class GachaBannerSO : ScriptableObject
    {
        public string bannerId;
        public string bannerName;
        public CharacterSO[] featuredCharacters;
        public CharacterSO[] characterPool;
        public RarityRate[] rateTable;
        public int costPerPull;
        public int pityThreshold;
        public Rarity guaranteedPityRarity;
        public Sprite bannerArt;
        [TextArea] public string description;
    }

    [System.Serializable]
    public class RarityRate
    {
        public Rarity rarity;
        [Range(0f, 1f)] public float rate;
    }
}
