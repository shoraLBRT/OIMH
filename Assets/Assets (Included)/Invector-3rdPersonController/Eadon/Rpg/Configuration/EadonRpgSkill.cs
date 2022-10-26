#if EADON_RPG_INVECTOR
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Skill", menuName = "Eadon RPG/New Skill")]
    public class EadonRpgSkill : ScriptableObject
    {
        /// <summary>Skill name.</summary>
        [SerializeField]
        private string skill = "";

        /// <summary>Skill min value.</summary>
        [SerializeField]
        private int minValue;

        /// <summary>Skill max value.</summary>
        [SerializeField]
        private int maxValue = 99;

        /// <summary>Related stat.</summary>
        [SerializeField]
        private EadonRpgStat relatedStat;

        [SerializeField] private bool affectsDamage;
        [SerializeField] private bool addStatBonusToSkill;

        public string SkillName => skill;

        public int SkillMinValue => minValue;

        public int SkillMaxValue => maxValue;

        public string RelatedStatName => relatedStat ? relatedStat.StatName : null;

        public bool SkillAffectsDamage => affectsDamage;

        public bool StatBonusAddToSKill => addStatBonusToSkill;
    }
}
#endif
