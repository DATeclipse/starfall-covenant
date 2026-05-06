using UnityEngine;
using StarfallCovenant.Data;

namespace StarfallCovenant.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewAbility", menuName = "Starfall Covenant/Ability")]
    public class AbilitySO : ScriptableObject
    {
        public string abilityId;
        public string abilityName;
        public AbilityType type;
        public int damage;
        public float cooldown;
        public float range;
        public GameObject vfxPrefab;
        public Sprite icon;
        [TextArea] public string description;
    }
}
