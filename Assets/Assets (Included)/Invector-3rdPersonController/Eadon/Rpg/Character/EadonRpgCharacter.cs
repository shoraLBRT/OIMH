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
    public class EadonRpgCharacter : EadonRpgCharacterBase
    {
        protected vThirdPersonController controller;
        protected vThirdPersonInput input;
        protected vHUDController hudController;
        protected GameObject overweightUi;
        
        protected bool overweight;
        
        protected float defaultWalkSpeed = 2;
        protected float defaultRunningSpeed = 2;
        protected float defaultSprintSpeed = 6;
        protected float defaultCrouchSpeed = 2;
        
        protected bool defaultWalk;
        protected bool defaultJumpInput;
        protected bool defaultStrafeInput;
        protected bool defaultRollInput;
        protected bool defaultSprintInput;

        #region Unity Methods

        /// <summary>
        /// Find components, add on dead listener for the collectable drop.
        /// TODO
        /// </summary>
        protected override void Start()
        {
            base.Start();
            controller = GetComponent<vThirdPersonController>();
            input = GetComponent<vThirdPersonInput>();
            hudController = FindObjectOfType<vHUDController>();

            defaultWalk = controller.freeSpeed.walkByDefault;
            defaultWalkSpeed = controller.freeSpeed.walkSpeed;
            defaultRunningSpeed = controller.freeSpeed.runningSpeed;
            defaultSprintSpeed = controller.freeSpeed.sprintSpeed;
            defaultCrouchSpeed = controller.freeSpeed.crouchSpeed;

            if (input != null)
            {
                defaultJumpInput = input.jumpInput.useInput;
                defaultStrafeInput = input.strafeInput.useInput;
                defaultRollInput = input.rollInput.useInput;
                defaultSprintInput = input.sprintInput.useInput;
            }

            if (overweightUiPrefab != null)
            {
                overweightUi = Instantiate(overweightUiPrefab);
                overweightUi.SetActive(false);
            }

            controller.maxHealth = maxLife;
            controller.ChangeHealth(currentLife);
            controller.healthRecovery = lifeRegeneration;
            controller.maxStamina = maxStamina;

            UpdateEadonHud();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (overweight)
            {
                if (disableJumpWhileOverweight && Input.GetKeyDown(input.jumpInput.key))
                {
                    hudController.ShowText(jumpMessage);
                }

                if (disableRollWhileOverweight && Input.GetKeyDown(input.rollInput.key))
                {
                    hudController.ShowText(rollMessage);
                }

                if (disableSprintWhileOverweight && Input.GetKeyDown(input.sprintInput.key))
                {
                    hudController.ShowText(sprintMessage);
                }
            }

            if (EadonHudController.Instance != null)
            {
                EadonHudController.Instance.UpdateManaSlider(this);
            }
        }

        #endregion

        #region Character Management

        public override void FreezeController(bool freezeRigidbody, float duration = 5)
        {
            base.FreezeController(freezeRigidbody, duration);
            input.SetLockAllInput(true);
        }

        public override void UnfreezeController()
        {
            base.UnfreezeController();
            input.SetLockAllInput(false);
        }
        
        public override void ResetCharacter()
        {
            if (controller == null)
            {
                controller = GetComponent<vThirdPersonController>();
            }

            base.ResetCharacter();
        }

        public override void AddMana(int manaIncrease)
        {
            base.AddMana(manaIncrease);
            UpdateEadonHud();
        }

        public override void UseMana(int manaCost)
        {
            base.UseMana(manaCost);
            UpdateEadonHud();
        }

        public override void AddXp(int value)
        {
            base.AddXp(value);
            UpdateEadonHud();
        }

        public override void SetManaMax()
        {
            base.SetManaMax();
            UpdateEadonHud();
        }

        protected override void CalculateMaxLife(bool updateToMax)
        {
            base.CalculateMaxLife(updateToMax);

            controller.maxHealth = maxLife;

            if (updateToMax)
            {
                currentLife = maxLife;
                if (controller != null)
                {
                    controller.ChangeHealth(currentLife);
                }
            }
        }

        protected override void CalculateMaxStamina()
        {
            base.CalculateMaxStamina();

            controller.maxStamina = maxStamina;
        }
        
        protected override void HandleManaSlider()
        {
            if (EadonHudController.Instance != null)
            {
                EadonHudController.Instance.UpdateManaSlider(this);
            }
        }
        
        protected override void SetupRegeneration(int regenerationAmount)
        {
            controller.healthRecovery = lifeRegeneration + lifeRegenerationBonus;
        }

        #endregion

        #region UI

        public virtual void UpdateEadonHud()
        {
            if (EadonHudController.Instance != null)
            {
                EadonHudController.Instance.UpdateManaSlider(this);
                EadonHudController.Instance.UpdateCharacterInfo(this);
            }
        }

        #endregion

        #region Inventory Management

        protected override void CheckCurrentEquipmentWeight(int itemWeight)
        {
            if (currentEquipmentLoad + itemWeight > maxEquipmentLoad)
            {
                if (enableWeightLimits)
                {
                    EnableOverweightSpeeds();
                    EnableOverweightInput();
                    if (!overweight)
                    {
                        hudController.ShowText(messageWhenOverweight);
                    }

                    overweight = true;
                    if (overweightUi != null)
                    {
                        overweightUi.SetActive(overweight);
                    }
                }
            }
            else
            {
                if (enableWeightLimits)
                {
                    ResetSpeeds();
                    ResetInput();
                    if (overweight)
                    {
                        hudController.ShowText(messageWhenNotOverweight);
                    }

                    overweight = false;
                    if (overweightUi != null)
                    {
                        overweightUi.SetActive(overweight);
                    }
                }
            }
        }

        void EnableOverweightSpeeds()
        {
            if (forceWalkWhileOverweight)
                controller.freeSpeed.walkByDefault = forceWalkWhileOverweight;
            controller.freeSpeed.walkSpeed = walkOverweightSpeed;
            controller.freeSpeed.runningSpeed = runningOverweightSpeed;
            controller.freeSpeed.sprintSpeed = sprintOverweightSpeed;
        }

        void ResetSpeeds()
        {
            controller.freeSpeed.walkByDefault = defaultWalk;
            controller.freeSpeed.walkSpeed = defaultWalkSpeed;
            controller.freeSpeed.runningSpeed = defaultRunningSpeed;
            controller.freeSpeed.sprintSpeed = defaultSprintSpeed;
            controller.freeSpeed.crouchSpeed = defaultCrouchSpeed;
        }

        void EnableOverweightInput()
        {
            input.jumpInput.useInput = !disableJumpWhileOverweight;
            input.rollInput.useInput = !disableRollWhileOverweight;
            input.strafeInput.useInput = !disableStrafeWhileOverweight;
            input.sprintInput.useInput = !disableSprintWhileOverweight;
        }

        void ResetInput()
        {
            if (input != null)
            {
                input.jumpInput.useInput = defaultJumpInput;
                input.rollInput.useInput = defaultRollInput;
                input.strafeInput.useInput = defaultStrafeInput;
                input.sprintInput.useInput = defaultSprintInput;
            }
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
                controller.ChangeHealth(currentLife);
            }

            if (magicDamage)
            {
                // found?
                var foundDblDoT = false;
                var foundDoT = false;
                var foundHalfDoT = false;
                var foundQtrDoT = false;
                foreach (var md in magicDamage.damage)
                {
                    if (md.value != 0)
                    {
                        // apply instant damage
                        currentLife -= MitigateDamage(md.DamageTypeName, md.value); // apply magic damage
                        controller.ChangeHealth(currentLife);
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
                    if (conditions.ContainsKey(md.DamageTypeName) &&
                        conditions[md.DamageTypeName].display != null) // has a condition
                    {
                        conditions[md.DamageTypeName].display.SetActive(true); // enable
                        conditions[md.DamageTypeName].length += conditionLength; // increase the length of the condition
                        foundDoT = true; // enable per second operations to count down the condition length
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
                {
                    // find targets of the correct type                    
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
                    if (listTargetsInRange != null) // fail safe
                    {
                        if (listTargetsInRange.Count > 0)
                        {
                            // targets found
                            foreach (EadonRpgCharacterBase cb in from tTarget in listTargetsInRange
                                where tTarget != transform
                                select tTarget.GetComponent<EadonRpgCharacterBase>()
                                into cb
                                where cb
                                select cb)
                            {
                                // found
                                cb.OnReceiveMagicalDamage(magicDamage, damage); // pass across the damage to target
                            }
                        }
                    }
                }

                // start per quarter second operations if DoTs added to the stack
                if (foundDblDoT && !isDamageEveryTwoSecondsRunning)
                {
                    isDamageEveryTwoSecondsRunning = true;
                    InvokeRepeating(nameof(EveryTwoSecondsOperations), 2f, 2f); // 1/4s delay, repeat every 1/4s
                }

                // start per second operations if DoTs added to the stack
                if (foundDoT && !isDamageEverySecondRunning)
                {
                    isDamageEverySecondRunning = true;
                    InvokeRepeating(nameof(EverySecondOperations), 1f, 1f); // 1s delay, repeat every 1s
                }

                // start per quarter second operations if DoTs added to the stack
                if (foundHalfDoT && !isDamageEveryHalfSecondRunning)
                {
                    isDamageEveryHalfSecondRunning = true;
                    InvokeRepeating(nameof(EveryHalfSecondOperations), .5f, .5f); // 1/4s delay, repeat every 1/4s
                }

                // start per quarter second operations if DoTs added to the stack
                if (foundQtrDoT && !isDamageEveryQuarterSecondsRunning)
                {
                    isDamageEveryQuarterSecondsRunning = true;
                    InvokeRepeating(nameof(EveryQuarterSecondOperations), .25f, .25f); // 1/4s delay, repeat every 1/4s
                }
            }

            // update the HUB display subscriber objects
            UpdateEadonHud();
        }

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
            else
            {
                UpdateEadonHud();
            }
        }

        /// <summary>
        /// Repeating coroutine applying DoT's and life/mana regen every second.
        /// </summary>
        public override void EverySecondOperations()
        {
            var bChangesApplied = false;

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

                    controller.ChangeHealth(currentLife);
                    bChangesApplied = true;
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
                        EadonHudController.Instance.UpdateManaSlider(this);
                    }

                    bChangesApplied = true;
                }
            }

            // damage over time
            if (!ApplyDamageOverTime(ref damageEverySecond) && !ApplyConditions() &&
                Math.Abs(manaRegeneration + manaRegenerationBonus + lifeRegeneration + lifeRegenerationBonus) < 0.001f)
            {
                CancelInvoke(nameof(EverySecondOperations)); // cancel if no DoTs or regen or active conditions
                isDamageEverySecondRunning = false;
            }
            else
            {
                bChangesApplied = true;
            }

            // update the on screen display (if any)
            if (bChangesApplied)
            {
                UpdateEadonHud();
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
            else
            {
                UpdateEadonHud();
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
            else
            {
                UpdateEadonHud();
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
            {
                // process DoT stack
                currentLife -=
                    MitigateDamage(md.DamageTypeName, md.dotValue > 0 ? md.dotValue : md.value); // apply magic damage
                controller.ChangeHealth(currentLife);

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

            whichDoT.RemoveAll(md => md.dotLength <= 0); // clear all with zero secs remaining
            UpdateEadonHud(); // update HUD
            return whichDoT.Count > 0; // only keep running if more DoTs in the stack
        }

        #endregion

        #region Saving & Loading

        public string GetSaveData()
        {
            var saveData = new CharacterSaveData();
            saveData.currentLevel = currentLevel;
            saveData.spentStatPoints = spentStatPoints;
            saveData.unspentStatPoints = unspentStatPoints;
            saveData.spentSkillPoints = spentSkillPoints;
            saveData.unspentSkillPoints = unspentSkillPoints;
            saveData.currentXp = currentXp;
            saveData.temporaryStatSkillBuffs = temporaryStatSkillBuffs;
            saveData.temporaryResistanceBuffs = temporaryResistanceBuffs;
#if EADON_USE_SURVIVAL
            saveData.temporarySurvivalBuffs = temporarySurvivalBuffs;
#endif
            saveData.permanentModifiers = permanentModifiers;
            saveData.acquiredTalents = acquiredTalents;
            var appearance = GetComponent<EadonCharacterAppearance>();
            if (appearance != null)
            {
                saveData.appearance = appearance.SaveData();
            }

            saveData.stats = stats;
            saveData.skills = skills;
            saveData.mana = currentMana;
            saveData.maxMana = maxMana;

            return StringSerialization.Serialize(typeof(CharacterSaveData), saveData);
        }

        public void LoadSaveData(string saveDataString)
        {
            var saveData =
                (CharacterSaveData)StringSerialization.Deserialize(typeof(CharacterSaveData), saveDataString);
            if (saveData != null)
            {
                currentLevel = saveData.currentLevel;
                spentStatPoints = saveData.spentStatPoints;
                unspentStatPoints = saveData.unspentStatPoints;
                spentSkillPoints = saveData.spentSkillPoints;
                unspentSkillPoints = saveData.unspentSkillPoints;
                currentXp = saveData.currentXp;
                temporaryStatSkillBuffs = saveData.temporaryStatSkillBuffs;
                temporaryResistanceBuffs = saveData.temporaryResistanceBuffs;
                permanentModifiers = saveData.permanentModifiers;
                acquiredTalents = saveData.acquiredTalents;

                baseArmourBonus = 0;
                baseDamageBonus = 0;
                baseLifeRegenerationBonus = 0;
                baseManaRegenerationBonus = 0;
                baseMaxLifeBonus = 0;
                baseMaxManaBonus = 0;
                baseMaxStaminaBonus = 0;

                stats = saveData.stats;
                skills = saveData.skills;
                currentMana = saveData.mana;
                maxMana = saveData.maxMana;

#if EADON_USE_SURVIVAL
                currentHungerResistance = currentRace.RaceHungerResistance + currentClass.ClassHungerResistance;
                currentThirstResistance = currentRace.RaceThirstResistance + currentClass.ClassThirstResistance;
                currentHeatResistance = currentRace.RaceHeatResistance + currentClass.ClassHeatResistance;
                currentColdResistance = currentRace.RaceColdResistance + currentClass.ClassColdResistance;

                temporarySurvivalBuffs = saveData.temporarySurvivalBuffs;
#endif

                // build resistances and relink condition child game objects                
                resistances = new EadonBaseResistPercentDictionary();
                resistancesModifiersTotals = new EadonBaseResistPercentDictionary();
                var resistNames = eadonRpgCharacterConfig.GetDamageTypeNames();
                foreach (var t in resistNames)
                {
                    // insert resistance
                    resistances.Add(t, new EadonBaseResistPercent() { resist = t, value = 0 });
                    resistancesModifiersTotals.Add(t, new EadonBaseResistPercent() { resist = t, value = 0 });
                }

                modifiers = new List<EadonBaseValue>();
                RebuildModifiers(false);

                xpToNextLevel = CalculateXpToNextLevel(currentLevel);
                currentEquipmentLoad = CalculateCurrentEquipmentWeight();

                if (manaRegeneration == 0)
                {
                    manaRegeneration = 1;
                }

                var appearance = GetComponent<EadonCharacterAppearance>();
                if (appearance != null)
                {
                    if (!string.IsNullOrWhiteSpace(saveData.appearance))
                    {
                        appearance.LoadData(saveData.appearance);
                    }
                }
            }
        }

        #endregion
    }

    #endregion
}
#endif
