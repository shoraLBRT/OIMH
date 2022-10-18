#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Utils;
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Race", menuName = "Eadon RPG/New Race")]

    public class EadonRpgRace : ScriptableObject
    {
        /// <summary>Race name</summary>
        [SerializeField]
        private string race = "";

        /// <summary>Race stat modifiers</summary>
        [SerializeField]
        private EadonRPGStatModifier[] statModifiers = new EadonRPGStatModifier[0];

        /// <summary>Race stat modifiers</summary>
        [SerializeField]
        private EadonRPGSkillModifier[] skillModifiers = new EadonRPGSkillModifier[0];

        /// <summary>Race resistance modifiers</summary>
        [SerializeField]
        private EadonRPGResistanceModifier[] raceResistances = new EadonRPGResistanceModifier[0];

        /// <summary>Race talents</summary>
        [SerializeField]
        private EadonRpgTalent[] raceTalents = new EadonRpgTalent[0];

        /// <summary>Race base armour</summary>
        [SerializeField]
        private float baseArmour = 0f;

        public string RaceName => race;

        public float RaceBaseArmour => baseArmour;

        public EadonRpgTalent[] RaceTalents => raceTalents;

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
        
        public float RaceHungerResistance => hungerResistance;

        public float RaceThirstResistance => thirstResistance;

        public float RaceHeatResistance => heatResistance;

        public float RaceColdResistance => coldResistance;
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

        public EadonBaseResistPercent[] GetRaceResistances()
        {
            EadonBaseResistPercent[] modifiers = new EadonBaseResistPercent[raceResistances.Length];

            for (int i = 0; i < raceResistances.Length; i++)
            {
                modifiers[i] = new EadonBaseResistPercent() { resist = raceResistances[i].damageType.DamageTypeName, value = raceResistances[i].value, src = EadonModifierSource.Character };
            }

            return modifiers;
        }
    }
}

#endif
