#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using Eadon.Rpg.Invector.Utils;
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Defaults", menuName = "Eadon RPG/New Default Values")]
    public class EadonRpgDefaultValues : ScriptableObject
    {
        [SerializeField]
        private int baseHp = 36;
        [SerializeField]
        private int hpPerLevel = 4;
        [SerializeField]
        private int hpPerStatPoint = 1;
        [SerializeField]
        private int baseMana = 100;
        [SerializeField]
        private int manaPerStatPoint = 6;
        [SerializeField]
        private int baseStamina = 150;
        [SerializeField]
        private float resistancePerLevel = 0.05f;
        [SerializeField]
        private int damagePerLevel = 2;
        [SerializeField]
        private int levelUpXpBase = 1000;
        [SerializeField]
        private int levelUpXpBandCost = 250;
        [SerializeField]
        private float armourReduction = 0.05f;
        [SerializeField]
        private float spellPowerScale = 0.01f;
        [SerializeField]
        private float spellPowerMax = 3f;
        [SerializeField]
        private float spellScaleIncreasePerLevel = .1f;
        [SerializeField]
        private int maxLevel = 999;
        [SerializeField]
        private int statMinValue = 3;
        [SerializeField]
        private int statMaxValue = 99;
        [SerializeField]
        private int statInitialValue = 8;
        [SerializeField]
        private int levelUpStatPoints = 4;
        [SerializeField]
        private int initialUnspentStatPoints;
        [SerializeField]
        private int skillMaxValue = 99;
        [SerializeField]
        private int skillInitialValue;
        [SerializeField]
        private int levelUpSkillPoints = 4;
        [SerializeField]
        private int initialUnspentSkillPoints;
        [SerializeField]
        private int initialTalents;
        [SerializeField]
        private int levelsPerTalent = 3;
        [SerializeField] 
        private int baseWeightLimit = 20;
        [SerializeField] 
        private int weightIncreasePerStatPoint = 2;
        [SerializeField]
        private EadonRPGStatBonusTier[] statBonusTiers = new EadonRPGStatBonusTier[0];

        public int BaseHpDefault => baseHp;

        public int HpPerLevelDefault => hpPerLevel;

        public int HpPerStatPointDefault => hpPerStatPoint;

        public int BaseManaDefault => baseMana;

        public int ManaPerStatPointDefault => manaPerStatPoint;

        public int BaseStaminaDefault => baseStamina;

        public float ResistancePerLevelDefault => resistancePerLevel;

        public float DamagePerLevelDefault => damagePerLevel;

        public int LevelUpXpBaseDefault => levelUpXpBase;

        public int LevelUpXpBandCostDefault => levelUpXpBandCost;

        public float ArmourReductionDefault => armourReduction;

        public float SpellScaleIncreasePerLevel => spellScaleIncreasePerLevel;
        
        public float SpellPowerMaxDefault => spellPowerMax;

        public float SpellPowerScaleDefault => spellPowerScale;

        public int MaxLevelDefault => maxLevel;

        public int StatMinValueDefault => statMinValue;

        public int StatMaxValueDefault => statMaxValue;

        public int StatInitialValueDefault => statInitialValue;

        public int LevelUpStatPointsDefault => levelUpStatPoints;

        public int InitialUnspentStatPointsDefault => initialUnspentStatPoints;

        public int SkillMaxValueDefault => skillMaxValue;

        public int SkillInitialValueDefault => skillInitialValue;

        public int LevelUpSkillPointsDefault => levelUpSkillPoints;

        public int InitialUnspentSkillPointsDefault => initialUnspentSkillPoints;

        public int LevelsPerTalentDefault => levelsPerTalent;

        public int InitialTalents => initialTalents;

        public int BaseWeightLimitDefault => baseWeightLimit;

        public int WeightIncreasePerStatPointDefault => weightIncreasePerStatPoint;

        public int GetSkillBonusForStatValue(float statValue)
        {
            if(statBonusTiers.Length == 0)
            {
                return 0;
            }

            var tiers = statBonusTiers.ToList();
            tiers = tiers.OrderBy(b => b.statValue).ToList();

            var bonus = 0;

            for (int tier = 0; tier < tiers.Count; tier++)
            {
                if (tiers[tier].statValue > statValue)
                {
                    return bonus;
                }

                bonus = tiers[tier].bonus;
            }

            return bonus;
        }
    }
}
#endif
