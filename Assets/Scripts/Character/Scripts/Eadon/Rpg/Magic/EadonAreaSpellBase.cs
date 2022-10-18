#if EADON_RPG_INVECTOR
using System.Linq;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Utils;
using Invector;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public abstract class EadonAreaSpellBase : EadonSpellBase
    {
        public float radius;
        public LayerMask targetLayers;
        public string[] targetTags;

        protected override void Start()
        {
            base.Start();
            ExecuteSpell();
        }

        protected void ExecuteSpell()
        {
            var targetsInRange = EadonGlobalFunctions.FindAllTargetsWithinRange(transform.position, radius, targetLayers, targetTags, false, 1f, true);

            foreach (var target in targetsInRange)
            {
                var targetHealthController = target.GetComponent<vHealthController>();
                if (targetHealthController != null)
                {
                    var targetCharacter = target.GetComponent<EadonRpgCharacter>();
                    ApplySpellEffectOnTarget(targetHealthController, targetCharacter);
                }
            }
        }

        protected abstract void ApplySpellEffectOnTarget(vHealthController targetHealthController, EadonRpgCharacter targetCharacter);
    }
}
#endif
