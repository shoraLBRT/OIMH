#if EADON_RPG_INVECTOR
using System;
using Eadon.Rpg.Invector.Configuration;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public enum BuffType
    {
        Stat,
        Skill,
        Resistance
#if EADON_USE_SURVIVAL
        ,
        HungerResistance,
        ThirstResistance,
        HeatResistance,
        ColdResistance
#endif
    }
    
    public class EadonBuffSpell : EadonSpellBase
    {
        public BuffType buffType;
        public EadonRpgStat statToBuff;
        public EadonRpgSkill skillToBuff;
        public EadonRpgDamageType resistanceToBuff;
        public float buffAmount;
        public float buffDuration;

        protected override void Start()
        {
            base.Start();
            
            if (character == null)
            {
                return;
            }
            
            var amount = buffAmount * (useSpellScale ? spellScale : 1);
            var duration = buffDuration * (useSpellScale ? spellScale : 1);
            
            switch (buffType)
            {
                case BuffType.Stat:
                    character.AddStatSkillBuff(statToBuff.StatName, amount, duration);
                    break;
                case BuffType.Skill:
                    character.AddStatSkillBuff(skillToBuff.SkillName, amount, duration);
                    break;
                case BuffType.Resistance:
                    character.AddResistanceBuff(resistanceToBuff.DamageTypeName, amount, duration);
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
#endif
