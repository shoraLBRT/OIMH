#if EADON_RPG_INVECTOR
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Stat", menuName = "Eadon RPG/New Stat")]
    public class EadonRpgStat : ScriptableObject
    {
        /// <summary>Stat name.</summary>
        [SerializeField]
        private string stat = "";

        /// <summary>Stat min value.</summary>
        [SerializeField]
        private int minValue = 0;

        /// <summary>Stat max value.</summary>
        [SerializeField]
        private int maxValue = 99;

        /// <summary>Stat affects HP</summary>
        [SerializeField]
        private bool affectsHp;

        /// <summary>Stat affects Stamina</summary>
        [SerializeField]
        private bool affectsStamina;

        /// <summary>Stat affects Mana</summary>
        [SerializeField]
        private bool affectsMana;

        /// <summary>Stat affects Weight Limits</summary>
        [SerializeField]
        private bool affectsWeightLimits;

        /// <summary>Stat affects Spell Scale</summary>
        [SerializeField]
        private bool affectsSpellScale;

        [SerializeField] private bool affectsDamage;

        public string StatName => stat;

        public int StatMinValue => minValue;

        public int StatMaxValue => maxValue;

        public bool StatAffectsHP => affectsHp;

        public bool StatAffectsStamina => affectsStamina;

        public bool StatAffectsMana => affectsMana;

        public bool StatAffectsWeightLimits => affectsWeightLimits;

        public bool StatAffectsSpellScale => affectsSpellScale;

        public bool StatAffectsDamage => affectsDamage;
    }
}
#endif
