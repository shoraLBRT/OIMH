#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Environment;
using UnityEditor;
using UnityEngine;

namespace Eadon.RPG
{
    [CustomEditor(typeof(EadonDamageArea))]
    public class EadonRpgDamageAreaInspector : EadonBaseEditor
    {
        private static readonly string[] ExcludedProperties = new string[] { "m_Script" };

        void OnEnable()
        {
            editorTitle = "Eadon RPG Damage Area";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_rpg_damage_area", typeof(Texture2D));
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
