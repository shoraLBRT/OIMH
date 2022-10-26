#if EADON_RPG_INVECTOR
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Character Configuration", menuName = "Eadon RPG/New Character Configuration")]

    public class EadonRpgCharacterConfig : ScriptableObject
    {
        [SerializeField]
        private EadonRpgStat[] availableStats = new EadonRpgStat[0];
        [SerializeField]
        private EadonRpgSkill[] availableSkills = new EadonRpgSkill[0];
        [SerializeField]
        private EadonRpgRace[] availableRaces = new EadonRpgRace[0];
        [SerializeField]
        private EadonRpgClass[] availableClasses = new EadonRpgClass[0];
        [SerializeField]
        private EadonRpgAlignment[] availableAlignments = new EadonRpgAlignment[0];
        [SerializeField]
        private EadonRpgDamageType[] availableDamageTypes = new EadonRpgDamageType[0];
        [SerializeField]
        private EadonRpgTalent[] availableTalents = new EadonRpgTalent[0];

        public EadonRpgStat[] Stats => availableStats;

        public EadonRpgSkill[] Skills => availableSkills;

        public EadonRpgRace[] Races => availableRaces;

        public EadonRpgClass[] Classes => availableClasses;

        public EadonRpgAlignment[] Alignments => availableAlignments;

        public EadonRpgDamageType[] DamageTypes => availableDamageTypes;

        public EadonRpgTalent[] AvailableTalents => availableTalents;

        public string[] GetStatNames()
        {
            string[] statNames = new string[availableStats.Length];

            for (int i = 0; i < availableStats.Length; i++)
            {
                statNames[i] = availableStats[i].StatName;
            }

            return statNames;
        }

        public string[] GetSkillNames()
        {
            string[] skillNames = new string[availableSkills.Length];

            for (int i = 0; i < availableSkills.Length; i++)
            {
                skillNames[i] = availableSkills[i].SkillName;
            }

            return skillNames;
        }

        public string[] GetDamageTypeNames()
        {
            string[] damageTypeNames = new string[availableDamageTypes.Length];

            for (int i = 0; i < availableDamageTypes.Length; i++)
            {
                damageTypeNames[i] = availableDamageTypes[i].DamageTypeName;
            }

            return damageTypeNames;
        }

        public EadonRpgDamageType GetDamageType(string damageTypeName)
        {
            for (int i = 0; i < availableDamageTypes.Length; i++)
            {
                if(availableDamageTypes[i].DamageTypeName == damageTypeName)
                {
                    return availableDamageTypes[i];
                }
            }
            return null;
        }

        public int GetDamageTypeIndex(string damageTypeName)
        {
            for (int i = 0; i < availableDamageTypes.Length; i++)
            {
                if (availableDamageTypes[i].DamageTypeName == damageTypeName)
                {
                    return i;
                }
            }
            return -1;
        }

        public EadonRpgTalent GetTalent(string talentName)
        {
            for (int i = 0; i < availableTalents.Length; i++)
            {
                if (availableTalents[i].TalentName == talentName)
                {
                    return AvailableTalents[i];
                }
            }
            return null;
        }
    }
}
#endif
