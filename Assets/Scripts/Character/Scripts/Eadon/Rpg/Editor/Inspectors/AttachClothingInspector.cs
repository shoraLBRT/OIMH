#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using Eadon.Rpg.Invector.ClothingSystem;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor.Inspectors
{
    [CustomEditor(typeof(AttachClothing))]
    public class AttachClothingInspector : EadonBaseEditor
    {
        private static readonly string[] ExcludedProperties = new string[] { "m_Script" };
        private AttachClothing _attachClothing;
        
        private void OnEnable()
        {
            editorTitle = "Eadon RPG Attach Clothing";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_rpg_attach_clothing", typeof(Texture2D));
            showExpandButton = false;
            _attachClothing = (AttachClothing) target;
        }
        
        protected override void OnBaseInspectorGUI()
        {
            // update serialized object
            serializedObject.Update();
            
            var rootBone = serializedObject.FindProperty("rootBone");
            var rootBoneName = serializedObject.FindProperty("rootBoneName");
            var rendererBones = serializedObject.FindProperty("rendererBones");
            var mainBody = serializedObject.FindProperty("mainBody");
            var blendshapes = serializedObject.FindProperty("blendshapes");
            var maskTexture = serializedObject.FindProperty("maskTexture");
            
            EditorGUILayout.PropertyField(rootBone, new GUIContent("Root Bone"), false);
            EditorGUILayout.PropertyField(rootBoneName, new GUIContent("Root Bone Name"), false);
            EditorGUILayout.PropertyField(rendererBones, new GUIContent("Renderer Bones"), true);
            EditorGUILayout.PropertyField(mainBody, new GUIContent("Main Body"), false);
            EditorGUILayout.PropertyField(blendshapes, new GUIContent("Blendshapes"), true);
            EditorGUILayout.PropertyField(maskTexture, new GUIContent("Mask Texture"), false);

            EditorGUILayout.Space();
            
            if (GUILayout.Button("Copy blendshapes"))
            {
                _attachClothing.CopyBlendshapes();
            }
            
            // apply modified properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
