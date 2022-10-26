#if EADON_RPG_INVECTOR
using System.Linq;
using CogsAndGoggles.Library.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    [ExecuteAlways]
    public class AttachClothing : MonoBehaviour
    {
        public HumanBodyBones rootBone;
        public string rootBoneName;
        public string[] rendererBones;
        public string mainBody;
        public string[] blendshapes = new string[0];
        public Texture2D maskTexture;

        private SkinnedMeshRenderer _skinnedMeshRenderer;
        private SkinnedMeshRenderer _bodyRenderer;
        private bool _isSkinned;

        private void OnEnable()
        {
            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (_skinnedMeshRenderer == null)
            {
                return;
            }
            var animator = GetComponentInParent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("AttachClothing: This component needs an Animator in the parent game object");
                return;
            }

            if (!string.IsNullOrWhiteSpace(rootBoneName))
            {
                foreach (Transform eachChild in animator.gameObject.transform) 
                {
                    if (eachChild.name == rootBoneName) {
                        _skinnedMeshRenderer.rootBone = eachChild;
                    }
                }
            }
            else
            {
                _skinnedMeshRenderer.rootBone = animator.GetBoneTransform(rootBone);
            }
//            var bodyGameObject = animator.transform.Find(mainBody);
            var bodyGameObject = animator.transform.FirstChildOrDefault(x => x.name == mainBody);
            _bodyRenderer = bodyGameObject.GetComponent<SkinnedMeshRenderer>();
            if (_bodyRenderer != null)
            {
                _skinnedMeshRenderer.bones = RestoreBones(_bodyRenderer);
            }
            
            CopyBlendshapes();
            
            _isSkinned = true;
        }

        private void Update()
        {
            if (_isSkinned)
            {
                foreach (var blendshape in blendshapes)
                {
                    if (_bodyRenderer != null && _skinnedMeshRenderer != null && !string.IsNullOrWhiteSpace(blendshape))
                    {
                        var sourceIndex = _bodyRenderer.sharedMesh.GetBlendShapeIndex(blendshape);
                        var localIndex = _skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendshape);
            
                        if (sourceIndex != -1 && localIndex != -1)
                        {
                            _skinnedMeshRenderer.SetBlendShapeWeight(localIndex, _bodyRenderer.GetBlendShapeWeight(sourceIndex));
                        }
                    }
                }
            }
        }

        private Transform[] RestoreBones(SkinnedMeshRenderer bodyRenderer)
        {
            return rendererBones.Select(bone => bodyRenderer.bones.First(b => b.name == bone)).ToArray();
        }

        public void CopyBlendshapes()
        {
            var smr = GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                blendshapes = new string[smr.sharedMesh.blendShapeCount];
                for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
                {
                    blendshapes[i] = smr.sharedMesh.GetBlendShapeName(i);
                }
            }
        }
    }
}
#endif
