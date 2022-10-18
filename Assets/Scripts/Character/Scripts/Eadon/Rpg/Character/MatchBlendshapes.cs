#if EADON_RPG_INVECTOR
using System;
using UnityEngine;

namespace Eadon.Rpg.Invector.Character
{
    public class MatchBlendshapes : MonoBehaviour
    {
        public GameObject bodyGameObject;
        public string[] blendshapes = new string[0];

        private SkinnedMeshRenderer _bodyRenderer;
        private SkinnedMeshRenderer _skinnedMeshRenderer;
        
        private void OnEnable()
        {
            _bodyRenderer = bodyGameObject.GetComponent<SkinnedMeshRenderer>();
            _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        private void Update()
        {
            if (_bodyRenderer != null && _skinnedMeshRenderer != null)
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
    }
}
#endif
