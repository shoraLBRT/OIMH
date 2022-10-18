#if EADON_RPG_INVECTOR
using Eadon.EadonRPG.Scripts.Editor;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic.Editor
{
    [CustomEditor(typeof(EadonSpellAttack), true)]
    public class EadonSpellAttackInspector : EadonBaseEditor
    {
        private EadonSpellAttack _eadonTarget;

        protected void OnEnable()
        {
            editorTitle = "Eadon Spell Attack";
            splashTexture = (Texture2D)Resources.Load("Textures/eadon_rpg_spell_attack", typeof(Texture2D));

            _eadonTarget = (EadonSpellAttack)target;
        }

        protected override void OnBaseInspectorGUI()
        {
            var spellAttack = (EadonSpellAttack)target;

            // update serialized object
            serializedObject.Update();

            // cache serialized properties
            var leftHandParticleEffect = serializedObject.FindProperty("leftHandParticleEffect");
            var rightHandParticleEffect = serializedObject.FindProperty("rightHandParticleEffect");
            var pChargeState = serializedObject.FindProperty("chargeState");

            GUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.PropertyField(leftHandParticleEffect, new GUIContent("Left Hand Particle Effect:", "Prefab of the particle system to attach to the left hand"));
            EditorGUILayout.PropertyField(rightHandParticleEffect, new GUIContent("Right Hand Particle Effect:", "Prefab of the particle system to attach to the right hand"));
            EditorGUILayout.PropertyField(pChargeState, new GUIContent("Is Charge State:", "Is this a charge state?"));
            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Space(8); // SPACE

            EditorGUILayout.BeginVertical(boxStyle);

            // add new spawn button
            EditorGUILayout.LabelField("Spawn Prefabs", EditorStyles.boldLabel);
            if (GUILayout.Button("Add New"))
            {
                spellAttack.spawnOverTime.Add(new EadonAnimatorRuntimeSpawner()
                {
                    spawnStartTime = 0.5f,
                    spawnEndTime = 0.5f,
                    numberToSpawn = 1,
                    destructionTimeOut = 4f,
                });
            }
            // list of spawn ables                    
            for (var s = 0; s < spellAttack.spawnOverTime.Count; s++)
            {
                // container
                GUILayout.BeginVertical("box");
                var spawnDetail = spellAttack.spawnOverTime[s];

                // start/end time
                GUILayout.BeginHorizontal();
                spawnDetail.spawnStartTime = EditorGUILayout.FloatField("Spawn Start:", spawnDetail.spawnStartTime);
                GUILayout.Label("End:", GUILayout.Width(50));
                spawnDetail.spawnEndTime = EditorGUILayout.FloatField(spawnDetail.spawnEndTime);
                GUILayout.EndHorizontal();

                // prefab                                                
                spawnDetail.prefab = EditorGUILayout.ObjectField("Prefab: ", spawnDetail.prefab, typeof(GameObject), false) as GameObject;
                if (spawnDetail.prefab)
                {
                    GUILayout.BeginHorizontal();
                    spawnDetail.numberToSpawn = EditorGUILayout.IntField("Spawn Quantity:", spawnDetail.numberToSpawn);
                    GUILayout.Label("Timeout:", GUILayout.Width(50));
                    spawnDetail.destructionTimeOut = EditorGUILayout.FloatField(spawnDetail.destructionTimeOut);
                    GUILayout.EndHorizontal();
                }

                // audio clip
                spawnDetail.spawnAudioClip = EditorGUILayout.ObjectField("Audio Clip: ", spawnDetail.spawnAudioClip, typeof(AudioClip), false) as AudioClip;

                // audio source
                if (spawnDetail.spawnAudioClip)
                {
                    spawnDetail.audioSource = EditorGUILayout.ObjectField("Audio Source: ", spawnDetail.audioSource, typeof(AudioSource), false) as AudioSource;
                }

                // advanced options
                if (spawnDetail.showAdvancedOptions && spawnDetail.prefab)
                {
                    // offset/rotation
                    EditorGUILayout.LabelField("Advanced Spawning", EditorStyles.boldLabel);
                    spawnDetail.offset = EditorGUILayout.Vector3Field("Offset: ", spawnDetail.offset);
                    spawnDetail.angle = EditorGUILayout.Vector3Field("Angle: ", spawnDetail.angle);
                    
                    // parenting
                    GUILayout.BeginHorizontal();
                    spawnDetail.keepParent = EditorGUILayout.Toggle("Keep Parent:", spawnDetail.keepParent);

                    // spawn location
                    spawnDetail.useRootTransform = EditorGUILayout.Toggle("Use Root Transform:", spawnDetail.useRootTransform);
                    GUILayout.EndHorizontal();
                }

                // delete this spawn
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Delete"))
                {
                    spellAttack.spawnOverTime.RemoveAt(s);  // delete this entry
                    break;  // force redisplay of the list
                }

                // show expanded options
                if (spawnDetail.prefab)
                {
                    if (GUILayout.Button((spawnDetail.showAdvancedOptions ? "Less" : "More")))
                    {
                        spawnDetail.showAdvancedOptions = !spawnDetail.showAdvancedOptions;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif
