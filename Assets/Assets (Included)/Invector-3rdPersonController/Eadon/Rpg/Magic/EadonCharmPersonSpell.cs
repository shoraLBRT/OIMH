#if EADON_RPG_INVECTOR
using UnityEngine;
#if EADON_USE_MALBERS || EADON_USE_HAP || EADON_USE_INVECTOR
using Eadon.AI.Controllers;
using Eadon.AI.Coordination;
using Eadon.Rpg.Invector.Character;
#endif

namespace Eadon.Rpg.Invector.Magic
{
    public class EadonCharmPersonSpell : EadonSpellBase
    {
        public float range = 10f;
        public float duration = 20;
        public string charmedState;
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
#if EADON_USE_MALBERS || EADON_USE_HAP || EADON_USE_INVECTOR
                    var targetCollaboration = hitInfo.transform.GetComponent<EadonAiCollaborationBase>();
                    var targetAiController = hitInfo.transform.GetComponent<EadonAiControllerBase>();
                    var playerCollaboration = spellcaster.GetComponent<EadonAiCollaborationBase>();
                    var targetCharacter = hitInfo.transform.GetComponent<EadonRpgCharacterBase>();
                    
                    if (playerCollaboration != null && targetCollaboration != null)
                    {
                        if (playerCollaboration.isPartOfGroup)
                        {
                            if (targetCharacter != null)
                            {
                                var valid = Random.Range(1, 101) > targetCharacter.charmResistance;
                                if (!valid)
                                {
                                    return;
                                }
                            }
                            targetCollaboration.ChangeGroup(playerCollaboration.groupName, true, true, actualDuration);
                            targetAiController.SwitchToTempState(charmedState, duration * spellScale);
                        }
                    }
#endif
                }
            }
        }
    }
}
#endif
