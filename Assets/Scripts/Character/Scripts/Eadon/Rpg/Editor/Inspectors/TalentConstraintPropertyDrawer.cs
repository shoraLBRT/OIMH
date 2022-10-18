#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Configuration;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor.Inspectors
{
    [CustomPropertyDrawer(typeof(EadonRpgTalentPrerequisite))]
    public class TalentConstraintPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            /*position = */EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 2;

            // Calculate rects
            var prerequisiteTypeRect = new Rect(position.x, position.y + 16 + (int)EditorGUIUtility.standardVerticalSpacing, 300, 16);
            var fieldOneRect = new Rect(position.x, position.y + 32 + 2 * (int)EditorGUIUtility.standardVerticalSpacing, 300, 16);
            var fieldTwoRect = new Rect(position.x, position.y + 48 + 3 * (int)EditorGUIUtility.standardVerticalSpacing, 300, 16);

            var prerequisiteType = property.FindPropertyRelative("prerequisiteType");
            var prerequisiteRace = property.FindPropertyRelative("prerequisiteRace");
            var prerequisiteClass = property.FindPropertyRelative("prerequisiteClass");
            var prerequisiteAlignment = property.FindPropertyRelative("prerequisiteAlignment");
            var prerequisiteTalent = property.FindPropertyRelative("prerequisiteTalent");
            var prerequisiteSkill = property.FindPropertyRelative("prerequisiteSkill");
            var prerequisiteStat = property.FindPropertyRelative("prerequisiteStat");
            var prerequisiteMinValue = property.FindPropertyRelative("prerequisiteMinValue");

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(prerequisiteTypeRect, prerequisiteType, new GUIContent("Type"));
            switch (prerequisiteType.enumValueIndex)
            {
                case 0:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteRace, new GUIContent("Race"));
                    break;
                case 1:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteRace, new GUIContent("Race"));
                    break;
                case 2:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteClass, new GUIContent("Class"));
                    break;
                case 3:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteClass, new GUIContent("Class"));
                    break;
                case 4:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteAlignment, new GUIContent("Alignment"));
                    break;
                case 5:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteAlignment, new GUIContent("Alignment"));
                    break;
                case 6:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteTalent, new GUIContent("Talent"));
                    break;
                case 7:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteTalent, new GUIContent("Talent"));
                    break;
                case 8:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteSkill, new GUIContent("Skill"));
                    EditorGUI.PropertyField(fieldTwoRect, prerequisiteMinValue, new GUIContent("Value"));
                    break;
                case 9:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteStat, new GUIContent("Stat"));
                    EditorGUI.PropertyField(fieldTwoRect, prerequisiteMinValue, new GUIContent("Value"));
                    break;
                case 10:
                    EditorGUI.PropertyField(fieldOneRect, prerequisiteMinValue, new GUIContent("Level"));
                    break;
            }
            
            // EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("unit"), GUIContent.none);
            // EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4 * (16 + (int)EditorGUIUtility.standardVerticalSpacing);
        }
    }
}
#endif
