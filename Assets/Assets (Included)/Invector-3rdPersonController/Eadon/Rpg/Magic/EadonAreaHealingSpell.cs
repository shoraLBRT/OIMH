#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Utils;
using Invector;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    /// <summary>
    /// Magic heal for projectiles or radius spells.
    /// </summary>
    public class EadonAreaHealingSpell : EadonAreaSpellBase
    {
        public float amount = 10;

        protected override void ApplySpellEffectOnTarget(vHealthController targetHealthController, EadonRpgCharacter targetCharacter)
        {
            if (targetHealthController != null)
            {
                targetHealthController.AddHealth(Mathf.CeilToInt(amount));
                if (targetCharacter != null)
                {
                    character.currentLife = Mathf.CeilToInt(targetHealthController.currentHealth);
                }
            }
        }
    }
}

#endif
