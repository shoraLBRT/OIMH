#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor.Inspectors
{
    [CustomEditor(typeof(EadonRpgTalent))]
    public class EadonRpgTalentInspector : EadonBaseEditor
    {
        private int _selectedTab;
        private SerializedObject _serializedController;
        private EadonRpgTalent _talent;
        private readonly List<bool> _toggled = new List<bool>(); // Folded or not
        private string[] _itemNamesList;
        private FontStyle _origFontStyle;
        
        private void OnEnable()
        {
            editorTitle = "Eadon RPG Talent";
            splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_talent", typeof(Texture2D));
            showExpandButton = false;
            _talent = (EadonRpgTalent) target;
            _serializedController = serializedObject;
        }

        protected override void OnBaseInspectorGUI()
        {
            // update serialized object
            serializedObject.Update();

            var talentName = _serializedController.FindProperty("talentName");
            var talentDescription = _serializedController.FindProperty("talentDescription");
            var talentIcon = _serializedController.FindProperty("talentIcon");
            var talentType = _serializedController.FindProperty("talentType");
            var talentStat = _serializedController.FindProperty("talentStat");
            var talentSkill = _serializedController.FindProperty("talentSkill");
            var intValue = _serializedController.FindProperty("intValue");
            var talentResistance = _serializedController.FindProperty("talentResistance");
            var floatValue = _serializedController.FindProperty("floatValue");
            var spellLikeAbility = _serializedController.FindProperty("spellLikeAbility");
            var prerequisites = _serializedController.FindProperty("prerequisites");
            var items = _serializedController.FindProperty("items");
            
            EditorGUILayout.PropertyField(talentName, new GUIContent("Talent Name"), false);
            EditorGUILayout.PropertyField(talentDescription, new GUIContent("Talent Description"), false);
            EditorGUILayout.PropertyField(talentIcon, new GUIContent("Icon"), false);
            EditorGUILayout.PropertyField(talentType, new GUIContent("Talent Type"), false);
            switch (talentType.enumValueIndex)
            {
                case 0:
                    EditorGUILayout.PropertyField(talentStat, new GUIContent("Stat"), false);
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Bonus"), false);
                    break;
                case 1:
                    EditorGUILayout.PropertyField(talentSkill, new GUIContent("Skill"), false);
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Bonus"), false);
                    break;
                case 2:
                    EditorGUILayout.PropertyField(talentResistance, new GUIContent("Resistance"), false);
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Bonus"), false);
                    break;
                case 3:
                    EditorGUILayout.PropertyField(spellLikeAbility, new GUIContent("Spell Like Ability"), false);
                    break;
                case 4:
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Extra Damage"), false);
                    break;
                case 5:
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Extra Armour"), false);
                    break;
                case 6:
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Extra Life"), false);
                    break;
                case 7:
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Extra Life Reg."), false);
                    break;
                case 8:
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Extra Mana"), false);
                    break;
                case 9:
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Extra Mana Reg."), false);
                    break;
                case 10:
                    EditorGUILayout.PropertyField(intValue, new GUIContent("Extra Stamina"), false);
                    break;
                case 11:
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Extra Weight Cap."), false);
                    break;
                case 12:
                    EditorGUILayout.PropertyField(items, new GUIContent("Items"), true);
                    break;
#if EADON_USE_SURVIVAL
                case 13:
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Hunger Resistance"), false);
                    break;
                case 14:
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Thirst Resistance"), false);
                    break;
                case 15:
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Heat Resistance"), false);
                    break;
                case 16:
                    EditorGUILayout.PropertyField(floatValue, new GUIContent("Cold Resistance"), false);
                    break;
#endif
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(prerequisites, new GUIContent("Prerequisites"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
