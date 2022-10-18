﻿#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.ClothingSystem;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Character.Editor
{
    [CustomEditor(typeof(EadonCharacterEquipmentAttributes))]
    public class EadonCharacterEquipmentAttributesInspector : EadonBaseEditor
    {
        private static readonly string[] ExcludedProperties = new string[] { "m_Script" };

        private void OnEnable()
        {
            editorTitle = "Eadon Character Equipment Attributes";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_rpg_character_equipment_attributes", typeof(Texture2D));
            showExpandButton = false;
        }
        
        protected override void OnBaseInspectorGUI()
        {
            // update serialized object
            serializedObject.Update();
            
            DrawPropertiesExcluding(serializedObject, ExcludedProperties);

            // apply modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
