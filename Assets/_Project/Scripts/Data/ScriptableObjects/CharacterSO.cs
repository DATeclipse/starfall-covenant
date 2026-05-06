using UnityEngine;
using StarfallCovenant.Data;

namespace StarfallCovenant.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Starfall Covenant/Character")]
    public class CharacterSO : ScriptableObject
    {
        public string characterId;
        public string characterName;
        public Race race;
        public Rarity baseRarity;
        public Stats baseStats;
        public AbilitySO[] abilities;
        public Sprite portrait;
        public Sprite battleSprite;
        public RuntimeAnimatorController animatorController;
        [TextArea] public string lore;
    }
}
