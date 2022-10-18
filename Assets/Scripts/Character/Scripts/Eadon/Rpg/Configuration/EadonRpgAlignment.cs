#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Utils;
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Alignment", menuName = "Eadon RPG/New Alignment")]
    public class EadonRpgAlignment : ScriptableObject
    {
        [SerializeField]
        private string alignment = "";

        [SerializeField]
        private EadonBaseResistPercent[] alignmentResistances = new EadonBaseResistPercent[0];

        public string AlignmentName => alignment;

        public EadonBaseResistPercent[] GetAlignmentResistances()
        {
            EadonBaseResistPercent[] modifiers = new EadonBaseResistPercent[alignmentResistances.Length];

            for (int i = 0; i < alignmentResistances.Length; i++)
            {
                modifiers[i] = new EadonBaseResistPercent() { resist = alignmentResistances[i].resist, value = alignmentResistances[i].value, src = EadonModifierSource.Character };
            }

            return modifiers;
        }
    }
}
#endif
