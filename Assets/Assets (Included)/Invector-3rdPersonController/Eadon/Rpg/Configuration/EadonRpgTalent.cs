#if EADON_RPG_INVECTOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Utils;
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    public enum TalentType
    {
        StatBonus,
        SkillBonus,
        ResistanceBonus,
        SpellLikeAbility,
        ExtraDamage,
        ExtraArmour,
        ExtraLife,
        ExtraLifeRegeneration,
        ExtraMana,
        ExtraManaRegeneration,
        ExtraStamina,
        ExtraWeightCapacity,
        AddItems,
        CharmResistance,
        HoldResistance
#if EADON_USE_SURVIVAL
        ,
        HungerResistance,
        ThirstResistance,
        HeatResistance,
        ColdResistance
#endif
    }

    public enum TalentPrerequisiteType
    {
        MustBeRace,
        MustNotBeRace,
        MustBeClass,
        MustNotBeClass,
        MustBeAlignment,
        MustNotBeAlignment,
        HasTalent,
        DoesNotHaveTalent,
        HasSkillAtMinValue,
        HasStatAtMinValue,
        MinLevel
    }

    [Serializable]
    public class EadonRpgTalentPrerequisite
    {
        public TalentPrerequisiteType prerequisiteType;
        public EadonRpgRace prerequisiteRace;
        public EadonRpgClass prerequisiteClass;
        public EadonRpgAlignment prerequisiteAlignment;
        public EadonRpgTalent prerequisiteTalent;
        public EadonRpgSkill prerequisiteSkill;
        public EadonRpgStat prerequisiteStat;
        public int prerequisiteMinValue;
    }
    
    [CreateAssetMenu(fileName = "New Eadon RPG Talent", menuName = "Eadon RPG/New Talent")]
    public class EadonRpgTalent : ScriptableObject
    {
        /// <summary>Talent name.</summary>
        [SerializeField]
        private string talentName = "";

        /// <summary>Talent description.</summary>
        [SerializeField]
        private string talentDescription = "";

        [SerializeField] 
        private Sprite talentIcon;

        [SerializeField] 
        private TalentType talentType;

        [SerializeField] 
        private EadonRpgStat talentStat;

        [SerializeField] 
        private EadonRpgSkill talentSkill;

        [SerializeField] 
        private EadonRpgDamageType talentResistance;

        [SerializeField] 
        private int intValue;

        [SerializeField] 
        private int floatValue;
        
        [SerializeField] 
        private string spellLikeAbility;

        [SerializeField] 
        private EadonRpgTalentPrerequisite[] prerequisites = new EadonRpgTalentPrerequisite[0];

        [SerializeField] 
        private int[] items = new int[0];

        public string TalentName => talentName;
        public string TalentDescription => talentDescription;
        public Sprite TalentIcon => talentIcon;
        public TalentType Type => talentType;
        public EadonRpgStat TalentStat => talentStat;
        public EadonRpgSkill TalentSkill => talentSkill;
        public int IntValue => intValue;
        public EadonRpgDamageType TalentResistance => talentResistance;
        public int FloatValue => floatValue;
        public string SpellLikeAbility => spellLikeAbility;
        public EadonRpgTalentPrerequisite[] Prerequisites => prerequisites;
        public int[] Items => items;

        public bool CanBeAcquiredBy(EadonRpgCharacterBase character)
        {
            var result = true;

            foreach (var prerequisite in prerequisites)
            {
                switch (prerequisite.prerequisiteType)
                {
                    case TalentPrerequisiteType.MustBeRace:
                        if (character.currentRace.RaceName != prerequisite.prerequisiteRace.RaceName)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.MustNotBeRace:
                        if (character.currentRace.RaceName == prerequisite.prerequisiteRace.RaceName)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.MustBeClass:
                        if (character.currentClass.ClassName != prerequisite.prerequisiteClass.ClassName)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.MustNotBeClass:
                        if (character.currentClass.ClassName == prerequisite.prerequisiteClass.ClassName)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.MustBeAlignment:
                        if (character.currentAlignment.AlignmentName != prerequisite.prerequisiteAlignment.AlignmentName)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.MustNotBeAlignment:
                        if (character.currentAlignment.AlignmentName == prerequisite.prerequisiteAlignment.AlignmentName)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.HasTalent:
                        if (!character.acquiredTalents.Contains(prerequisite.prerequisiteTalent.TalentName))
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.DoesNotHaveTalent:
                        if (character.acquiredTalents.Contains(prerequisite.prerequisiteTalent.TalentName))
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.HasSkillAtMinValue:
                        var skillValue = character.GetCharacterSkillValue(prerequisite.prerequisiteSkill.SkillName, false);
                        if (skillValue < prerequisite.prerequisiteMinValue)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.HasStatAtMinValue:
                        var statValue = character.GetCharacterStatValue(prerequisite.prerequisiteStat.StatName, false);
                        if (statValue < prerequisite.prerequisiteMinValue)
                        {
                            result = false;
                        }
                        break;
                    case TalentPrerequisiteType.MinLevel:
                        if (character.currentLevel < prerequisite.prerequisiteMinValue)
                        {
                            result = false;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            return result;
        }

        public string GetRequirements(bool newlines)
        {
            if (prerequisites.Length == 0)
            {
                return "Nothing";
            }

            var result = new StringBuilder();
            
            foreach (var prerequisite in prerequisites)
            {
                switch (prerequisite.prerequisiteType)
                {
                    case TalentPrerequisiteType.MustBeRace:
                        result.Append($"Race must be {prerequisite.prerequisiteRace.RaceName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.MustNotBeRace:
                        result.Append($"Race must not be {prerequisite.prerequisiteRace.RaceName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.MustBeClass:
                        result.Append($"Class must be {prerequisite.prerequisiteClass.ClassName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.MustNotBeClass:
                        result.Append($"Class must not be {prerequisite.prerequisiteClass.ClassName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.MustBeAlignment:
                        result.Append($"Alignment must be {prerequisite.prerequisiteAlignment}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.MustNotBeAlignment:
                        result.Append($"Alignment must not be {prerequisite.prerequisiteAlignment.AlignmentName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.HasTalent:
                        result.Append($"Must have {prerequisite.prerequisiteTalent.TalentName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.DoesNotHaveTalent:
                        result.Append($"Must not have {prerequisite.prerequisiteTalent.TalentName}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.HasSkillAtMinValue:
                        result.Append($"Must have {prerequisite.prerequisiteSkill.SkillName} at {prerequisite.prerequisiteMinValue}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.HasStatAtMinValue:
                        result.Append($"Must have {prerequisite.prerequisiteStat.StatName} at {prerequisite.prerequisiteMinValue}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    case TalentPrerequisiteType.MinLevel:
                        result.Append($"Must be of level {prerequisite.prerequisiteMinValue}");
                        result.Append(newlines ? "\n" : ", ");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (newlines)
            {
                result.Remove(result.Length - 1, 1);
            }
            else
            {
                result.Remove(result.Length - 2, 2);
            }

            return result.ToString();
        }

        public void ApplyTo(EadonRpgCharacterBase character, bool ignorePrerequisites, bool onlyModifiers)
        {
            if (ignorePrerequisites || CanBeAcquiredBy(character))
            {
                character.acquiredTalents.Add(talentName);
                switch (talentType)
                {
                    case TalentType.StatBonus:
                        character.AddModifier(talentStat.StatName, intValue, EadonModifierSource.Talent);
                        break;
                    case TalentType.SkillBonus:
                        character.AddModifier(talentSkill.SkillName, intValue, EadonModifierSource.Talent);
                        break;
                    case TalentType.ResistanceBonus:
                        character.AddResistanceModifier(talentResistance.DamageTypeName, floatValue, EadonModifierSource.Talent);
                        break;
                    case TalentType.SpellLikeAbility:
                        if (!onlyModifiers)
                        {
                            character.AddSpell(spellLikeAbility);
                        }
                        break;
                    case TalentType.ExtraDamage:
                        character.baseDamageBonus += intValue;
                        break;
                    case TalentType.ExtraArmour:
                        character.baseArmourBonus += floatValue;
                        break;
                    case TalentType.ExtraLife:
                        character.baseMaxLifeBonus += intValue;
                        break;
                    case TalentType.ExtraLifeRegeneration:
                        character.baseLifeRegenerationBonus += intValue;
                        break;
                    case TalentType.ExtraMana:
                        character.baseMaxManaBonus = intValue;
                        break;
                    case TalentType.ExtraManaRegeneration:
                        character.baseManaRegenerationBonus += intValue;
                        break;
                    case TalentType.ExtraStamina:
                        character.baseMaxStaminaBonus += intValue;
                        break;
                    case TalentType.ExtraWeightCapacity:
                        character.baseMaxEquipmentLoadBonus += floatValue;
                        break;
                    case TalentType.AddItems:
                        if (!onlyModifiers)
                        {
                            foreach (var item in items)
                            {
                                character.AddItem(item);
                            }
                        }
                        break;
                    case TalentType.CharmResistance:
                        character.baseCharmResistance += intValue;
                        break;
                    case TalentType.HoldResistance:
                        character.baseHoldResistance += intValue;
                        break;
#if EADON_USE_SURVIVAL
                    case TalentType.HungerResistance:
                        character.currentHungerResistance += floatValue;
                        break;
                    case TalentType.ThirstResistance:
                        character.currentThirstResistance += floatValue;
                        break;
                    case TalentType.HeatResistance:
                        character.currentHeatResistance += floatValue;
                        break;
                    case TalentType.ColdResistance:
                        character.currentColdResistance += floatValue;
                        break;
#endif
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
#endif
