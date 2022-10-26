#if EADON_RPG_INVECTOR
using Invector;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public class EadonHealingSpell : EadonSpellBase
    {
        public int amount = 10;

        protected override void Start()
        {
            base.Start();
            if (spellcaster != null)
            {
                var healthController = spellcaster.GetComponent<vHealthController>();
                healthController.AddHealth(amount);
                character.currentLife = Mathf.CeilToInt(healthController.currentHealth);
            }
        }
    }
}
#endif
