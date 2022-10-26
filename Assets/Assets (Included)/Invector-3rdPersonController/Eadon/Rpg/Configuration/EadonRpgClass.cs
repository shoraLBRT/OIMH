#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Utils;
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Class", menuName = "Eadon RPG/New Class")]

    public class EadonRpgClass : ScriptableObject
    {
        [SerializeField]
        private string className = "";
        [SerializeField]
        private EadonRPGStatModifier[] statModifiers = new EadonRPGStatModifier[0];
        [SerializeField]
        private EadonRPGSkillModifier[] skillModifiers = new EadonRPGSkillModifier[0];
        [SerializeField]
        private EadonRPGResistanceModifier[] classResistances = new EadonRPGResistanceModifier[0];
        [SerializeField]
        private float baseArmour;
        [SerializeField]
        private float damageBonus;
        [SerializeField]
        private int magicResistanceMultiplier = 1;
        [SerializeField]
        private bool aiAttackAtRange = false;

        /// <summary>Race talents</summary>
        [SerializeField]
        private EadonRpgTalent[] classTalents = new EadonRpgTalent[0];

        public string ClassName => className;

        public float ClassBaseArmour => baseArmour;

        public float ClassDamageBonus => damageBonus;

        public int ClassMagicResistanceMultiplier => magicResistanceMultiplier;

        public bool ClassAiAttackAtRange => aiAttackAtRange;

        public EadonRpgTalent[] ClassTalents => classTalents;

#if EADON_USE_SURVIVAL
        /// <summary>Race base armour</summary>
        [SerializeField]
        private float hungerResistance = 0f;
        
        /// <summary>Race base armour</summary>
        [SerializeField]
        private float thirstResistance = 0f;
        
        /// <summary>Race base armour</summary>
        [SerializeField]
        private float heatResistance = 0f;
        
        /// <summary>Race base armour</summary>
        [SerializeField]
        private float coldResistance = 0f;
        
        public float ClassHungerResistance => hungerResistance;

        public float ClassThirstResistance => thirstResistance;

        public float ClassHeatResistance => heatResistance;

        public float ClassColdResistance => coldResistance;
#endif
        
        public EadonBaseStatSkillValue[] GetStatModifiers()
        {
            EadonBaseStatSkillValue[] modifiers = new EadonBaseStatSkillValue[statModifiers.Length];

            for (int i = 0; i < statModifiers.Length; i++)
            {
                modifiers[i] = new EadonBaseStatSkillValue() { stat = statModifiers[i].stat.StatName, value = statModifiers[i].value, src = EadonModifierSource.Character };
            }

            return modifiers;
        }

        public EadonBaseStatSkillValue[] GetSkillModifiers()
        {
            EadonBaseStatSkillValue[] modifiers = new EadonBaseStatSkillValue[skillModifiers.Length];

            for (int i = 0; i < skillModifiers.Length; i++)
            {
                modifiers[i] = new EadonBaseStatSkillValue() { stat = skillModifiers[i].skill.SkillName, value = skillModifiers[i].value, src = EadonModifierSource.Character };
            }

            return modifiers;
        }

        public EadonBaseResistPercent[] GetClassResistances()
        {
            EadonBaseResistPercent[] modifiers = new EadonBaseResistPercent[classResistances.Length];

            for (int i = 0; i < classResistances.Length; i++)
            {
                modifiers[i] = new EadonBaseResistPercent() { resist = classResistances[i].damageType.DamageTypeName, value = classResistances[i].value, src = EadonModifierSource.Character };
            }

            return modifiers;
        }
    }
}
#endif
