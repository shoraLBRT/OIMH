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
#if EADON_USE_SURVIVAL
    public class EadonRpgCharacterBase : MonoBehaviour, IResistanceProvider
#else
    public class EadonRpgCharacterBase : MonoBehaviour
#endif
    {
        public EadonRpgDefaultValues eadonRpgDefaultValues;
        public EadonRpgCharacterConfig eadonRpgCharacterConfig;

        public EadonRpgAlignment currentAlignment;
        public EadonRpgRace currentRace;
        public EadonRpgClass currentClass;

        public string characterName;
        
        public int currentLevel;
        public int currentXp;
        public int xpToNextLevel;
        
        public int currentLife;
        public int maxLife;
        public int lifeRegeneration;
        public int baseMaxLifeBonus;
        public int baseLifeRegenerationBonus;
        public int maxLifeBonus;
        public int lifeRegenerationBonus;
        
        public int currentMana;
        public int maxMana;
        public int manaRegeneration = 1;
        public int baseMaxManaBonus;
        public int baseManaRegenerationBonus;
        public int maxManaBonus;
        public int manaRegenerationBonus;
        
        public int maxStamina;

        public int baseMaxStaminaBonus;
        public int maxStaminaBonus;
        
        public float maxEquipmentLoad;
        public float baseMaxEquipmentLoadBonus;
        public float currentEquipmentLoad;
        
        public float currentArmour;
        public float baseArmourBonus;
        public float armourBonus;
        
        public int damageBonus;
        public int baseDamageBonus;
        public int currentDamageBonus;

        public EadonBaseStatValueDictionary stats;
        public int unspentStatPoints;
        public int spentStatPoints;
        
        public EadonBaseStatValueDictionary skills;
        public int unspentSkillPoints;
        public int spentSkillPoints;

        public int talentsAvailable;

        public EadonBaseResistPercentDictionary resistances;
        
        public List<EadonBaseValue> modifiers;
        public List<EadonBaseValue> permanentModifiers;
        public EadonBaseStatValueDictionary statsModifierTotals;
        public EadonBaseStatValueDictionary skillsModifiersTotals;
        public EadonBaseResistPercentDictionary resistancesModifiersTotals;

        public List<string> acquiredTalents;
        
        public List<EadonTempStatSkillBuff> temporaryStatSkillBuffs;
        public List<EadonTempResistBuff> temporaryResistanceBuffs;
#if EADON_USE_SURVIVAL
        public List<EadonTempSurvivalBuff> temporarySurvivalBuffs;
#endif
        public List<EadonMagicDamageOverTime> damageEveryTwoSeconds;
        public List<EadonMagicDamageOverTime> damageEverySecond;
        public List<EadonMagicDamageOverTime> damageEveryHalfSecond;
        public List<EadonMagicDamageOverTime> damageEveryQuarterSecond;

        public Transform conditionsRoot;
        public EadonConditionDictionary conditions;

        public bool enableWeightLimits = true;
        public string messageWhenOverweight = "You're now overweight";
        public string messageWhenNotOverweight = "You're now carrying normal weight";
        public bool forceWalkWhileOverweight = true;
        public bool disableStrafeWhileOverweight = true;
        public bool disableJumpWhileOverweight = true;
        public string jumpMessage = "I'm to heavy to jump!";
        public bool disableRollWhileOverweight = true;
        public string rollMessage = "I'm to heavy to roll!";
        public bool disableSprintWhileOverweight = true;
        public string sprintMessage = "I'm to heavy to sprint!";
        public float walkOverweightSpeed = 1;
        public float runningOverweightSpeed = 2;
        public float sprintOverweightSpeed = 4;

        public GameObject overweightUiPrefab;

        public bool debuggingMessages;

        public Transform magicSpawnPoint;
        public Slider manaSlider;
        public LayerMask friendLayers = (1 << 8) | (1 << 10);
        public string[] friendTags = new string[] { "Player", "CompanionAI" };
        public LayerMask enemyLayers = (1 << 8) | (1 << 9) | (1 << 10) | (1 << 17);
        public string[] enemyTags = new string[] { "Enemy", "Boss" };
        public LayerMask allLayers = (1 << 9) | (1 << 17);
        public string[] allTags = new string[] { "Player", "CompanionAI", "Enemy", "Boss" };

        public float spellCooldown = 5f;

        public int charmResistance = 0;
        public int baseCharmResistance = 0;
        public int holdResistance = 0;
        public int baseHoldResistance = 0;

#if EADON_USE_SURVIVAL
        public float currentHungerResistance;
        public float currentThirstResistance;
        public float currentHeatResistance;
        public float currentColdResistance;
        public float equipHungerResistance;
        public float equipThirstResistance;
        public float equipHeatResistance;
        public float equipColdResistance;
#endif

        public bool isFrozen;
        private float _animatorSpeedPreFreeze;
        private float _freezeDuration;
        private float _freezeTimer;
        
        protected bool isDamageEveryTwoSecondsRunning;
        protected bool isDamageEverySecondRunning;
        protected bool isDamageEveryHalfSecondRunning;
        protected bool isDamageEveryQuarterSecondsRunning;

        protected vItemManager characterItemManager;
        protected Animator animator;

        protected float spellCooldownTimer;

        protected static readonly int MagicIdParameter = Animator.StringToHash("MagicID");
        protected static readonly int MagicAttackTrigger = Animator.StringToHash("MagicAttack");
        
        public ClothingManager clothingManager;

        #region Unity Methods

        /// <summary>
        /// Find components, add on dead listener for the collectable drop.
        /// </summary>
        protected virtual void Start()
        {
            if (stats == null || stats.Count == 0)
            {
                ResetCharacter();
            }

            currentEquipmentLoad = CalculateCurrentEquipmentWeight();

            characterItemManager = GetComponent<vItemManager>();
            animator = GetComponent<Animator>();
            
            spellCooldownTimer = spellCooldown;

            if (clothingManager != null)
            {
                clothingManager.Init();
            }
        }

        protected virtual void Update()
        {
            spellCooldownTimer += Time.deltaTime;
            
            if (isFrozen)
            {
                if (_freezeDuration > 0)
                {
                    _freezeTimer += Time.deltaTime;
                    if (_freezeTimer > _freezeDuration)
                    {
                        UnfreezeController();
                        _freezeTimer = 0;
                    }
                }
            }
        }
        
        protected virtual void LateUpdate()
        {
            UpdateBuffs();
        }

        /// <summary>
        /// Enable repeating operations when enabled.
        /// </summary>
        public void OnEnable()
        {
            damageEveryTwoSeconds = new List<EadonMagicDamageOverTime>();
            damageEverySecond = new List<EadonMagicDamageOverTime>();
            damageEveryHalfSecond = new List<EadonMagicDamageOverTime>();
            damageEveryQuarterSecond = new List<EadonMagicDamageOverTime>();

            if (!Application.isPlaying /*||
                !(lifeRegeneration + lifeRegenerationBonus > 0) || !(manaRegeneration + manaRegenerationBonus > 0)*/) return;
            isDamageEverySecondRunning = true;
            InvokeRepeating(nameof(EverySecondOperations), 1f, 1f);
        }  

        /// <summary>
        /// Disable repeating operations when disabled failsafe.
        /// </summary>
        public virtual void OnDisable()
        {
            // ensure per second operations are disabled
            if (isDamageEveryTwoSecondsRunning)
            {
                CancelInvoke(nameof(EveryTwoSecondsOperations));
                isDamageEveryTwoSecondsRunning = false;
            }
            if (isDamageEverySecondRunning)
            {
                CancelInvoke(nameof(EverySecondOperations));
                isDamageEverySecondRunning = false;
            }
            if (isDamageEveryHalfSecondRunning)
            {
                CancelInvoke(nameof(EveryHalfSecondOperations));
                isDamageEveryHalfSecondRunning = false;
            }
            if (isDamageEveryQuarterSecondsRunning)
            {
                CancelInvoke(nameof(EveryQuarterSecondOperations));
                isDamageEveryQuarterSecondsRunning = false;
            }
        }  

        #endregion

        #region Character Management

        public virtual void FreezeController(bool freezeRigidbody, float duration = 5f)
        {
            if (freezeRigidbody)
            {
                var mRigidbody = GetComponent<Rigidbody>();
                mRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                mRigidbody.isKinematic = true;
                mRigidbody.useGravity = false;
            }

            var mAnimator = GetComponent<Animator>();
            _animatorSpeedPreFreeze = mAnimator.speed;
            mAnimator.speed = 0f;
            isFrozen = true;
            _freezeDuration = duration;
        }

        public virtual void UnfreezeController()
        {
            var mRigidbody = GetComponent<Rigidbody>();
            var mAnimator = GetComponent<Animator>();
            
            isFrozen = false;
            mRigidbody.isKinematic = false;
            mRigidbody.useGravity = true;
            mAnimator.speed = _animatorSpeedPreFreeze;
        }

        
        public virtual void ResetCharacter()
        {
            if (acquiredTalents != null && acquiredTalents.Count > 0)
            {
                foreach (var talentName in acquiredTalents)
                {
                    var talent = eadonRpgCharacterConfig.GetTalent(talentName);
                    if (talent != null && talent.Type == TalentType.SpellLikeAbility)
                    {
                        RemoveSpell(talent.SpellLikeAbility);
                    }
                    if (talent != null && talent.Type == TalentType.AddItems)
                    {
                        foreach (var item in talent.Items)
                        {
                            RemoveItem(item);
                        }
                    }
                }
            }
            
            // core
            currentLevel = 1;
            unspentStatPoints = eadonRpgDefaultValues.InitialUnspentStatPointsDefault;
            unspentSkillPoints = eadonRpgDefaultValues.InitialUnspentSkillPointsDefault;
            currentXp = 0;
            xpToNextLevel = CalculateXpToNextLevel(currentLevel);
            currentEquipmentLoad = CalculateCurrentEquipmentWeight();

            baseArmourBonus = 0;
            baseDamageBonus = 0;
            baseLifeRegenerationBonus = 0;
            baseManaRegenerationBonus = 0;
            baseMaxLifeBonus = 0;
            baseMaxManaBonus = 0;
            baseMaxStaminaBonus = 0;

#if EADON_USE_SURVIVAL
            currentHungerResistance = currentRace.RaceHungerResistance + currentClass.ClassHungerResistance;
            currentThirstResistance = currentRace.RaceThirstResistance + currentClass.ClassThirstResistance;
            currentHeatResistance = currentRace.RaceHeatResistance + currentClass.ClassHeatResistance;
            currentColdResistance = currentRace.RaceColdResistance + currentClass.ClassColdResistance;
#endif
            
            talentsAvailable = eadonRpgDefaultValues.InitialTalents;

            // build Stats from defaults            
            stats = new EadonBaseStatValueDictionary();
            if (statsModifierTotals == null)
            {
                statsModifierTotals = new EadonBaseStatValueDictionary();
            }
            else
            {
                statsModifierTotals.Clear();
            }
            var statNames = eadonRpgCharacterConfig.GetStatNames();
            foreach (var t in statNames)
            {
                stats.Add(t, new EadonBaseStatSkillValue() { stat = t, value = eadonRpgDefaultValues.StatInitialValueDefault });
                statsModifierTotals.Add(t, new EadonBaseStatSkillValue() { stat = t, value = 0 });
            }

            // build Skills from defaults            
            skills = new EadonBaseStatValueDictionary();
            skillsModifiersTotals = new EadonBaseStatValueDictionary();
            var skillNames = eadonRpgCharacterConfig.GetSkillNames();
            foreach (var t in skillNames)
            {
                skills.Add(t, new EadonBaseStatSkillValue() { stat = t, value = eadonRpgDefaultValues.SkillInitialValueDefault });
                skillsModifiersTotals.Add(t, new EadonBaseStatSkillValue() { stat = t, value = 0 });
            }

            // build resistances and relink condition child game objects                
            conditions = new EadonConditionDictionary();
            resistances = new EadonBaseResistPercentDictionary();
            resistancesModifiersTotals = new EadonBaseResistPercentDictionary();
            var resistNames = eadonRpgCharacterConfig.GetDamageTypeNames();
            foreach (var t in resistNames)
            {
                resistances.Add(t, new EadonBaseResistPercent() { resist = t, value = 0 });
                resistancesModifiersTotals.Add(t, new EadonBaseResistPercent() { resist = t, value = 0 });
                var conditionGo = FindInActiveChild(conditionsRoot, t);
                if (conditionGo != null)
                {
                    if (!conditions.ContainsKey(t))
                    {
                        conditions.Add(t, new BaseCondition() { type = t, display =  conditionGo});
                    }
                }

                Debug.Log("Added resistance to " + t);
            }
            
            acquiredTalents = new List<string>();
            foreach (var raceTalent in currentRace.RaceTalents)
            {
                acquiredTalents.Add(raceTalent.TalentName);
            }
            foreach (var classTalent in currentClass.ClassTalents)
            {
                acquiredTalents.Add(classTalent.TalentName);
            }

            // rebuild the skill/resistance modifiers
            modifiers = new List<EadonBaseValue>();
            permanentModifiers = new List<EadonBaseValue>();
            RebuildModifiers(false);
            
            temporaryStatSkillBuffs = new List<EadonTempStatSkillBuff>();
            temporaryResistanceBuffs = new List<EadonTempResistBuff>();
#if EADON_USE_SURVIVAL
            temporarySurvivalBuffs = new List<EadonTempSurvivalBuff>();
#endif
        } 

        public void IncreaseStat(string statName)
        {
            if (unspentStatPoints > 0)
            {
                unspentStatPoints--;
                spentStatPoints++;
                stats[statName].value += 1;
                RebuildModifiers(true);
            }
        }

        public void IncreaseSkill(string skillName)
        {
            if (unspentSkillPoints > 0)
            {
                unspentSkillPoints--;
                spentSkillPoints++;
                skills[skillName].value += 1;
                RebuildModifiers(true);
            }
        }

        public virtual void AddMana(int manaIncrease)
        {
            currentMana += manaIncrease;
            if (currentMana > maxMana + maxManaBonus) currentMana = maxMana;  // limit mana gain to max mana
        }

        public virtual void UseMana(int manaCost)
        {
            currentMana -= manaCost;  // subtract the used mana
        }

        public virtual void AddXp(int value)
        {
            currentXp += value;
            if (currentXp > xpToNextLevel)
            {
                if (EadonHudController.Instance != null)
                {
                    EadonHudController.Instance.ShowLevelUpImage();
                }
                currentLevel += 1;
                if (currentLevel < eadonRpgDefaultValues.MaxLevelDefault)
                {
                    xpToNextLevel = CalculateXpToNextLevel(currentLevel);
                    unspentSkillPoints += eadonRpgDefaultValues.LevelUpSkillPointsDefault;
                    unspentStatPoints += eadonRpgDefaultValues.LevelUpStatPointsDefault;
                    if (currentLevel % eadonRpgDefaultValues.LevelsPerTalentDefault == 0)
                    {
                        talentsAvailable++;
                    }
                    RebuildModifiers(true);
                }
                else
                {
                    currentLevel = eadonRpgDefaultValues.MaxLevelDefault;
                }
            }
        }  

        public virtual void SetManaMax()
        {
            currentMana = maxMana + maxManaBonus;
        }

        public void RebuildModifiers(bool onlyModifiers)
        {
            // clear the list
            modifiers.RemoveAll(s => s.src == EadonModifierSource.Character || (s.src == EadonModifierSource.Talent));

            currentArmour = 0;

            // push race modifiers
            var raceStatMods = currentRace.GetStatModifiers();
            foreach (var mod in raceStatMods)
            {
                modifiers.Add(mod);
            }
            
            var raceSkillMods = currentRace.GetSkillModifiers();
            foreach (var mod in raceSkillMods)
            {
                modifiers.Add(mod);
            }
            
            foreach (var skill in eadonRpgCharacterConfig.Skills)
            {
                if (skill.RelatedStatName == null || !skill.StatBonusAddToSKill) continue;
                var bonus = eadonRpgDefaultValues.GetSkillBonusForStatValue(stats[skill.RelatedStatName].value);
                var mod = new EadonBaseStatSkillValue() { stat = skill.SkillName, value = bonus, src = EadonModifierSource.Character };
                modifiers.Add(mod);
            }
            
            var raceResist = currentRace.GetRaceResistances();
            foreach (var mod in raceResist)
            {
                Debug.Log($"Adding resistance to {mod.resist} from race");
                modifiers.Add(mod);
            }
            currentArmour += currentRace.RaceBaseArmour;

            // push class modifiers
            var classStatMods = currentClass.GetStatModifiers();
            foreach (var mod in classStatMods)
            {
                modifiers.Add(mod);
            }
            var classSkillMods = currentClass.GetSkillModifiers();
            foreach (var mod in classSkillMods)
            {
                modifiers.Add(mod);
            }
            var classResist = currentClass.GetClassResistances();
            foreach (var mod in classResist)
            {
                Debug.Log($"Adding resistance to {mod.resist} from class");
                modifiers.Add(mod);
            }
            currentArmour += currentClass.ClassBaseArmour;

            // push alignment modifiers
            var alignmentResist = currentAlignment.GetAlignmentResistances();
            foreach (var mod in alignmentResist)
            {
                Debug.Log($"Adding resistance to {mod.resist} from alignment");
                modifiers.Add(mod);
            }

            // push talent modifiers
            foreach (var talent in eadonRpgCharacterConfig.AvailableTalents)
            {
                if (acquiredTalents != null && acquiredTalents.Contains(talent.TalentName))
                {
                    if (talent.Type == TalentType.SpellLikeAbility)
                    {
                        if (!onlyModifiers)
                        {
                            talent.ApplyTo(this, true, onlyModifiers);
                        }
                    }
                    else
                    {
                        talent.ApplyTo(this, true, onlyModifiers);
                    }
                }
            }
            
            damageBonus = baseDamageBonus;

            // rebuild modifier cache
            RebuildModifierTotals(true); 
        }

        /// <summary>
        /// Formula to determine next level up.
        /// </summary>
        /// <param name="level">Current character level.</param>
        /// <returns>XP required to level up.</returns>
        public int CalculateXpToNextLevel(int level)
        {
            var baseXp = eadonRpgDefaultValues.LevelUpXpBaseDefault;
            var bandXp = eadonRpgDefaultValues.LevelUpXpBandCostDefault;

            var xpToNext = baseXp * level + bandXp * level * (level - 1);
            return xpToNext;
        }

        protected virtual void CalculateMaxLife(bool updateToMax)
        {
            maxLife = eadonRpgDefaultValues.BaseHpDefault + eadonRpgDefaultValues.HpPerLevelDefault * currentLevel;

            foreach (var stat in eadonRpgCharacterConfig.Stats)
            {
                if (stat.StatAffectsHP)
                {
                    maxLife += (int)(stats[stat.StatName].value + statsModifierTotals[stat.StatName].value) * eadonRpgDefaultValues.HpPerStatPointDefault;
                }
            }

            foreach (var modifier in permanentModifiers)
            {
                if (modifier is InvectorBaseStatSkillValue invectorModifier)
                {
                    if (invectorModifier.stat == InvectorStats.Health)
                    {
                        maxLife += Mathf.CeilToInt(invectorModifier.value);
                    }
                }
            }
        }

        /// <summary>
        /// Abstract formula to determine bonus resistance per level.
        /// </summary>
        protected virtual void CalculateResistanceLevelBonus()
        {
            var levelBonus = eadonRpgDefaultValues.ResistancePerLevelDefault * currentClass.ClassMagicResistanceMultiplier;
            // zero the resist cache totals
            foreach (var rp in resistancesModifiersTotals.Values)
            {
                rp.value = levelBonus * currentLevel;
            }

            // recalculate the resist totals
            foreach (var eadonBaseValue in modifiers.Where(t => t is EadonBaseResistPercent))
            {
                var mod = (EadonBaseResistPercent) eadonBaseValue;
                resistancesModifiersTotals[mod.resist].value += mod.value;
            }
        }

        /// <summary>
        /// Abstract formula to determine max stamina from stats.
        /// </summary>
        protected virtual void CalculateMaxStamina()
        {
            maxStamina = eadonRpgDefaultValues.BaseStaminaDefault;

            foreach (var modifier in permanentModifiers)
            {
                if (modifier is InvectorBaseStatSkillValue invectorModifier)
                {
                    if (invectorModifier.stat == InvectorStats.Stamina)
                    {
                        maxStamina += Mathf.CeilToInt(invectorModifier.value);
                    }
                }
            }

            foreach (var stat in eadonRpgCharacterConfig.Stats)
            {
                if (stat.StatAffectsStamina)
                {
                    maxStamina += (int)(stats[stat.StatName].value + statsModifierTotals[stat.StatName].value - currentEquipmentLoad);
                }
            }
        }

        /// <summary>
        /// Abstract formula to determine max mana from stats.
        /// </summary>
        /// <param name="updateToMax">Option to MAX out the mana attribute on upgrade.</param>
        protected virtual void CalculateMaxMana(bool updateToMax)
        {
            maxMana = eadonRpgDefaultValues.BaseManaDefault;

            foreach (var stat in eadonRpgCharacterConfig.Stats)
            {
                if (stat.StatAffectsMana)
                {
                    maxMana += (int)((stats[stat.StatName].value + statsModifierTotals[stat.StatName].value) * eadonRpgDefaultValues.ManaPerStatPointDefault);
                }
            }

            foreach (var modifier in permanentModifiers)
            {
                if (modifier is EadonBaseStatSkillValue eadonModifier)
                {
                    if (eadonModifier.stat == "Mana")
                    {
                        maxMana += Mathf.CeilToInt(eadonModifier.value);
                    }
                }
            }

            if (updateToMax)
            {
                currentMana = maxMana;
            }
            
            HandleManaSlider();
        }

        /// <summary>
        /// Abstract formula to determine max equip weight from stats.
        /// </summary>
        protected virtual void CalculateMaxEquipmentLoad()
        {
            maxEquipmentLoad = eadonRpgDefaultValues.BaseWeightLimitDefault + baseMaxEquipmentLoadBonus;

            foreach (var stat in eadonRpgCharacterConfig.Stats)
            {
                if (stat.StatAffectsWeightLimits)
                {
                    maxEquipmentLoad += (stats[stat.StatName].value + statsModifierTotals[stat.StatName].value) * eadonRpgDefaultValues.WeightIncreasePerStatPointDefault;
                }
            }
            CheckCurrentEquipmentWeight(0);
        }

        /// <summary>
        /// Abstract formula to link to the spell size by level script.
        /// </summary>
        /// <returns>Spell scale modifier.</returns>
        public float CalculateSpellScale()
        {
            var spellScaleModifier = eadonRpgCharacterConfig.Stats.Where(stat => stat.StatAffectsSpellScale).Sum(stat => stats[stat.StatName].value / eadonRpgDefaultValues.SpellPowerScaleDefault);
            spellScaleModifier += currentLevel * eadonRpgDefaultValues.SpellScaleIncreasePerLevel;
            var spellScale = 1f + spellScaleModifier;
            return spellScale > eadonRpgDefaultValues.SpellPowerMaxDefault ? eadonRpgDefaultValues.SpellPowerMaxDefault : spellScale;
        }

        /// <summary>
        /// Cache the skill point/resistance bonuses for faster calc.
        /// </summary>
        /// <param name="updateToMax">Update core stats to the MAX after recalculate.</param>
        protected virtual void RebuildModifierTotals(bool updateToMax)
        {
            // zero the skill cache totals
            foreach (var sv in statsModifierTotals.Values)
            {
                sv.value = 0;
            }

            foreach (var sv in skillsModifiersTotals.Values)
            {
                sv.value = 0;
            }

            foreach (var stat in eadonRpgCharacterConfig.GetStatNames())
            {
                // recalculate the stat totals
                foreach (var eadonBaseValue in modifiers.Where(t => t is EadonBaseStatSkillValue))
                {
                    var mod = (EadonBaseStatSkillValue) eadonBaseValue;
                    if (mod.stat == stat)
                    {
                        statsModifierTotals[stat].value += mod.value;
                    }
                }
            }

            foreach (var skill in eadonRpgCharacterConfig.GetSkillNames())
            {
                // recalculate the skill totals
                foreach (var eadonBaseValue in modifiers.Where(t => t is EadonBaseStatSkillValue))
                {
                    var mod = (EadonBaseStatSkillValue) eadonBaseValue;
                    if (mod.stat == skill)
                    {
                        skillsModifiersTotals[skill].value += mod.value;
                    }
                }
            }

            currentDamageBonus = damageBonus;
            foreach (var stat in eadonRpgCharacterConfig.Stats)
            {
                if (!stat.StatAffectsDamage) continue;
                var currentStat = GetTotalStatValue(stat.StatName);
                var currentStatBonus = eadonRpgDefaultValues.GetSkillBonusForStatValue(currentStat);
                currentDamageBonus += currentStatBonus;
            }

            foreach (var skill in eadonRpgCharacterConfig.Skills)
            {
                if (!skill.SkillAffectsDamage) continue;
                var currentSkill = skills[skill.SkillName].value;
                var currentStatBonus = 1;
                if (skill.RelatedStatName != null)
                {
                    var currentStat = GetTotalStatValue(skill.RelatedStatName); 
                    currentStatBonus = eadonRpgDefaultValues.GetSkillBonusForStatValue(currentStat);
                }
                currentDamageBonus += (int)(currentStatBonus * currentSkill);
            }

            currentDamageBonus += (int)(currentClass.ClassDamageBonus * currentLevel);
            currentDamageBonus += (int)(eadonRpgDefaultValues.DamagePerLevelDefault * currentLevel);
            
            charmResistance = baseCharmResistance;
            holdResistance = baseHoldResistance;
            
            CalculateMaxLife(updateToMax);
            CalculateResistanceLevelBonus();
            CalculateMaxEquipmentLoad();
            CalculateMaxStamina();
            CalculateMaxMana(updateToMax);
        }

        public float GetTotalStatValue(string stat)
        {
            return stats[stat].value + statsModifierTotals[stat].value + GetStatSkillBuffTotal(stat);
        }

        public float GetCharacterStatValue(string stat, bool includeBuffs)
        {
            var statValue = stats[stat].value;

            foreach (var modifier in modifiers)
            {
                if (!(modifier is EadonBaseStatSkillValue statModifier)) continue;
                if (statModifier.stat == stat)
                {
                    switch (modifier.src)
                    {
                        case EadonModifierSource.Character:
                            statValue += modifier.value;
                            break;
                        case EadonModifierSource.Buff:
                            if (includeBuffs)
                            {
                                statValue += modifier.value;
                            }
                            break;
                        case EadonModifierSource.Talent:
                            statValue += modifier.value;
                            break;
                        case EadonModifierSource.Armour:
                        case EadonModifierSource.Weapon:
                        case EadonModifierSource.MagicItem:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return statValue;
        }
        
        /// <summary>
        /// Find the specified skill (aka attribute) total (ie skill points+skill points from equipment).
        /// </summary>
        /// <param name="skill">Which skill to find.</param>
        /// <returns>Total skill points from character and equipment.</returns>
        public float GetTotalSkillValue(string skill)
        {
            return stats[skill].value + statsModifierTotals[skill].value;
        }

        public float GetCharacterSkillValue(string skill, bool includeBuffs)
        {
            var skillValue = skills[skill].value;

            foreach (var modifier in modifiers)
            {
                if (!(modifier is EadonBaseStatSkillValue statModifier)) continue;
                if (statModifier.stat == skill)
                {
                    switch (modifier.src)
                    {
                        case EadonModifierSource.Character:
                            skillValue += modifier.value;
                            break;
                        case EadonModifierSource.Buff:
                            if (includeBuffs)
                            {
                                skillValue += modifier.value;
                            }

                            break;
                        case EadonModifierSource.Talent:
                            skillValue += modifier.value;
                            break;
                        case EadonModifierSource.Armour:
                        case EadonModifierSource.Weapon:
                        case EadonModifierSource.MagicItem:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return skillValue;
        }

        /// <summary>
        /// Link to item equipped/unequipped to rebuild the equipment modifiers.
        /// </summary>
        /// <param name="source">Source item causing the stat change.</param>
        /// <param name="equipped">Whether the source item is being equipped.</param>
        public void RecalculateEquipmentBonuses(EadonCharacterEquipmentAttributes source, bool equipped)
        {
            // ensure per second operations are disabled
            if (isDamageEverySecondRunning)
            {
                CancelInvoke(nameof(EverySecondOperations));
                isDamageEverySecondRunning = false;
            }

            // remove all previous (non character) equipment bonuses
            modifiers.RemoveAll(s => s.src == EadonModifierSource.Armour || s.src == EadonModifierSource.Weapon || s.src == EadonModifierSource.MagicItem);

            // zero previous core totals
            baseMaxEquipmentLoadBonus = 0f;
            armourBonus = baseArmourBonus;
            maxLifeBonus = baseMaxLifeBonus;
            lifeRegenerationBonus = baseLifeRegenerationBonus;
            maxManaBonus = baseMaxManaBonus;
            manaRegenerationBonus = baseManaRegenerationBonus;
            maxStaminaBonus = baseMaxStaminaBonus;
            damageBonus = baseDamageBonus;
            charmResistance = baseCharmResistance;
            holdResistance = baseHoldResistance;
            
#if EADON_USE_SURVIVAL
            equipHungerResistance = 0;
            equipThirstResistance = 0;
            equipHeatResistance = 0;
            equipColdResistance = 0;
#endif

            // process children with equipment that have attributes
            var allEquipped = GetComponentsInChildren<EadonCharacterEquipmentAttributes>();  // find all magic equipment
            Debug.Log($"Found {allEquipped.Length} items");
            if (!equipped)
            {  // remove source item that has just been unequipped?
                allEquipped = allEquipped.Where(s => s != source).ToArray();  // remove the source item
            }

            // found some?
            foreach (var equip in allEquipped)
            {  // process all
                // recalculate core modifiers
                foreach (var cb in equip.core)
                {
                    switch (cb.attribute)
                    {
                        case EadonCharacterCoreAttribute.Weight:
                            baseMaxEquipmentLoadBonus += cb.value;
                            break;
                        case EadonCharacterCoreAttribute.Armour:
                            armourBonus += cb.value;
                            break;
                        case EadonCharacterCoreAttribute.Life:
                            maxLifeBonus += (int)cb.value;
                            break;
                        case EadonCharacterCoreAttribute.LifeRegeneration:
                            lifeRegenerationBonus += (int)cb.value;
                            break;
                        case EadonCharacterCoreAttribute.Mana:
                            maxManaBonus += (int)cb.value;
                            break;
                        case EadonCharacterCoreAttribute.ManaRegeneration:
                            manaRegenerationBonus += (int)cb.value;
                            break;
                        case EadonCharacterCoreAttribute.Stamina:
                            maxStaminaBonus += (int)cb.value;
                            break;
                        case EadonCharacterCoreAttribute.Damage:
                            damageBonus += (int)cb.value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // recalculate stat modifier stack
                foreach (var sb in equip.statPoints.Where(sb => !string.IsNullOrEmpty(sb.StatName)))
                {
                    modifiers.Add(new EadonBaseStatSkillValue() { src = equip.bonusSource, stat = sb.StatName, value = sb.value });
                }

                // recalculate skill modifier stack
                foreach (var sb in equip.skillPoints.Where(sb => !string.IsNullOrEmpty(sb.SkillName)))
                {
                    modifiers.Add(new EadonBaseStatSkillValue() { src = equip.bonusSource, stat = sb.SkillName, value = sb.value });
                }

                // recalculate resist modifier stack
                foreach (var md in equip.resistance)
                {
                    modifiers.Add(new EadonBaseResistPercent() { src = equip.bonusSource, resist = md.DamageTypeName, value = md.value });
                }

                charmResistance += equip.charmResistance;
                holdResistance += equip.holdResistance;

#if EADON_USE_SURVIVAL
                equipHungerResistance += equip.hungerResistance;
                equipThirstResistance += equip.thirstResistance;
                equipHeatResistance += equip.heatResistance;
                equipColdResistance += equip.coldResistance;
#endif
            }

            SetupRegeneration(lifeRegeneration + lifeRegenerationBonus);
            CalculateMaxLife(false);
            CalculateMaxStamina();
            CalculateMaxMana(false);

            HandleManaSlider();
            
            // re enable per second operations, if have DoTs or regen attributes
            if (Application.isPlaying && damageEverySecond.Count + lifeRegeneration + lifeRegenerationBonus + manaRegeneration + manaRegenerationBonus > 0)
            {
                isDamageEverySecondRunning = true;
                InvokeRepeating(nameof(EverySecondOperations), 1f, 1f);  // 1s delay, repeat every 1s
            }

            // recalculate the totals
            RebuildModifiers(true);
        }

        protected virtual void HandleManaSlider()
        {
            
        }
        
        protected virtual void SetupRegeneration(int regenerationAmount)
        {
            
        }
        
        /// <summary>
        /// Add skill modifiers onto the stack.
        /// </summary>
        /// <param name="statName">Which skill.</param>
        /// <param name="applyValue">Value to apply.</param>
        /// <param name="source"></param>
        public void AddModifier(string statName, int applyValue, EadonModifierSource source)
        {
            modifiers.Add(new EadonBaseStatSkillValue() { stat = statName, value = applyValue, src = source });
        }

        /// <summary>
        /// Add resistance modifiers onto the stack.
        /// </summary>
        /// <param name="resistanceName">Which skill.</param>
        /// <param name="value">Value to apply.</param>
        /// <param name="source"></param>
        public void AddResistanceModifier(string resistanceName, float value, EadonModifierSource source)
        {
            modifiers.Add(new EadonBaseResistPercent() { resist = resistanceName, value = value, src = source });
        }

        #endregion

        #region Buff Handling

        protected virtual void UpdateBuffs()
        {
            var deltaTime = Time.deltaTime;
            
            foreach (var temporaryStatSkillBuff in temporaryStatSkillBuffs)
            {
                temporaryStatSkillBuff.timer += deltaTime;
            }
            var removed = temporaryStatSkillBuffs.RemoveAll(tb => tb.timer >= tb.duration);

            foreach (var temporaryResistanceBuff in temporaryResistanceBuffs)
            {
                temporaryResistanceBuff.timer += deltaTime;
            }
            removed += temporaryResistanceBuffs.RemoveAll(tb => tb.timer >= tb.duration);

#if EADON_USE_SURVIVAL
            foreach (var tempSurvivalBuff in temporarySurvivalBuffs)
            {
                tempSurvivalBuff.timer += deltaTime;
            }
            removed += temporaryResistanceBuffs.RemoveAll(tb => tb.timer >= tb.duration);
#endif
            if (removed > 0)
            {
                RebuildModifiers(true);
            }
        }

        public float GetStatSkillBuffTotal(string statSkillName)
        {
            var total = 0f;

            if (temporaryStatSkillBuffs != null)
            {
                foreach (var temporaryStatSkillBuff in temporaryStatSkillBuffs)
                {
                    if (temporaryStatSkillBuff.buffValue.stat == statSkillName)
                    {
                        total += temporaryStatSkillBuff.buffValue.value;
                    }
                }
            }

            return total;
        }

        public float GetResistanceBuffTotal(string damageTypeName)
        {
            var total = 0f;

            if (temporaryResistanceBuffs != null)
            {
                foreach (var temporaryResistanceBuff in temporaryResistanceBuffs)
                {
                    if (temporaryResistanceBuff.buffValue.resist == damageTypeName)
                    {
                        total += temporaryResistanceBuff.buffValue.value;
                    }
                }
            }

            return total;
        }

#if EADON_USE_SURVIVAL
        public float GetSurvivalBuffTotal(BuffType type)
        {
            var total = 0f;
            
            if (temporarySurvivalBuffs != null)
            {
                foreach (var temporarySurvivalBuff in temporarySurvivalBuffs)
                {
                    if (temporarySurvivalBuff.buffType == type)
                    {
                        total += temporarySurvivalBuff.buffValue;
                    }
                }
            }

            return total;
        }
#endif

        public void AddStatSkillBuff(string statSkillName, float amount, float duration)
        {
            var buff = new EadonTempStatSkillBuff();
            buff.duration = duration;
            buff.timer = 0f;
            buff.buffValue = new EadonBaseStatSkillValue();
            buff.buffValue.stat = statSkillName;
            buff.buffValue.value = amount;
            buff.buffValue.src = EadonModifierSource.Buff;
            
            // {
            //     duration = duration,
            //     timer = 0f,
            //     buffValue = {stat = statSkillName, value = amount, src = EadonModifierSource.Buff}
            // };

            temporaryStatSkillBuffs.Add(buff);
            RebuildModifiers(true);
        }

        public void AddResistanceBuff(string damageTypeName, float amount, float duration)
        {
            var buff = new EadonTempResistBuff()
            {
                duration = duration,
                timer = 0f,
                buffValue = {resist = damageTypeName, value = amount, src = EadonModifierSource.Buff}
            };

            temporaryResistanceBuffs.Add(buff);
            RebuildModifiers(true);
        }

#if EADON_USE_SURVIVAL
        public void AddSurvivalBuff(BuffType type, float amount, float duration)
        {
            var buff = new EadonTempSurvivalBuff()
            {
                buffType = type,
                duration = duration,
                timer = 0f,
                buffValue = amount
            };

            temporarySurvivalBuffs.Add(buff);
            RebuildModifiers(true);
        }
#endif

        #endregion

        #region Inventory Management

        public void OnAddItem(vItem itemToAdd)
        {
            if (itemToAdd.type == vItemType.ClothingSet)
            {
                var clothingSetIdAttribute = itemToAdd.GetItemAttribute(vItemAttributes.ClothingID);
                if (clothingSetIdAttribute != null)
                {
                    var clothingSet = clothingManager.GetItemSetInstance(clothingSetIdAttribute.value);
                    if (clothingSet != null)
                    {
                        foreach (var clothingItem in clothingSet.allItems)
                        {
                            var clothingId = clothingItem.clothingId;
                            var items = characterItemManager.itemListData.items.FindAll(i => i.type == vItemType.Clothing);
                            foreach (var item in items)
                            {
                                if (item.GetItemAttribute(vItemAttributes.ClothingID).value == clothingId)
                                {
                                    var itemRef = new ItemReference(item.id) {amount = 1, autoEquip = false};
                                    characterItemManager.AddItem(itemRef);
                                }
                            }
                        }
                    }
                }
            }
            currentEquipmentLoad = CalculateCurrentEquipmentWeight();
            CheckCurrentEquipmentWeight(0);
            
            /*
            var itemWeightAttribute = item.attributes.Find(ai => ai.name.ToString() == "ItemWeight");
            if (itemWeightAttribute != null)
            {
                var itemWeight = itemWeightAttribute.value;

                CheckCurrentEquipmentWeight(itemWeight);
                if (currentEquipmentLoad + itemWeight > maxEquipmentLoad)
                {
                    if (enableWeightLimits)
                    {
                        EnableOverweightSpeeds();
                        EnableOverweightInput();
                    }
                }
                else
                {
                    if (enableWeightLimits)
                    {
                        ResetSpeeds();
                        ResetInput();
                    }
                }
            }
        */
        }

        public void OnDropItem(vItem item, int quantity)
        {
            currentEquipmentLoad = CalculateCurrentEquipmentWeight();

            if (item == null || item.attributes == null)
            {
                return;
            }
            var itemWeightAttribute = item.attributes.Find(ai => ai.name.ToString() == "ItemWeight");
            if (itemWeightAttribute != null)
            {
                var itemWeight = itemWeightAttribute.value;

                currentEquipmentLoad -= itemWeight * quantity;
                
                CheckCurrentEquipmentWeight(0);
                
                currentEquipmentLoad = CalculateCurrentEquipmentWeight();
            }
        }

        protected virtual void CheckCurrentEquipmentWeight(int itemWeight)
        {
        }

        public int CalculateCurrentEquipmentWeight()
        {
            var currentWeight = 0;

            if (characterItemManager == null)
            {
                characterItemManager = GetComponent<vItemManager>();
            }

            if (characterItemManager != null)
            {
                if (characterItemManager.items != null)
                {
                    foreach (var item in characterItemManager.items)
                    {
                        var itemWeightAttribute = item.attributes.Find(ai => ai.name.ToString() == "ItemWeight");
                        if (itemWeightAttribute != null)
                        {
                            currentWeight += itemWeightAttribute.value * item.amount;
                        }
                    }
                }
            }

            return currentWeight;
        }
        
        #endregion

        #region Magic

        public virtual void CastSpell(vItem spell)
        {
            if (spell.type != vItemType.Spell) return;
            if (spellCooldownTimer < spellCooldown)
            {
                return;
            }
            var magicId = spell.attributes.Find(ai => ai.name.ToString() == "MagicID").value;
            var manaCost = spell.attributes.Find(ai => ai.name.ToString() == "ManaCost").value;
            if (currentMana >= manaCost)
            {
                animator.SetInteger(MagicIdParameter, magicId);  // set the animator magic ID to select the spell
                animator.SetTrigger(MagicAttackTrigger);   // trigger the Magic State                                         
                UseMana(manaCost);
                spellCooldownTimer = 0;
            }
        }
        
        public void SpellEquipped(vItem spellItem)
        {
            var magicId = spellItem.attributes.Find(ai => ai.name.ToString() == "MagicID");  // grab the magic id
            if (magicId != null)  // fail safe
            {  
                if (debuggingMessages)
                {
                    Debug.Log("Equipped " + spellItem.name);
                }
            }
            else
            {
                if (debuggingMessages)
                {
                    Debug.Log(spellItem.name + " is missing required attribute MagicID, unable to equip the spell");
                }
            }
        }

        public void SpellUnequipped(vItem viSpell)
        {
            var magicId = viSpell.attributes.Find(ai => ai.name.ToString() == "MagicID");  // grab the magic id
            if (magicId != null)
            {  // fail safe
                if (debuggingMessages)
                {
                    Debug.Log("Unequipped " + viSpell.name);
                }
            }
            else
            {
                if (debuggingMessages)
                {
                    Debug.Log(viSpell.name + " is missing required attribute MagicID, unable to unequip the spell");
                }
            }
        }  

        public virtual void UsePotion(vItem viDrinkMe)
        {
            if (viDrinkMe.type != vItemType.Consumable) return;
            // ensure is a potion
            foreach (var viaAttrib in viDrinkMe.attributes)
            {  // check for valid attributes
                switch (viaAttrib.name.ToString())
                {  // naming is important
                    case "Mana":
                        AddMana(viaAttrib.value);
                        break;
                    case "MaxMana":
                        SetManaMax();
                        break;
                    case "MaxHealth":
                        var maxHealthMod = new InvectorBaseStatSkillValue
                        {
                            stat = InvectorStats.Health,
                            value = viaAttrib.value
                        };
                        permanentModifiers.Add(maxHealthMod);
                        CalculateMaxLife(true);
                        break;
                    case "MaxStamina":
                        var maxStaminaMod = new InvectorBaseStatSkillValue
                        {
                            stat = InvectorStats.Stamina,
                            value = viaAttrib.value
                        };
                        permanentModifiers.Add(maxStaminaMod);
                        CalculateMaxStamina();
                        break;
                }
            }
        }

        public void AddSpell(string spellName)
        {
            var spellItem = characterItemManager.itemListData.items.FirstOrDefault(item => item.name == spellName);

            if (spellItem == null) return;

            var itemCount = characterItemManager.items.Count(i => i.id == spellItem.id);

            if (itemCount == 0)
            {
                var itemReference = new ItemReference(spellItem.id) {amount = 1};

                characterItemManager.AddItem(itemReference);
            }
        }

        public void RemoveSpell(string spellName)
        {
            var spellItem = characterItemManager.itemListData.items.FirstOrDefault(item => item.name == spellName);

            if (spellItem == null) return;

            if (characterItemManager.items.Contains(spellItem))
            {
                characterItemManager.DestroyItem(spellItem, 1);
            }
        }

        public void AddItem(int itemId)
        {
            var vItem = characterItemManager.itemListData.items.FirstOrDefault(item => item.id == itemId);

            if (vItem == null) return;

            if (!characterItemManager.items.Contains(vItem))
            {
                var itemReference = new ItemReference(vItem.id) {amount = 1};

                characterItemManager.AddItem(itemReference);
            }
        }
        
        public void RemoveItem(int itemId)
        {
            var vItem = characterItemManager.itemListData.items.FirstOrDefault(item => item.id == itemId);

            if (vItem == null) return;

            if (characterItemManager.items.Contains(vItem))
            {
                characterItemManager.DestroyItem(vItem, 1);
            }
        }

        #endregion
        
        #region Damage

        /// <summary>
        /// Triggered when collider takes a hit on this player/NPC, linked to invector hit event.
        /// </summary>
        /// <param name="hit">Info about the hit.</param>
        public void OnSendHit(vHitInfo hit)
        {
            var additionalDamage = new vDamage {sender = transform, damageValue = currentDamageBonus, hitReaction = true};
            hit.targetCollider.gameObject.ApplyDamage(additionalDamage);

            if (!(hit.attackObject is vMeleeWeapon)) return;
            var mDamage = hit.attackObject.GetComponent<EadonMagicObjectDamage>();  // attempt grab magic damage
            var targetCharacter = hit.targetCollider.GetComponent<EadonRpgCharacterBase>();
            if (targetCharacter != null) targetCharacter.OnReceiveMagicalDamage(mDamage, 0);
        }

        /// <summary>
        /// Triggered when invector vObjectDamage strikes this player/NPC, linked to invector damage event.
        /// </summary>
        /// <param name="damage">Info about the damage received.</param>
        public void OnReceiveDamage(vDamage damage)
        {
            EadonMagicObjectDamage mDamage = null;
            if (damage.sender != null)
            {
                mDamage = damage.sender.GetComponent<EadonMagicObjectDamage>();
            }
            OnReceiveMagicalDamage(mDamage, damage.damageValue);
        }  

        /// <summary>
        /// Damage mitigation by elemental type.
        /// </summary>
        /// <param name="magicDamage">Magic damage data.</param>
        /// <param name="damage">Physical damage amount.</param>
        public virtual void OnReceiveMagicalDamage(EadonMagicObjectDamage magicDamage, float damage)
        {
        }

        /// <summary>
        /// Abstract formula for damage mitigation.
        /// </summary>
        /// <param name="type">Type of elemental damage.</param>
        /// <param name="amount">Amount of damage.</param>
        /// <returns></returns>
        protected int MitigateDamage(string type, float amount)
        {
            if (type == "Physical")
            {  // physical damage?
                if (currentArmour + armourBonus > 0)
                {  // has armour?
                    amount -= eadonRpgDefaultValues.ArmourReductionDefault * (currentArmour + armourBonus);  // apply armour mitigation
                }
            }

            var resistanceModifier = resistancesModifiersTotals.ContainsKey(type)
                ? resistancesModifiersTotals[type].value
                : 0;
            if (resistances[type].value + resistanceModifier > 0)
            {  // has resistance to this damage type?
                amount -= amount / 100 * (resistances[type].value + resistanceModifier + GetResistanceBuffTotal(type));  // apply resistance mitigation
            }

            return Mathf.CeilToInt(amount);
        }
        
        #endregion

        #region Damage Over Time

        /// <summary>
        /// Repeating coroutine applying DoT's every 2 seconds.
        /// </summary>
        public virtual void EveryTwoSecondsOperations()
        {
        }

        /// <summary>
        /// Repeating coroutine applying DoT's and life/mana regen every second.
        /// </summary>
        public virtual void EverySecondOperations()
        {
        }

        /// <summary>
        /// Repeating coroutine applying DoT's every half second.
        /// </summary>
        public virtual void EveryHalfSecondOperations()
        {
        }

        /// <summary>
        /// Repeating coroutine applying DoT's every quarter second.
        /// </summary>
        public virtual void EveryQuarterSecondOperations()
        {
        }

        /// <summary>
        /// Apply DoT's, called from per second operations repeating coroutines.
        /// </summary>
        /// <param name="whichDoT">Type of damage over time (and frequency) to apply via the mitigation function.</param>
        /// <returns></returns>
        protected virtual bool ApplyDamageOverTime(ref List<EadonMagicDamageOverTime> whichDoT)
        {
            return false;
        }

        /// <summary>
        /// Enable condition particle effects (eg burning, bleeding) on the parent character.
        /// </summary>
        /// <returns>Whether any conditions are still active.</returns>
        protected bool ApplyConditions()
        {
            if (conditions == null) return false;
            var stillActive = false;
            foreach (var bc in conditions.Values.Where(bc => bc.display))
            {
                if (bc.length >= 1)  // found an active condition
                {
                    stillActive = true;  // keep per second operations running
                    bc.length -= 1;  // subtract a second for next run
                    bc.display.SetActive(true);  // ensure effect is enabled
                }
                else if (bc.length < 1)  // condition has expired
                {
                    bc.length = 0;  // reset length
                    bc.display.SetActive(false);  // ensure effect is disabled
                }
            }
            return stillActive;  // all done
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Finds child transform by name even if inactive.
        /// </summary>
        /// <param name="parent">Transform to search.</param>
        /// <param name="childName">Name of transform to find.</param>
        /// <returns>Game object searched for or null if not found.</returns>
        protected virtual GameObject FindInActiveChild(Transform parent, string childName)
        {
            if (!parent) return null;
            var trs = parent.GetComponentsInChildren<Transform>(true);
            return (from t in trs where t.name == childName select t.gameObject).FirstOrDefault();
        }

        #endregion

        #region AI Support

        /// <summary>
        /// Is this a ranged unit type?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAttackAtRange()
        {
            return false;
        }

        /// <summary>
        /// Is this a magic unit type?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanCastSpells()
        {
            return false;
        }

        #endregion

        #region Eadon Survival

#if EADON_USE_SURVIVAL
        public float GetHungerProtection()
        {
            return currentHungerResistance + equipHungerResistance + GetSurvivalBuffTotal(BuffType.HungerResistance);
        }

        public float GetThirstProtection()
        {
            return currentThirstResistance + equipThirstResistance + GetSurvivalBuffTotal(BuffType.ThirstResistance);
        }

        public float GetHeatProtection()
        {
            return currentHeatResistance + equipHeatResistance + GetSurvivalBuffTotal(BuffType.HeatResistance);
        }

        public float GetColdProtection()
        {
            return currentColdResistance + equipColdResistance + GetSurvivalBuffTotal(BuffType.ColdResistance);
        }
#endif
        
        #endregion
    }

    #endregion

    #region "Modifier Stack & Other Lists"



    /// <summary>
    /// Condition reaction to elemental damage.
    /// </summary>
    [Serializable]
    public class BaseCondition
    {
        /// <summary>Condition damage type.</summary>
        public string type;

        /// <summary>Child game object to enable when the condition is active.</summary>
        public GameObject display;

        /// <summary>Remaining seconds to keep the condition active.</summary>
        public float length;
    }

    /// <summary>
    /// Prefabs vs value the prefab represents
    /// </summary>
    [Serializable]
    public class CollectablePrefab
    {
        /// <summary>Game object to spawn when dropping this amount of the collectable.</summary>
        public GameObject prefab;

        /// <summary>Amount of collectables this prefab represents.</summary>
        public float amount;  
    }

    /// <summary>
    /// Core attribute list for GUI.
    /// </summary>
    public enum EadonCharacterCoreAttribute
    {
        Weight, Armour, Life, LifeRegeneration, Mana, ManaRegeneration, Stamina, Damage
    }

    /// <summary>
    /// Core bonus attributes for equipment to increase.
    /// </summary>
    [Serializable]
    public class EadonCoreBonus
    {
        /// <summary>Type of core attribute to increase whilst wearing/holding this item.</summary>
        [Tooltip("Type of core attribute to increase whilst wearing/holding this item")]
        public EadonCharacterCoreAttribute attribute;

        /// <summary>Amount of core attribute of the specified type to apply.</summary>
        [Tooltip("Amount of core attribute of the specified type to apply")]
        public float value;
    }

    /// <summary>
    /// Magic damage to apply when a weapon/spell strikes.
    /// </summary>
    [Serializable]
    public class EadonMagicDamage
    {
        /// <summary>Type of magic damage, note that the normal vObjectDamage is effectively applying physical and is required.</summary>
        [Tooltip("Type of magic damage, note that the normal vObjectDamage is effectively applying physical and is required")]
        public EadonRpgDamageType damageType;

        public string DamageTypeName => damageType != null ? damageType.DamageTypeName : "";

        /// <summary>Amount of magic damage/resist of the specified type to apply.</summary>
        [Tooltip("Amount of magic damage/resist of the specified type to apply")]
        public float value;
    }

    /// <summary>
    /// Update frequency choice.
    /// </summary>
    public enum EadonUpdateFrequency
    {
        QuarterSecond, HalfSecond, WholeSecond, TwoSeconds
    }

    /// <summary>
    /// Damage over time.
    /// </summary>
    [Serializable]
    public class EadonMagicDamageOverTime : EadonMagicDamage
    {
        /// <summary>Number of seconds to apply this damage, per second, enable by setting to greater than zero.</summary>
        [Tooltip("Number of seconds to apply this damage, per second, enable by setting to greater than zero")]
        public float dotLength;

        /// <summary>Amount of magic damage of the specified type to apply per tick of the frequency, if zero then the main damage value will be applied instead.</summary>
        [Tooltip("Amount of magic damage of the specified type to apply per tick of the frequency, if zero then the main damage value will be applied instead")]
        public float dotValue;

        /// <summary>Update frequency of the magic damage over time, eg if length = 2 secs, damage value = 2 and frequency = Quarter Second then 16 total damage will be applied.</summary>
        [Tooltip("Update frequency of the magic damage over time, eg if length = 2 secs, damage value = 2 and frequency = Quarter Second then 16 total damage will be applied")]
        public EadonUpdateFrequency dotFrequency;
    }


    /// <summary>
    /// Skill bonus for equipment.
    /// </summary>
    [Serializable]
    public class EadonStatBonus
    {
        /// <summary>Type of skill to increase whilst wearing/holding this item.</summary>
        [Tooltip("Type of stat to increase whilst wearing/holding this item")]
        public EadonRpgStat stat;
        public string StatName => stat != null ? stat.StatName : "";

        /// <summary>Amount of skill attribute of the specified type to apply.</summary>
        [Tooltip("Amount of stat attribute of the specified type to apply")]
        public float value;
    }

    /// <summary>
    /// Skill bonus for equipment.
    /// </summary>
    [Serializable]
    public class EadonSkillBonus
    {
        /// <summary>Type of skill to increase whilst wearing/holding this item.</summary>
        [Tooltip("Type of skill to increase whilst wearing/holding this item")]
        public EadonRpgSkill skill;
        public string SkillName => skill != null ? skill.SkillName : "";

        /// <summary>Amount of skill attribute of the specified type to apply.</summary>
        [Tooltip("Amount of skill attribute of the specified type to apply")]
        public float value;
    }

    #endregion
}
#endif
