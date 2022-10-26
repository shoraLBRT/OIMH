#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using Eadon.Rpg.Invector.Utils;

namespace Eadon.Rpg.Invector.Character
{
    [Serializable]
    public class CharacterSaveData
    {
        public int currentLevel;
        public int spentStatPoints;
        public int unspentStatPoints;
        public int spentSkillPoints;
        public int unspentSkillPoints;
        public int currentXp;
        public List<EadonTempStatSkillBuff> temporaryStatSkillBuffs;
        public List<EadonTempResistBuff> temporaryResistanceBuffs;
        public List<EadonBaseValue> permanentModifiers;
        public List<string> acquiredTalents;
        public EadonBaseStatValueDictionary stats;
        public EadonBaseStatValueDictionary skills;
        public string appearance;
        public int mana;
        public int maxMana;
#if EADON_USE_SURVIVAL
        public List<EadonTempSurvivalBuff> temporarySurvivalBuffs;
#endif
    }
}
#endif
