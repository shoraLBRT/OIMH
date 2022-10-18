#if EADON_RPG_INVECTOR
using System;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using Invector;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public class EadonAreaBuffSpell : EadonAreaSpellBase
    {
        public BuffType buffType;
        public EadonRpgStat statToBuff;
        public EadonRpgSkill skillToBuff;
        public EadonRpgDamageType resistanceToBuff;
        public float buffAmount;
        public float buffDuration;

        protected override void ApplySpellEffectOnTarget(vHealthController targetHealthController, EadonRpgCharacter targetCharacter)
        {
            if (targetCharacter != null)
            {
                var amount = buffAmount * (useSpellScale ? spellScale : 1);
                var duration = buffDuration * (useSpellScale ? spellScale : 1);
            
                switch (buffType)
                {
                    case BuffType.Stat:
                        targetCharacter.AddStatSkillBuff(statToBuff.StatName, amount, duration);
                        break;
                    case BuffType.Skill:
                        targetCharacter.AddStatSkillBuff(skillToBuff.SkillName, amount, duration);
                        break;
                    case BuffType.Resistance:
                        targetCharacter.AddResistanceBuff(resistanceToBuff.DamageTypeName, amount, duration);
                        break;
#if EADON_USE_SURVIVAL
                    case BuffType.HungerResistance:
                        character.AddSurvivalBuff(BuffType.HungerResistance, amount, duration);
                        break;
                    case BuffType.ThirstResistance:
                        character.AddSurvivalBuff(BuffType.ThirstResistance, amount, duration);
                        break;
                    case BuffType.HeatResistance:
                        character.AddSurvivalBuff(BuffType.HeatResistance, amount, duration);
                        break;
                    case BuffType.ColdResistance:
                        character.AddSurvivalBuff(BuffType.HungerResistance, amount, duration);
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
