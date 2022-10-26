#if EADON_RPG_INVECTOR
#if INVECTOR_AI_TEMPLATE
using System.Collections.Generic;
using System.Globalization;
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor.Inspectors
{
    [CustomEditor(typeof(EadonRpgNpc))]
    public class EadonRpgNpcInspector : EadonBaseEditor
    {
        private int _selectedTab;
//        private SerializedObject serializedObject;
        private EadonRpgNpc _character;
        private readonly List<bool> _toggled = new List<bool>(); // Folded or not
        private string[] _itemNamesList;
        private FontStyle _origFontStyle;
        
        private void OnEnable()
        {
            editorTitle = "Eadon RPG NPC";
            splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_npc", typeof(Texture2D));
            showExpandButton = false;
            _character = (EadonRpgNpc) target;
//            serializedObject = serializedObject;
        }

        protected override void OnBaseInspectorGUI()
        {
            string[] tabs = {"General", "Core", "Stats", "Skills", "Resistances"};
            _selectedTab = GUILayout.SelectionGrid(_selectedTab, tabs, 3, GUILayout.Width(300));

            _origFontStyle = EditorStyles.label.fontStyle;

            EditorGUILayout.Space();

            // update serialized object
            serializedObject.Update();

            switch (_selectedTab)
            {
                case 0:
                    DrawGeneralTab();
                    break;
                case 1:
                    DrawCoreTab();
                    break;
                case 2:
                    DrawStatsTab();
                    break;
                case 3:
                    DrawSkillsTab();
                    break;
                case 4:
                    DrawResistancesTab();
                    break;
            }

            // apply modified properties
            var changed = serializedObject.ApplyModifiedProperties();
            if (changed)
            {
                Debug.Log("CHANGED");
            }
        }

        private void DrawResistancesTab()
        {
            foreach (var damageType in _character.eadonRpgCharacterConfig.GetDamageTypeNames())
            {
                var resistVal = _character.resistances[damageType];
                var resistMod = _character.resistancesModifiersTotals[damageType];
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(damageType);
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField($"{resistVal.value + resistMod.value} ({resistVal.value} {resistMod.value.ToString(CultureInfo.InvariantCulture)})");
                EditorGUILayout.EndHorizontal();

            }
        }

        private void DrawSkillsTab()
        {
            var unspentSkillPoints = _character.unspentSkillPoints;

            unspentSkillPoints = EditorGUILayout.IntField("Unspent Skill Points", unspentSkillPoints);

            _character.unspentSkillPoints = unspentSkillPoints;
            
            EditorGUILayout.Space();
            
            var minValue = 0;

            foreach (var skill in _character.eadonRpgCharacterConfig.GetSkillNames())
            {
                if (_character.skills.ContainsKey(skill))
                {
                    var skillVal = _character.skills[skill];
                    var skillMod = _character.skillsModifiersTotals[skill];

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(skill);
                    GUILayout.FlexibleSpace();
                    if (skillVal.value >= 5)
                    {
                        if (GUILayout.Button("-5", GUILayout.Width(30)))
                        {
                            ChangeSkill(skill, -5, minValue);
                        }
                    }
                    if (skillVal.value >= 1)
                    {
                        if (GUILayout.Button("-1", GUILayout.Width(30)))
                        {
                            ChangeSkill(skill, -1, minValue);
                        }
                    }

                    if (unspentSkillPoints >= 1)
                    {
                        if (GUILayout.Button("+1", GUILayout.Width(30)))
                        {
                            ChangeSkill(skill, 1, minValue);
                        }
                    }
                    if (unspentSkillPoints >= 5)
                    {
                        if (GUILayout.Button("+5", GUILayout.Width(30)))
                        {
                            ChangeSkill(skill, 5, minValue);
                        }
                    }
                    EditorGUILayout.LabelField($"{skillVal.value + skillMod.value} ({skillVal.value} {skillMod.value.ToString(CultureInfo.InvariantCulture)})");
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void DrawStatsTab()
        {
            var unspentStatPoints = _character.unspentStatPoints;

            unspentStatPoints = EditorGUILayout.IntField("Unspent Stat Points", unspentStatPoints);

            _character.unspentStatPoints = unspentStatPoints;
            
            EditorGUILayout.Space();
            
            foreach (var stat in _character.eadonRpgCharacterConfig.GetStatNames())
            {
                var statVal = _character.stats[stat];
                var statMod = _character.statsModifierTotals[stat];
                var minValue = _character.eadonRpgDefaultValues.StatMinValueDefault;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(stat);
                GUILayout.FlexibleSpace();
                if (statVal.value >= 5)
                {
                    if (GUILayout.Button("-5", GUILayout.Width(30)))
                    {
                        ChangeStat(stat, -5, minValue);
                    }
                }
                if (statVal.value >= 1)
                {
                    if (GUILayout.Button("-1", GUILayout.Width(30)))
                    {
                        ChangeStat(stat, -1, minValue);
                    }
                }

                if (unspentStatPoints >= 1)
                {
                    if (GUILayout.Button("+1", GUILayout.Width(30)))
                    {
                        ChangeStat(stat, 1, minValue);
                    }
                }
                if (unspentStatPoints >= 5)
                {
                    if (GUILayout.Button("+5", GUILayout.Width(30)))
                    {
                        ChangeStat(stat, 5, minValue);
                    }
                }
                EditorGUILayout.LabelField($"{statVal.value + statMod.value} ({statVal.value} {statMod.value.ToString(CultureInfo.InvariantCulture)})");
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCoreTab()
        {
            var currentLife = _character.currentLife;
            var maxLife = _character.maxLife;
            var regenLife = _character.lifeRegeneration;
            var currentMana = _character.currentMana;
            var maxMana = _character.maxMana;
            var regenMana = _character.manaRegeneration;
            var maxStamina = _character.maxStamina;
            var currentArmour = _character.currentArmour;
            var bonusArmour = _character.armourBonus;
            var currentBonusDamage = _character.currentDamageBonus;
            var currentEquipLoad = _character.currentEquipmentLoad;
            var maxEquipLoad = _character.maxEquipmentLoad;
            var currentXp = _character.currentXp;
            var xpToNextLevel = _character.xpToNextLevel;
            var currentLevel = _character.currentLevel;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Life");
            GUILayout.FlexibleSpace();
            var lifeLabel = $"{currentLife}/{maxLife}";
            EditorGUILayout.LabelField(lifeLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Life Regen");
            GUILayout.FlexibleSpace();
            if (regenLife >= 5)
            {
                if (GUILayout.Button("-5", GUILayout.Width(30)))
                {
                    _character.lifeRegeneration -= 5;
                }
            }
            if (regenLife >= 1)
            {
                if (GUILayout.Button("-1", GUILayout.Width(30)))
                {
                    _character.lifeRegeneration -= 1;
                }
            }
            if (GUILayout.Button("+1", GUILayout.Width(30)))
            {
                _character.lifeRegeneration += 1;
            }
            if (GUILayout.Button("+5", GUILayout.Width(30)))
            {
                _character.lifeRegeneration += 5;
            }
            EditorGUILayout.LabelField($"{regenLife} per sec");
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mana");
            GUILayout.FlexibleSpace();
            var manaLabel = $"{currentMana}/{maxMana}";
            EditorGUILayout.LabelField(manaLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mana Regen");
            GUILayout.FlexibleSpace();
            if (regenMana >= 5)
            {
                if (GUILayout.Button("-5", GUILayout.Width(30)))
                {
                    _character.manaRegeneration -= 5;
                }
            }
            if (regenMana >= 1)
            {
                if (GUILayout.Button("-1", GUILayout.Width(30)))
                {
                    _character.manaRegeneration -= 1;
                }
            }
            if (GUILayout.Button("+1", GUILayout.Width(30)))
            {
                _character.manaRegeneration += 1;
            }
            if (GUILayout.Button("+5", GUILayout.Width(30)))
            {
                _character.manaRegeneration += 5;
            }
            EditorGUILayout.LabelField($"{regenMana} per sec");
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stamina");
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"{maxStamina}");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Armour");
            GUILayout.FlexibleSpace();
            var armourLabel = $"Base {currentArmour} Equipped {bonusArmour}";
            EditorGUILayout.LabelField(armourLabel);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Current Damage Bonus");
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"{currentBonusDamage}");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Equipment Load");
            GUILayout.FlexibleSpace();
            var eqLabel = $"{currentEquipLoad}/{maxEquipLoad}";
            EditorGUILayout.LabelField(eqLabel);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("XP");
            GUILayout.FlexibleSpace();
            var xpLabel = $"{currentXp}/{xpToNextLevel}";
            EditorGUILayout.LabelField(xpLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level");
            GUILayout.FlexibleSpace();
            if (_character.currentLevel > 5)
            {
                if (GUILayout.Button("-5", GUILayout.Width(30)))
                {
                    _character.currentLevel -= 5;
                    RecalculateLevel();
                }
            }
            if (_character.currentLevel > 1)
            {
                if (GUILayout.Button("-1", GUILayout.Width(30)))
                {
                    _character.currentLevel -= 1;
                    RecalculateLevel();
                }
            }

            if (_character.currentLevel < _character.eadonRpgDefaultValues.MaxLevelDefault)
            {
                if (GUILayout.Button("+1", GUILayout.Width(30)))
                {
                    _character.currentLevel += 1;
                    RecalculateLevel();
                }
            }

            if (_character.currentLevel < _character.eadonRpgDefaultValues.MaxLevelDefault - 5)
            {
                if (GUILayout.Button("+5", GUILayout.Width(30)))
                {
                    _character.currentLevel += 5;
                    RecalculateLevel();
                }
            }
            EditorGUILayout.LabelField($"{currentLevel}");
            EditorGUILayout.EndHorizontal();
        }

        private void RecalculateLevel()
        {
            _character.currentXp = _character.CalculateXpToNextLevel(_character.currentLevel - 1);
            _character.xpToNextLevel = _character.CalculateXpToNextLevel(_character.currentLevel);
            _character.unspentStatPoints =
                _character.currentLevel * _character.eadonRpgDefaultValues.LevelUpStatPointsDefault -
                _character.spentStatPoints;
            _character.unspentSkillPoints =
                _character.currentLevel * _character.eadonRpgDefaultValues.LevelUpSkillPointsDefault -
                _character.spentSkillPoints;
        }

        private void DrawGeneralTab()
        {
            // var eadonRpgDefaultValues = serializedObject.FindProperty("eadonRpgDefaultValues");
            // var eadonRpgCharacterConfig = serializedObject.FindProperty("eadonRpgCharacterConfig");
            // var currentRace = serializedObject.FindProperty("currentRace");
            // var currentClass = serializedObject.FindProperty("currentClass");
            // var currentAlignment = serializedObject.FindProperty("currentAlignment");
            // var characterName = serializedObject.FindProperty("characterName");

            var eadonRpgDefaultValues = _character.eadonRpgDefaultValues;
            var eadonRpgCharacterConfig = _character.eadonRpgCharacterConfig;
            var currentRace = _character.currentRace;
            var currentClass = _character.currentClass;
            var currentAlignment = _character.currentAlignment;
            var characterName = serializedObject.FindProperty("characterName");
            var spellCooldown = serializedObject.FindProperty("spellCooldown");
            
            // EditorGUILayout.PropertyField(eadonRpgDefaultValues, new GUIContent("Default Values"), false);
            // EditorGUILayout.PropertyField(eadonRpgCharacterConfig, new GUIContent("Character Config"), false);
            // EditorGUILayout.PropertyField(currentRace, new GUIContent("Race"), false);
            // EditorGUILayout.PropertyField(currentClass, new GUIContent("Class"), false);
            // EditorGUILayout.PropertyField(currentAlignment, new GUIContent("Alignment"), false);
            
            eadonRpgDefaultValues = (EadonRpgDefaultValues) EditorGUILayout.ObjectField("Default Values", eadonRpgDefaultValues, typeof(EadonRpgDefaultValues), false);
            eadonRpgCharacterConfig = (EadonRpgCharacterConfig) EditorGUILayout.ObjectField("Character Config", eadonRpgCharacterConfig, typeof(EadonRpgCharacterConfig), false);
            currentRace = (EadonRpgRace) EditorGUILayout.ObjectField("Race", currentRace, typeof(EadonRpgRace), false);
            currentClass = (EadonRpgClass) EditorGUILayout.ObjectField("Class", currentClass, typeof(EadonRpgClass), false);
            currentAlignment = (EadonRpgAlignment) EditorGUILayout.ObjectField("Alignment", currentAlignment, typeof(EadonRpgAlignment), false);

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(characterName, new GUIContent("Character Name"), false);

            var defaultValues = _character.eadonRpgDefaultValues;
            if (defaultValues != eadonRpgDefaultValues)
            {
                _character.eadonRpgDefaultValues = eadonRpgDefaultValues;
                _character.ResetCharacter();
            }
            
            var config = _character.eadonRpgCharacterConfig;
            if (config != eadonRpgCharacterConfig)
            {
                _character.eadonRpgCharacterConfig = eadonRpgCharacterConfig;
                _character.ResetCharacter();
            }
            
            var race = _character.currentRace;
            if (race != currentRace)
            {
                _character.currentRace = currentRace;
                _character.ResetCharacter();
            }
            
            var cClass = _character.currentClass;
            if (cClass != currentClass)
            {
                _character.currentClass = currentClass;
                _character.ResetCharacter();
            }
            
            var alignment = _character.currentAlignment;
            if (alignment != currentAlignment)
            {
                _character.currentAlignment = currentAlignment;
                _character.ResetCharacter();
            }

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(spellCooldown, new GUIContent("Spell Cooldown"), false);
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Restore To Defaults"))
            {
                _character.conditionsRoot = _character.transform.Find("Conditions");
                _character.ResetCharacter();
            }
        }

        private void ChangeStat(string stat, int amount, int minValue)
        {
            Debug.Log($"Changing {stat} by {amount}");
            _character.stats[stat].value += amount;
            _character.unspentStatPoints -= amount;
            _character.spentStatPoints += amount;
            _character.RebuildModifiers(true);
        }

        private void ChangeSkill(string skill, int amount, int minValue)
        {
            Debug.Log($"Changing {skill} by {amount}");
            _character.skills[skill].value += amount;
            _character.unspentSkillPoints -= amount;
            _character.spentSkillPoints += amount;
            _character.RebuildModifiers(true);
        }

        private void ChangeResistance(string stat, int amount, int minValue)
        {
            Debug.Log($"Changing {stat} by {amount}");
            _character.resistances[stat].value += amount;
            _character.unspentStatPoints -= amount;
        }
    }
}
#endif
#endif
