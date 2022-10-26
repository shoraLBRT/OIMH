#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using Eadon.Rpg.Invector.Character;
using Invector;
using UnityEngine;
#if EADON_USE_MALBERS
using MalbersAnimations;
#endif

namespace Eadon.Rpg.Invector.Magic
{
    public class EadonDamageShieldSpell : EadonSpellBase
    {
        public LayerMask targetLayerMask;
        public List<string> tags;        
        public int damage = 15;       
        public Transform overrideDamageSender;
#if EADON_USE_MALBERS
        public int modeId = 3;
        public int ability = -1;
        public string statToModify = "Health";
#endif
        public int reactionId; // Change to desired reaction id
        public int recoilId; //change to desired recoil id;
        public bool activeRagdoll = true; //if you want ragdoll

        private List<GameObject> _targetsHit;

        private void OnCollisionEnter(Collision other)
        {
            Hit(other.gameObject, other.GetContact(0).point);
        }

        private void OnCollisionExit(Collision other)
        {
            if (_targetsHit.Contains(other.gameObject))
            {
                _targetsHit.Remove(other.gameObject);
            }
        }

        private void Hit(GameObject target, Vector3 hitPoint)
        {
            if (_targetsHit == null)
            {
                _targetsHit = new List<GameObject>();
            }
            
            if(((1<<target.gameObject.layer) & targetLayerMask) != 0 || CheckTag(target.gameObject.tag))
            {
                Debug.Log("HIT VALID TARGET");
                if (!_targetsHit.Contains(target.gameObject))
                {
                    ApplyDamage(target.transform, hitPoint);
                    _targetsHit.Add(target.gameObject);
                }
            }
            else
            {
                Debug.Log("INVALID TARGET");
            }
        }

        private bool CheckTag(string targetTag)
        {
            return tags.Contains(targetTag);
        }

        private void ApplyDamage(Transform target, Vector3 hitPoint)
        {
            var d = new vDamage
            {
                hitReaction = true,
                sender = overrideDamageSender ? overrideDamageSender : transform,
                hitPosition = hitPoint,
                receiver = target,
                damageValue = Mathf.CeilToInt(spellScale * damage),
                reaction_id = reactionId,
                recoil_id = recoilId,
                activeRagdoll = activeRagdoll
            };


            target.gameObject.ApplyDamage( new vDamage(d));

#if EADON_USE_EMERALD
            if (target.gameObject.GetComponent<EmeraldAI.EmeraldAISystem>() != null)
            {
                target.gameObject.GetComponent<EmeraldAI.EmeraldAISystem>().Damage(d.damageValue, EmeraldAI.EmeraldAISystem.TargetType.Player);
            }
#endif

#if EADON_USE_MALBERS
            var acEnemy = target.GetComponentInParent<IMDamage>();                           //Get the Animal on the Other collider
            if (acEnemy != null)
            {
                Vector3 direction = (transform.position - target.position).normalized;    //Calculate the direction of the attack
                acEnemy.HitDirection = direction;
                Stats enemyStats = target.GetComponentInParent<Stats>();
                var enemyStatModifier = new StatModifier()
                {
                    ID = MTools.GetInstance<StatID>(statToModify),
                    modify = StatOption.SubstractValue,
                    Value = new MalbersAnimations.Scriptables.FloatReference() { UseConstant = true, ConstantValue = Mathf.CeilToInt(spellScale * damage) },
                };
                acEnemy.ReceiveDamage(direction, gameObject, enemyStatModifier, false, true, null, true);

                enemyStatModifier.ModifyStat(enemyStats);
            }
#endif
        }
    }
}
#endif
