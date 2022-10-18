#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using Invector;
using Invector.vCharacterController;
using UnityEngine;

#if EADON_USE_MALBERS
using MalbersAnimations;
using MalbersAnimations.Utilities;
#endif

namespace Eadon.Rpg.Invector.Magic
{
    /// <summary>
    /// Magic projectile options and actions.
    /// </summary>
    public class EadonMagicProjectile : EadonSpellBase
    {
        public LayerMask targetLayerMask;
        public List<string> tags;        
        public bool isAOE;
        public float aoeRange;
        public int damage = 15;       
        public Transform overrideDamageSender;
        public int reactionId = 0; // Change to desired reaction id
        public int recoilId = 0; //change to desired recoil id;
        public bool activeRagdoll = true; //if you want ragdoll
        
#if EADON_USE_MALBERS
        public int modeId = 3;
        public int ability = -1;
        public string statToModify = "Health";
#endif

        private List<GameObject> _targetsHit = null;
        private vLockOn _lockOn;
        private bool _useInternal = true;
        private bool _destroyOnCollision;
        private float _destroyDelay;

        /// <summary>
        /// Occurs on activation, sets up the projectile for targeting.
        /// </summary>
        private void OnEnable()
        {
            if (overrideDamageSender != null)
            {
                _lockOn = (vLockOn) overrideDamageSender.GetComponent(typeof(vLockOn));
            }
            
            var ep = GetComponentInChildren<EadonProjectile>(true);
            if (ep != null)
            {
                if (_lockOn != null)
                {
                    if (_lockOn.currentTarget != null)
                    {
                        ep.target = _lockOn.currentTarget.gameObject;
                    }
                }

                _destroyOnCollision = ep.destroyOnCollision;
                _destroyDelay = ep.destroyDelay;
            }

#if EADON_USE_RFX1
            var tm = GetComponentInChildren<RFX1_TransformMotion>(true);
            if (tm != null)
            {
                tm.CollisionEnter += Rfx1CollisionEnter;
                if (_lockOn != null)
                {
                    if (_lockOn.currentTarget != null)
                    {
                        tm.Target = _lockOn.currentTarget.gameObject;
                    }
                }
                _useInternal = false;
            }
#endif
            
#if EADON_USE_RFX3
            var es = GetComponentInChildren<EffectSettings>(true);
            if (es != null)
            {
                es.CollisionEnter += Rfx3OnCollisionEnter;
                if (_lockOn != null)
                {
                    if (_lockOn.currentTarget != null)
                    {
                        es.Target = _lockOn.currentTarget.gameObject;
                    }
                }
                _useInternal = false;
            }
#endif
           
#if EADON_USE_RFX4
            var physicsMotion = GetComponentInChildren<RFX4_PhysicsMotion>(true);
            if (physicsMotion != null) 
            {
                physicsMotion.CollisionEnter += Rfx4CollisionEnter;
                _useInternal = false;
            }

            var raycastCollision = GetComponentInChildren<RFX4_RaycastCollision>(true);
            if(raycastCollision != null) 
            {
                raycastCollision.CollisionEnter += Rfx4CollisionEnter;
                _useInternal = false;
            }
#endif

#if EADON_USE_AE
            var aepm = GetComponentInChildren<AE_PhysicsMotion>(true);
            if (aepm!=null)
            {
                aepm.CollisionEnter += AeOnCollisionEnter;
                _useInternal = false;
            }
#endif

#if EADON_USE_MALBERS
            var _enemyStatModifier = new StatModifier()
            {
                ID = MTools.GetInstance<StatID>(statToModify),
                modify = StatOption.SubstractValue,
                Value = new MalbersAnimations.Scriptables.FloatReference() { UseConstant = true, ConstantValue = damage },
            };
#endif
        }

#if EADON_USE_RFX1
        private void Rfx1CollisionEnter(object sender, RFX1_TransformMotion.RFX1_CollisionInfo e)
        {
            Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
            Hit(e.Hit.transform.gameObject, e.Hit.point);
        }
#endif

#if EADON_USE_RFX3
        private void Rfx3OnCollisionEnter(object sender, CollisionInfo e)
        {
            Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
            Hit(e.Hit.transform.gameObject, e.Hit.point);
        }
#endif

#if EADON_USE_RFX4
        private void Rfx4CollisionEnter(object sender, RFX4_PhysicsMotion.RFX4_CollisionInfo e)
        {
            Debug.Log(e.HitPoint); //a collision coordinates in world space
            Debug.Log(e.HitGameObject.name); //a collided gameobject
            Debug.Log(e.HitCollider.name); //a collided collider :)
            Hit(e.HitGameObject, e.HitPoint);
        }
#endif


#if EADON_USE_AE
        private void AeOnCollisionEnter(object sender, AE_PhysicsMotion.AE_CollisionInfo e)
        {
            Debug.Log(e.ContactPoint.otherCollider.transform.name); //will print collided object name to the console.
            Hit(e.ContactPoint.otherCollider.transform.gameObject, e.ContactPoint.point);
        }
#endif

        public void Hit(GameObject target, Vector3 hitPoint)
        {
            if (_targetsHit == null)
            {
                _targetsHit = new List<GameObject>();
            }
            
            if(targetLayerMask == (targetLayerMask | (1 << target.layer)) && CheckTags(target.gameObject.tag))
            {
                Debug.Log("HIT VALID TARGET" + target.name);
                if (!_targetsHit.Contains(target.gameObject))
                {
                    ApplyDamage(target.transform, transform.position);
                    _targetsHit.Add(target.gameObject);
                }
            }
            else
            {
                Debug.Log("INVALID TARGET: " + target.name);
                return;
            }

            if (isAOE)
            {
                var aoeTargets = Physics.SphereCastAll(hitPoint, aoeRange, Vector3.up, 0.1f, targetLayerMask);
                foreach (var aoeTarget in aoeTargets)
                {
                    if(((1<<aoeTarget.transform.gameObject.layer) & targetLayerMask) != 0 && CheckTags(aoeTarget.transform.gameObject.tag))
                    {
                        Debug.Log("HIT VALID AOE TARGET: " + aoeTarget.transform.gameObject.name);
                        if (!_targetsHit.Contains(aoeTarget.transform.gameObject))
                        {
                            ApplyDamage(aoeTarget.transform, transform.position);
                            _targetsHit.Add(aoeTarget.transform.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("INVALID AOE TARGET: " + aoeTarget.transform.gameObject.name);
                    }
                }
            }
        }
        
        protected virtual void ApplyDamage(Transform target, Vector3 hitPoint)
        {
            var d = new vDamage();
            d.hitReaction = true;
            d.sender = overrideDamageSender? overrideDamageSender : transform;
            d.hitPosition = hitPoint;
            d.receiver = target;
            d.damageValue = Mathf.CeilToInt(spellScale * damage);
            d.sender = spellcaster != null ? spellcaster.transform : null;

            d.reaction_id = reactionId;
            d.recoil_id = recoilId;
            d.activeRagdoll = activeRagdoll;
        
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
            }
#endif

        }

        protected bool CheckTags(string gameObjectTag)
        {
            if (tags == null || tags.Count == 0)
            {
                return true;
            }
            else
            {
                return tags.Contains(gameObjectTag);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_useInternal)
            {
                Hit(other.gameObject, other.contacts[0].point);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_useInternal)
            {
                Hit(other.gameObject, Vector3.zero);
                if (_destroyOnCollision)
                {
                    Destroy(gameObject, _destroyDelay);
                }
            }
        }
    }
}

#endif
