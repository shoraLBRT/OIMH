#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.StashSystem;
using UnityEngine;
using UnityEditor;

namespace Eadon.RPG
{
    [CustomEditor(typeof(EadonStashManager))]
    public class EadonStashManagerInspector : EadonBaseEditor
    {
        private static readonly string[] ExcludedProperties = new string[] { "m_Script" };

        void OnEnable()
        {
            editorTitle = "Eadon Stash Manager";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_stash_manager", typeof(Texture2D));
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
