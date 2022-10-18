#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using CogsAndGoggles.Library.Utilities.FullSerializer;
using Eadon.Rpg.Invector.ClothingSystem;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Magic;
using Eadon.Rpg.Invector.UI;
using Eadon.Rpg.Invector.Utils;
#if EADON_USE_SURVIVAL
using Eadon.Survival.Invector.Data;
#endif
using Invector;
using Invector.vCharacterController;
using Invector.vCharacterController.AI;
using Invector.vItemManager;
using Invector.vMelee;
using UnityEngine;
using UnityEngine.UI;
using vItemAttributes = Invector.vItemManager.vItemAttributes;

namespace Eadon.Rpg.Invector.Character
{
    #region "Base character class"

    /// <summary>
    /// base class for a character.
    /// </summary>
    public class EadonRpgSimpleNpc : EadonRpgCharacterBase
    {
        private vSimpleMeleeAI_Controller _controller;

        #region Unity Methods

        /// <summary>
        /// Find components, add on dead listener for the collectable drop.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _controller = GetComponent<vSimpleMeleeAI_Controller>();
        }

        #endregion

        #region Character Management

        public override void FreezeController(bool freezeRigidbody, float duration = 5)
        {
            base.FreezeController(freezeRigidbody, duration);
            _controller.enabled = false;
        }

        public override void UnfreezeController()
        {
            base.UnfreezeController();
            _controller.enabled = true;
        }
        
        public override void ResetCharacter()
        {
            if (_controller == null)
            {
                _controller = GetComponent<vSimpleMeleeAI_Controller>();
            }
            
            base.ResetCharacter();
        } 
        
        protected override void CalculateMaxLife(bool updateToMax)
        {
            base.CalculateMaxLife(updateToMax);
            _controller.maxHealth = maxLife;

            if (updateToMax)
            {
                currentLife = maxLife;
                if (_controller != null)
                {
                    _controller.ChangeHealth(currentLife);
                }
            }
        }

        protected override void HandleManaSlider()
        {
            
        }
        
        protected override void SetupRegeneration(int regenerationAmount)
        {
            _controller.healthRecovery = lifeRegeneration + lifeRegenerationBonus;
        }

        #endregion

        #region Damage
        
        /// <summary>
        /// Damage mitigation by elemental type.
        /// </summary>
        /// <param name="magicDamage">Magic damage data.</param>
        /// <param name="damage">Physical damage amount.</param>
        public override void OnReceiveMagicalDamage(EadonMagicObjectDamage magicDamage, float damage)
        {
            if (damage > 0)
            {
                var newDamage = MitigateDamage("Physical", damage);
                currentLife -= newDamage;
                _controller.ChangeHealth(currentLife);
            }
            if (magicDamage)
            {  // found?
                var foundDblDoT = false;
                var foundDoT = false;
                var foundHalfDoT = false;
                var foundQtrDoT = false;
                foreach (var md in magicDamage.damage)
                {
                    if (md.value != 0)
                    {
                        // apply instant damage
                        currentLife -= MitigateDamage(md.DamageTypeName, md.value);  // apply magic damage
                        _controller.ChangeHealth(currentLife);
                    }

                    // apply damage over time?
                    var conditionLength = 0f;
                    if (md.dotLength > 0)
                    {
                        switch (md.dotFrequency)
                        {
                            case EadonUpdateFrequency.TwoSeconds:
                                foundDblDoT = true;
                                conditionLength += 2f * md.dotLength; 
                                damageEveryTwoSeconds.Add(md);
                                break;
                            case EadonUpdateFrequency.HalfSecond:
                                foundHalfDoT = true;
                                conditionLength += 0.5f * md.dotLength;
                                damageEveryHalfSecond.Add(md);
                                break;
                            case EadonUpdateFrequency.QuarterSecond:
                                foundQtrDoT = true;
                                conditionLength += 0.25f * md.dotLength;
                                damageEveryQuarterSecond.Add(md);
                                break;
                            case EadonUpdateFrequency.WholeSecond:
                                foundDoT = true;
                                conditionLength += md.dotLength;
                                damageEverySecond.Add(md);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        } 
                    }

                    // attempt enable condition                        
                    if (conditions.ContainsKey(md.DamageTypeName) && conditions[md.DamageTypeName].display != null)  // has a condition
                    {
                        conditions[md.DamageTypeName].display.SetActive(true);  // enable
                        conditions[md.DamageTypeName].length += conditionLength;  // increase the length of the condition
                        foundDoT = true;  // enable per second operations to count down the condition length
                    }
                    else
                    {
                        if (EadonGlobalFunctions.debuggingMessages)
                        {
                            Debug.Log("No Condition for " + transform.name + " with " + md.DamageTypeName + " damage");
                        }
                    }
                }

                // area of effect damage
                if (magicDamage.aoeRadius > 0) // AOE enabled?
                {   // find targets of the correct type                    
                    List<Transform> listTargetsInRange;
                    switch (magicDamage.aoeTarget)
                    {
                        case SpellTarget.Friend:
                            listTargetsInRange = EadonGlobalFunctions.FindAllTargetsWithinRange(
                                transform.localPosition, magicDamage.aoeRadius,
                                friendLayers,
                                friendTags,
                                false, 0f, true);
                            break;
                        case SpellTarget.Enemy:
                            listTargetsInRange = EadonGlobalFunctions.FindAllTargetsWithinRange(
                                transform.localPosition, magicDamage.aoeRadius,
                                enemyLayers,
                                enemyTags,
                                false, 0f, true);
                            break;
                        case SpellTarget.Any:
                            listTargetsInRange = EadonGlobalFunctions.FindAllTargetsWithinRange(
                                transform.localPosition, magicDamage.aoeRadius,
                                allLayers,
                                allTags,
                                false, 0f, true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // clear AOE radius preventing endless loop
                    magicDamage.aoeRadius = 0;

                    // apply damage to all targets in range
                    if (listTargetsInRange != null)  // fail safe
                    {  
                        if (listTargetsInRange.Count > 0)
                        {
                            // targets found
                            foreach (EadonRpgCharacterBase cb in from tTarget in listTargetsInRange where tTarget != transform select tTarget.GetComponent<EadonRpgCharacterBase>() into cb where cb select cb)
                            {
                                // found
                                cb.OnReceiveMagicalDamage(magicDamage, damage);  // pass across the damage to target
                            }
                        }
                    }
                }

                // start per quarter second operations if DoTs added to the stack
                if (foundDblDoT && !isDamageEveryTwoSecondsRunning)
                {
                    isDamageEveryTwoSecondsRunning = true;
                    InvokeRepeating(nameof(EveryTwoSecondsOperations), 2f, 2f);  // 1/4s delay, repeat every 1/4s
                }

                // start per second operations if DoTs added to the stack
                if (foundDoT && !isDamageEverySecondRunning)
                {
                    isDamageEverySecondRunning = true;
                    InvokeRepeating(nameof(EverySecondOperations), 1f, 1f);  // 1s delay, repeat every 1s
                }

                // start per quarter second operations if DoTs added to the stack
                if (foundHalfDoT && !isDamageEveryHalfSecondRunning)
                {
                    isDamageEveryHalfSecondRunning = true;
                    InvokeRepeating(nameof(EveryHalfSecondOperations), .5f, .5f);  // 1/4s delay, repeat every 1/4s
                }

                // start per quarter second operations if DoTs added to the stack
                if (foundQtrDoT && !isDamageEveryQuarterSecondsRunning)
                {
                    isDamageEveryQuarterSecondsRunning = true;
                    InvokeRepeating(nameof(EveryQuarterSecondOperations), .25f, .25f);  // 1/4s delay, repeat every 1/4s
                }
            }

        }  // add damage including elemental and update the HUD
        
        #endregion

        #region Damage Over Time

        /// <summary>
        /// Repeating coroutine applying DoT's every 2 seconds.
        /// </summary>
        public override void EveryTwoSecondsOperations()
        {
            if (!ApplyDamageOverTime(ref damageEveryTwoSeconds))
            {
                CancelInvoke(nameof(EveryTwoSecondsOperations));
                isDamageEveryTwoSecondsRunning = false;
            }
        }

        /// <summary>
        /// Repeating coroutine applying DoT's and life/mana regen every second.
        /// </summary>
        public override void EverySecondOperations()
        {
            // apply life if needed
            if (lifeRegeneration + lifeRegenerationBonus > 0)
            {
                if (currentLife < maxLife + maxLifeBonus)
                {
                    currentLife += lifeRegeneration + lifeRegenerationBonus;
                    if (currentLife > maxLife + maxLifeBonus)
                    {
                        currentLife = maxLife + maxLifeBonus;
                    }
                    _controller.ChangeHealth(currentLife);
                }
            }

            // and mana
            if (manaRegeneration + manaRegenerationBonus > 0)
            {
                if (currentMana < maxMana + maxManaBonus)
                {
                    currentMana += manaRegeneration + manaRegenerationBonus;
                    if (currentMana > maxMana + maxManaBonus)
                    {
                        currentMana = maxMana + maxManaBonus;
                    }
                }
            }

            // damage over time
            if (!ApplyDamageOverTime(ref damageEverySecond) && !ApplyConditions() && Math.Abs(manaRegeneration + manaRegenerationBonus + lifeRegeneration + lifeRegenerationBonus) < 0.001f)
            {
                CancelInvoke(nameof(EverySecondOperations));  // cancel if no DoTs or regen or active conditions
                isDamageEverySecondRunning = false;
            }
        }

        /// <summary>
        /// Repeating coroutine applying DoT's every half second.
        /// </summary>
        public override void EveryHalfSecondOperations()
        {
            if (!ApplyDamageOverTime(ref damageEveryHalfSecond))
            {
                CancelInvoke(nameof(EveryHalfSecondOperations));
                isDamageEveryHalfSecondRunning = false;
            }
        }

        /// <summary>
        /// Repeating coroutine applying DoT's every quarter second.
        /// </summary>
        public override void EveryQuarterSecondOperations()
        {
            if (!ApplyDamageOverTime(ref damageEveryQuarterSecond))
            {
                CancelInvoke(nameof(EveryQuarterSecondOperations));
                isDamageEveryQuarterSecondsRunning = false;
            }
        }

        /// <summary>
        /// Apply DoT's, called from per second operations repeating coroutines.
        /// </summary>
        /// <param name="whichDoT">Type of damage over time (and frequency) to apply via the mitigation function.</param>
        /// <returns></returns>
        protected override bool ApplyDamageOverTime(ref List<EadonMagicDamageOverTime> whichDoT)
        {
            if (whichDoT == null) return false; // return DoTs all run, cancel invoke
            // fail safe
            if (whichDoT.Count <= 0) return false; // return DoTs all run, cancel invoke
            // DoTs still in stack
            foreach (var md in whichDoT)
            {  // process DoT stack
                currentLife -= MitigateDamage(md.DamageTypeName, md.dotValue > 0 ? md.dotValue : md.value);  // apply magic damage
                _controller.ChangeHealth(currentLife);

                // reduce the remaining seconds for the DoT
                switch (md.dotFrequency)
                {
                    case EadonUpdateFrequency.TwoSeconds:
                        md.dotLength -= 2f;
                        break;
                    case EadonUpdateFrequency.HalfSecond:
                        md.dotLength -= 0.5f;
                        break;
                    case EadonUpdateFrequency.QuarterSecond:
                        md.dotLength -= 0.25f;
                        break;
                    case EadonUpdateFrequency.WholeSecond:
                        md.dotLength -= 1f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            whichDoT.RemoveAll(md => md.dotLength <= 0);  // clear all with zero secs remaining
            return whichDoT.Count > 0;  // only keep running if more DoTs in the stack
        }

        #endregion
    }

    #endregion
}
#endif
