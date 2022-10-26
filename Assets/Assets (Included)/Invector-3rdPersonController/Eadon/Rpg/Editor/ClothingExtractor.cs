#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using System.Linq;
using Eadon.Rpg.Invector.ClothingSystem;
using Eadon.Rpg.Invector.Utils;
using UnityEditor;
using UnityEngine;

namespace Eadon.CharacterController.Editor
{
    public class SkinnedMeshConfig
    {
        public string Name;
        public bool IsMainBody;
        public bool Extract;
        public string PrefabName;
    }
    
    public class ClothingExtractor : EditorWindow
    {
        private bool _selectionScanned;
        private Vector2 _scrollPos;
        private GUIStyle _redStyle;
        private string _clothingPrefabTag;
        
        private readonly Dictionary<string, SkinnedMeshRenderer> _skinnedMeshRenderersDict =
            new Dictionary<string, SkinnedMeshRenderer>();
        private readonly List<SkinnedMeshConfig> _bodyConfig = new List<SkinnedMeshConfig>();
        private readonly Dictionary<string, HumanBodyBones> _modelBones = new Dictionary<string, HumanBodyBones>();

        [MenuItem("Invector/Eadon RPG/Tools/Extract Clothing From Model")]
        private static void ShowWindow()
        {
            var window = GetWindow<ClothingExtractor>();
            window.titleContent = new GUIContent("Extract Clothing From Model");
            window.minSize = new Vector2(512, 800);
            window.maxSize = new Vector2(512, 800);
            window.SetupStyles();
            window.Show();
        }

        private void OnGUI()
        {
            var splashTexture = (Texture2D) Resources.Load("Textures/eadon_rpg_clothing_extractor", typeof(Texture2D));
            GUILayout.Box(splashTexture);

            EditorGUILayout.Space();

            _clothingPrefabTag = EditorGUILayout.TextField("Prefab Tag", _clothingPrefabTag);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            if (Selection.activeGameObject == null)
            {
                EditorGUILayout.LabelField("Please select a GameObject in the scene", _redStyle);
            }
            else
            {
                if (!_selectionScanned)
                {
                    ScanSelection();
                }

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(512), GUILayout.Height(600));

                foreach (var bodyPartConfig in _bodyConfig)
                {
                    DisplayBodyPartConfig(bodyPartConfig);
                }
                
                EditorGUILayout.EndScrollView();

                GUILayout.FlexibleSpace();

                if (ChoiceValid())
                {
                    if (GUILayout.Button("Create"))
                    {
                        ExtractClothing();
                    }
                }
            }
            
            EditorGUILayout.EndVertical();
        }

        private void SetupStyles()
        {
            _redStyle = new GUIStyle {normal = {textColor = Color.red}};
        }
        
        private void ScanSelection()
        {
            _skinnedMeshRenderersDict.Clear();
            
            var currentGameObject = Selection.activeGameObject;
            var skinnedMeshRenderers = currentGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                _skinnedMeshRenderersDict.Add(skinnedMeshRenderer.name, skinnedMeshRenderer);
                _bodyConfig.Add(new SkinnedMeshConfig { Name = skinnedMeshRenderer.name, Extract = true, PrefabName = skinnedMeshRenderer.name});
            }
            
            ReadSkeleton(currentGameObject);

            _selectionScanned = true;
        }

        private void DisplayBodyPartConfig(SkinnedMeshConfig skinnedMeshConfig)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(skinnedMeshConfig.Name, EditorStyles.boldLabel);
            skinnedMeshConfig.IsMainBody = EditorGUILayout.Toggle("Is Main Body", skinnedMeshConfig.IsMainBody);
            skinnedMeshConfig.Extract = EditorGUILayout.Toggle("Extract", skinnedMeshConfig.Extract);
            if (skinnedMeshConfig.Extract)
            {
                skinnedMeshConfig.PrefabName = EditorGUILayout.TextField("Prefab Name", skinnedMeshConfig.PrefabName);
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private bool ChoiceValid()
        {
            if (_selectionScanned)
            {
                foreach (var bodyPartConfig in _bodyConfig)
                {
                    if (bodyPartConfig.IsMainBody)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ExtractClothing()
        {
            var absolutePath = EditorUtility.OpenFolderPanel("Select Folder To Save Meshes and Prefabs", "", "");
            var localPath = FileUtility.AssetsRelativePath(absolutePath);
            
            var meshFolder = localPath + "/Meshes";
            if (!AssetDatabase.IsValidFolder(meshFolder))
            {
                AssetDatabase.CreateFolder(localPath, "Meshes");
            }

            var prefabFolder = localPath + "/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabFolder))
            {
                AssetDatabase.CreateFolder(localPath, "Prefabs");
            }

            var itemsFolder = localPath + "/Inventory Items";
            if (!AssetDatabase.IsValidFolder(itemsFolder))
            {
                AssetDatabase.CreateFolder(localPath, "Inventory Items");
            }

            var mainBody = _bodyConfig.First(bc => bc.IsMainBody);
            
            foreach (var bodyPartConfig in _bodyConfig.Where(bodyPartConfig => bodyPartConfig.Extract))
            {
                var skinnedMeshRenderer = _skinnedMeshRenderersDict[bodyPartConfig.Name];
                var mesh = CopyMesh(skinnedMeshRenderer.sharedMesh);
                
                var newGameObject = new GameObject(bodyPartConfig.Name);
                newGameObject.transform.position = Vector3.zero;
                newGameObject.transform.rotation = Quaternion.identity;
                
                foreach (var component in skinnedMeshRenderer.gameObject.GetComponents<Component>())
                {
                    var componentType = component.GetType();
                    if (componentType == typeof(Transform)) continue;
                    Debug.Log("Found a component of type " + component.GetType());
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newGameObject);
                    Debug.Log( "Copied " + component.GetType() + " from " + skinnedMeshRenderer.gameObject.name + " to " + newGameObject.name);
                }

                var attachClothing = newGameObject.AddComponent<AttachClothing>();
                if (_modelBones.ContainsKey(skinnedMeshRenderer.rootBone.name))
                {
                    attachClothing.rootBone = _modelBones[skinnedMeshRenderer.rootBone.name];
                }

                attachClothing.rendererBones = skinnedMeshRenderer.bones.Select(transform => transform.name).ToArray();
                attachClothing.mainBody = mainBody.Name;
                
                var meshPath = $"{meshFolder}/{bodyPartConfig.PrefabName}.asset";
                meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);
                AssetDatabase.CreateAsset( mesh, meshPath );
                
                var prefabPath = $"{prefabFolder}/{bodyPartConfig.PrefabName}.prefab";
                prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(newGameObject, prefabPath, InteractionMode.AutomatedAction);
                
                var clothingItem = CreateInstance<EadonClothingItem>();
                clothingItem.name = bodyPartConfig.PrefabName;
                clothingItem.clothingType = ClothingItemType.SpawnAndAttachOnMainObject;
                clothingItem.clothingPrefabs.Add(new ClothingPrefab { prefabTag = _clothingPrefabTag, prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath)});
                var itemPath = $"{itemsFolder}/{bodyPartConfig.PrefabName}.asset";
                AssetDatabase.CreateAsset(clothingItem, itemPath);
                EditorUtility.SetDirty(clothingItem);
            }
            
            AssetDatabase.SaveAssets();
        }

        private static Mesh CopyMesh(Mesh originalMesh)
        {
            // COPY MESH AND SKINNING
            var newMesh = new Mesh
            {
                name = originalMesh.name,
                vertices = originalMesh.vertices,
                bounds = originalMesh.bounds,
                uv = originalMesh.uv,
                uv2 = originalMesh.uv2,
                normals = originalMesh.normals,
                colors = originalMesh.colors,
                tangents = originalMesh.tangents,
                triangles = originalMesh.triangles,
                bindposes = originalMesh.bindposes,
                boneWeights = originalMesh.boneWeights
            };

            // copy over blend shapes
            var dVertices = new Vector3[originalMesh.vertexCount];
            var dNormals = new Vector3[originalMesh.vertexCount];
            var dTangents= new Vector3[originalMesh.vertexCount];
            for (var shape = 0; shape < originalMesh.blendShapeCount; shape++) {
                for (var frame = 0; frame < originalMesh.GetBlendShapeFrameCount(shape); frame++) {
                    var shapeName = originalMesh.GetBlendShapeName(shape);
                    var frameWeight = originalMesh.GetBlendShapeFrameWeight(shape, frame);
 
                    originalMesh.GetBlendShapeFrameVertices(shape, frame, dVertices, dNormals, dTangents);
                    newMesh.AddBlendShapeFrame(shapeName, frameWeight, dVertices, dNormals, dTangents);
                }
            }

            return newMesh;
        }

        private void ReadSkeleton(GameObject gameObject)
        {
            var animator = gameObject.GetComponentInChildren<Animator>();
            
            _modelBones.Clear();
            
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.Chest).name, HumanBodyBones.Chest);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.Head).name, HumanBodyBones.Head);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.Hips).name, HumanBodyBones.Hips);
            var jawbone = animator.GetBoneTransform(HumanBodyBones.Jaw);
            if (jawbone != null)
            {
                _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.Jaw).name, HumanBodyBones.Jaw);
            }
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.Neck).name, HumanBodyBones.Neck);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.Spine).name, HumanBodyBones.Spine);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftFoot).name, HumanBodyBones.LeftFoot);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftHand).name, HumanBodyBones.LeftHand);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftShoulder).name, HumanBodyBones.LeftShoulder);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightFoot).name, HumanBodyBones.RightFoot);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightHand).name, HumanBodyBones.RightHand);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightShoulder).name, HumanBodyBones.RightShoulder);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.UpperChest).name, HumanBodyBones.UpperChest);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).name, HumanBodyBones.LeftLowerArm);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).name, HumanBodyBones.LeftLowerLeg);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).name, HumanBodyBones.LeftUpperArm);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).name, HumanBodyBones.LeftUpperLeg);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightLowerArm).name, HumanBodyBones.RightLowerArm);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).name, HumanBodyBones.RightLowerLeg);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightUpperArm).name, HumanBodyBones.RightUpperArm);
            _modelBones.Add(animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).name, HumanBodyBones.RightUpperLeg);
        }
    }
}

#endif
