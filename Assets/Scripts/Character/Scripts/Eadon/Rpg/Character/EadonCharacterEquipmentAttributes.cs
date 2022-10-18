#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using Eadon.Rpg.Invector.Utils;
using UnityEngine;

namespace Eadon.Rpg.Invector.Character
{
    public class EadonCharacterEquipmentAttributes : MonoBehaviour
    {
        public List<EadonCoreBonus> core = new List<EadonCoreBonus>();
        public List<EadonStatBonus> statPoints = new List<EadonStatBonus>();
        public List<EadonSkillBonus> skillPoints = new List<EadonSkillBonus>();
        public List<EadonMagicDamage> resistance = new List<EadonMagicDamage>();
        [Tooltip("Charm person spells resistance (0-100%) from the equipment")]
        public int charmResistance = 0;
        [Tooltip("Hold person spells resistance (0-100%) from the equipment")]
        public int holdResistance = 0;
#if EADON_USE_SURVIVAL
        public float hungerResistance;
        public float thirstResistance;
        public float heatResistance;
        public float coldResistance;
#endif
        public EadonModifierSource bonusSource = EadonModifierSource.Armour;

        private EadonRpgCharacter _character;
        private bool _updated;

        private void Update()
        {
            if (_updated) return;
            if (_character) return;
            _character = GetComponentInParent<EadonRpgCharacter>();
            if (!_character) return;
            _character.RecalculateEquipmentBonuses(null, true);
            _updated = true;
        }  

        private void OnDisable()
        {
            if (!_character) return;
            _character.RecalculateEquipmentBonuses(this, false);
            _character = null;
            _updated = false;
        } 
    }
}
#endif
