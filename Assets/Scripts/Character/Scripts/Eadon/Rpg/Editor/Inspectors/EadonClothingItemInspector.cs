#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem.Editor
{
    [CustomEditor(typeof(EadonClothingItem))]
    public class EadonClothingItemInspector : EadonBaseEditor
    {
        private static readonly string[] ExcludedProperties = new string[] { "m_Script" };

        private EadonClothingItem _clothingItem;
        private SerializedObject _serializedClothingItem;

        private void OnEnable()
        {
            editorTitle = "Eadon Clothing Item";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_rpg_clothing_item", typeof(Texture2D));
            showExpandButton = false;
            _clothingItem = (EadonClothingItem) target;
            _serializedClothingItem = serializedObject;

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
