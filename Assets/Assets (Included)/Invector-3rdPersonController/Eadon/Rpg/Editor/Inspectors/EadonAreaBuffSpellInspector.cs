#if EADON_RPG_INVECTOR
using System;
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Magic;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor.Inspectors
{
    [CustomEditor(typeof(EadonAreaBuffSpell))]
    public class EadonAreaBuffSpellInspector : EadonBaseEditor
    {
        private SerializedObject _serializedBuffSpell;
        
        private void OnEnable()
        {
            editorTitle = "Eadon Area Buff Spell";
            splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_area_buff_spell", typeof(Texture2D));
            showExpandButton = false;
            _serializedBuffSpell = serializedObject;
        }

        protected override void OnBaseInspectorGUI()
        {
            serializedObject.Update();

            var buffType = _serializedBuffSpell.FindProperty("buffType");
            var statToBuff = _serializedBuffSpell.FindProperty("statToBuff");
            var skillToBuff = _serializedBuffSpell.FindProperty("skillToBuff");
            var resistanceToBuff = _serializedBuffSpell.FindProperty("currentAlignment");
            var buffAmount = _serializedBuffSpell.FindProperty("buffAmount");
            var buffDuration = _serializedBuffSpell.FindProperty("buffDuration");
            var useSpellScale = _serializedBuffSpell.FindProperty("useSpellScale");
            var destroyTime = _serializedBuffSpell.FindProperty("destroyTime");
            var radius = _serializedBuffSpell.FindProperty("radius");
            var targetLayers = _serializedBuffSpell.FindProperty("targetLayers");
            var targetTags = _serializedBuffSpell.FindProperty("targetTags");
            
            EditorGUILayout.PropertyField(buffType, new GUIContent("Buff Type"), false);
            BuffType type = (BuffType) buffType.enumValueIndex;
            switch (type)
            {
                case BuffType.Stat:
                    EditorGUILayout.PropertyField(statToBuff, new GUIContent("Stat To Buff"), false);
                    break;
                case BuffType.Skill:
                    EditorGUILayout.PropertyField(skillToBuff, new GUIContent("Skill To Buff"), false);
                    break;
                case BuffType.Resistance:
                    EditorGUILayout.PropertyField(resistanceToBuff, new GUIContent("Resistance To Buff"), false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            EditorGUILayout.PropertyField(buffAmount, new GUIContent("Buff Amount"), false);
            EditorGUILayout.PropertyField(buffDuration, new GUIContent("Buff Duration"), false);
            EditorGUILayout.PropertyField(useSpellScale, new GUIContent("Use Spell Scale"), false);
            EditorGUILayout.PropertyField(destroyTime, new GUIContent("Destroy Time"), false);
            EditorGUILayout.PropertyField(radius, new GUIContent("Radius"), false);
            EditorGUILayout.PropertyField(targetLayers, new GUIContent("Target Layers"), false);
            EditorGUILayout.PropertyField(targetTags, new GUIContent("Target Tags"), false);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
