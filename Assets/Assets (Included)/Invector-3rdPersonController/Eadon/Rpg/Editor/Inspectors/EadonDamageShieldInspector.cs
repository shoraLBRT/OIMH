#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Magic;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg
{
    [CustomEditor(typeof(EadonDamageShieldSpell))]
    public class EadonDamageShieldInspector : EadonBaseEditor
    {
        private static readonly string[] ExcludedProperties = new string[] { "m_Script" };

        void OnEnable()
        {
            editorTitle = "Eadon Damage Shield";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_rpg_damage_shield", typeof(Texture2D));
            showExpandButton = false;
        }
        
        protected override void OnBaseInspectorGUI()
        {
            // update serialized object
            serializedObject.Update();

            // draw inspector
            DrawPropertiesExcluding(serializedObject, ExcludedProperties);

            // apply modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
