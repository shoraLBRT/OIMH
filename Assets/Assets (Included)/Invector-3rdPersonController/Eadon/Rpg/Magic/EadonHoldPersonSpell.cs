#if EADON_RPG_INVECTOR
using UnityEngine;
using Eadon.Rpg.Invector.Character;

namespace Eadon.Rpg.Invector.Magic
{
    public class EadonHoldPersonSpell : EadonSpellBase
    {
        public float range = 10f;
        public float duration = 5f;
        public LayerMask ignoreLayers = 1;

        protected override void Start()
        {
            base.Start();

            ignoreLayers |= (1 << LayerMask.NameToLayer("Player"));
            
            if (spellcaster != null)
            {
                var origin = spellcaster.transform.position;
                origin.y += 1.2f;

                var actualRange = range * (useSpellScale ? spellScale : 1);
                var actualDuration = duration * (useSpellScale ? spellScale : 1);
                
                var ray = new Ray(origin, spellcaster.transform.forward);
                if (Physics.SphereCast(ray, 0.5f, out var hitInfo, actualRange, ignoreLayers))
                {
                    var targetCharacter = hitInfo.transform.GetComponent<EadonRpgCharacterBase>();
                    if (targetCharacter != null)
                    {
                        var valid = Random.Range(1, 101) > targetCharacter.charmResistance;
                        if (!valid)
                        {
                            return;
                        }
                        targetCharacter.FreezeController(true, actualDuration);
                    }
                }
            }
        }
    }
}
#endif
