#if EADON_RPG_INVECTOR
using System;
using CogsAndGoggles.Library.DataStructures.SerializableDictionary;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Magic;

namespace Eadon.Rpg.Invector.Utils
{
    #region Enums

    /// <summary>
    /// Modifier stack source for filtering.
    /// </summary>
    public enum EadonModifierSource
    {
        Character, Armour, Weapon, MagicItem, Buff, Talent
    }

    public enum InvectorStats
    {
        Health, Stamina
    }

    #endregion

    #region Data Structures

    [Serializable]
    public class EadonBaseValue
    {
        public EadonModifierSource src;
        public float value;
    }

    [Serializable]
    public class InvectorBaseStatSkillValue : EadonBaseValue
    {
        public InvectorStats stat;
    }

    [Serializable]
    public class EadonBaseStatSkillValue : EadonBaseValue
    {
        public string stat;
    }

    [Serializable]
    public class EadonBaseResistPercent : EadonBaseValue
    {
        public string resist;
    }

    [Serializable]
    public class EadonTempStatSkillBuff
    {
        public EadonBaseStatSkillValue buffValue;
        public float duration;
        public float timer;
    }

    [Serializable]
    public class EadonTempResistBuff
    {
        public EadonBaseResistPercent buffValue;
        public float duration;
        public float timer;
    }

    #endregion
    
    #region Survival

#if EADON_USE_SURVIVAL

    [Serializable]
    public class EadonTempSurvivalBuff
    {
        public BuffType buffType;
        public float buffValue;
        public float duration;
        public float timer;
    }
    
#endif
    
    #endregion

    #region Scriptable Object Support

    [Serializable]
    public class EadonRPGStatModifier
    {
        public EadonRpgStat stat;
        public int value;
    }

    [Serializable]
    public class EadonRPGSkillModifier
    {
        public EadonRpgSkill skill;
        public int value;
    }

    [Serializable]
    public class EadonRPGResistanceModifier
    {
        public EadonRpgDamageType damageType;
        public int value;
    }

    [Serializable]
    public class EadonRPGStatBonusTier
    {
        public int statValue;
        public int bonus;
    }
    
    #endregion

    #region Dictionaries

    [Serializable]
    public class EadonBaseStatValueDictionary : SerializableDictionary<string, EadonBaseStatSkillValue>
    {
    }

    [Serializable]
    public class EadonBaseResistPercentDictionary : SerializableDictionary<string, EadonBaseResistPercent>
    {
    }

    [Serializable]
    public class EadonConditionDictionary : SerializableDictionary<string, BaseCondition>
    {
    }

    #endregion
}

#endif
