#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using Invector;
using UnityEngine;
#if EADON_USE_MALBERS
using MalbersAnimations;
#endif

namespace Eadon.Rpg.Invector.Environment
{
    public class DamageAreaTrackedEntity
    {
        public GameObject gameObject;
        public float quarterSecondTimer;
        public float halfSecondTimer;
        public float secondTimer;
        public float twoSecondTimer;
    }
    
    public class EadonDamageArea : MonoBehaviour
    {
        public LayerMask targetLayerMask;
        public List<string> tags;        
        
        public int damage;
        public int damagePerQuarterSecond;
        public int damagePerHalfSecond;
        public int damagePerSecond;
        public int damagePerTwoSecond;
        public int reactionId; // Change to desired reaction id
        public int recoilId; //change to desired recoil id;
        public bool activeRagdollOnEntry = true; //if you want ragdoll
#if EADON_USE_MALBERS
        public int modeId = 3;
        public int ability = -1;
        public string statToModify = "Health";
#endif

        private Collider _trigger;
        private float _quarterSecondTimer;
        private float _halfSecondTimer;
        private float _secondTimer;
        private float _twoSecondTimer;
        //private IDamageable damageable;

        private Dictionary<GameObject, DamageAreaTrackedEntity> _trackedEntities;
        
        private void Awake()
        {
            _trigger = GetComponent<Collider>();
            _trackedEntities = new Dictionary<GameObject, DamageAreaTrackedEntity>();
        }

        private void Start()
        {
            if (_trigger == null)
            {
                Debug.LogError("DamageArea: Missing collider");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(((1<<other.gameObject.layer) & targetLayerMask) != 0 && CheckTags(other.gameObject.tag))
            {
                Debug.Log("HIT VALID TARGET");
                if (!_trackedEntities.ContainsKey(other.gameObject))
                {
                    ApplyDamage(other.transform, transform.position, damage, activeRagdollOnEntry);
                    var o = other.gameObject;
                    var trackedEntity = new DamageAreaTrackedEntity {gameObject = o, quarterSecondTimer = 0, halfSecondTimer = 0, secondTimer = 0, twoSecondTimer = 0};
                    _trackedEntities.Add(o, trackedEntity);
                }
            }
            else
            {
                Debug.Log("INVALID TARGET");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_trackedEntities.ContainsKey(other.gameObject))
            {
                _trackedEntities.Remove(other.gameObject);
            }
        }

        private void Update()
        {
            var t = Time.deltaTime;

            foreach (var trackedEntity in _trackedEntities.Values)
            {
                trackedEntity.quarterSecondTimer += t;
                trackedEntity.halfSecondTimer += t;
                trackedEntity.secondTimer += t;
                trackedEntity.twoSecondTimer += t;

                if (trackedEntity.quarterSecondTimer >= .25f)
                {
                    if (damagePerQuarterSecond > 0)
                    {
                        ApplyDamage(trackedEntity.gameObject.transform, Vector3.zero, damagePerQuarterSecond, false);
                    }
                    trackedEntity.quarterSecondTimer = 0f;
                }
                if (trackedEntity.halfSecondTimer >= .5f)
                {
                    if (damagePerHalfSecond > 0)
                    {
                        ApplyDamage(trackedEntity.gameObject.transform, Vector3.zero, damagePerHalfSecond, false);
                    }
                    trackedEntity.halfSecondTimer = 0f;
                }
                if (trackedEntity.secondTimer >= 1f)
                {
                    if (damagePerSecond > 0)
                    {
                        ApplyDamage(trackedEntity.gameObject.transform, Vector3.zero, damagePerSecond, false);
                    }
                    trackedEntity.secondTimer = 0f;
                }
                if (trackedEntity.twoSecondTimer >= 2f)
                {
                    if (damagePerTwoSecond > 0)
                    {
                        ApplyDamage(trackedEntity.gameObject.transform, Vector3.zero, damagePerTwoSecond, false);
                    }
                    trackedEntity.twoSecondTimer = 0f;
                }
            }
        }

        private void ApplyDamage(Transform target, Vector3 hitPoint, int damageValue, bool ragdoll)
        {
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
                    Value = new MalbersAnimations.Scriptables.FloatReference() { UseConstant = true, ConstantValue = damage },
                };
                acEnemy.ReceiveDamage(direction, gameObject, enemyStatModifier, false, true, null, true);

                enemyStatModifier.ModifyStat(enemyStats);
            }
            else
            {
#endif
            var d = new vDamage
            {
                hitReaction = true,
                sender = transform,
                hitPosition = hitPoint,
                receiver = target,
                damageValue = damageValue,
                reaction_id = reactionId,
                recoil_id = recoilId,
                activeRagdoll = ragdoll
            };


            target.gameObject.ApplyDamage( new vDamage(d));
#if EADON_USE_MALBERS
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
    }
}
#endif
