#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using Eadon.Rpg.Invector.Character;
using Invector.vCharacterController.AI;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    /// <summary>
    /// Attached to a spell animator state, causes hand effects and spawning of spells.
    /// </summary>
    /// <remarks>
    /// Critical, causes the hand particle IK tracking and the spell instances to be
    /// spawned at specific points in the animation.
    /// </remarks>
    public class EadonSpellAttack : StateMachineBehaviour
    {
        public GameObject leftHandParticleEffect;
        public GameObject rightHandParticleEffect;
        public bool chargeState;
        public List<EadonAnimatorRuntimeSpawner> spawnOverTime = new List<EadonAnimatorRuntimeSpawner>();

        private GameObject _leftHandParticleInstance;
        private GameObject _rightHandParticleInstance;
        private Transform _spawnPoint;
        private bool _isAi;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _spawnPoint = animator.GetComponentInChildren<EadonRpgCharacter>().magicSpawnPoint;
            if (_spawnPoint == null)
            {
                _spawnPoint = animator.gameObject.transform;
            }

#if INVECTOR_AI_TEMPLATE
            var controlAi = animator.gameObject.GetComponent(typeof(vControlAI));
            if (controlAi != null)
            {
                _isAi = true;
            }
#endif
            var ai = animator.gameObject.GetComponent<vSimpleMeleeAI_Controller>();
            if (ai != null)
            {
                _isAi = true;
            }

            if (leftHandParticleEffect != null && !_leftHandParticleInstance)
            {
                _leftHandParticleInstance = Instantiate(leftHandParticleEffect, animator.GetBoneTransform(HumanBodyBones.LeftHand));
            }
            if (rightHandParticleEffect != null && !_rightHandParticleInstance)
            {
                _rightHandParticleInstance = Instantiate(rightHandParticleEffect, animator.GetBoneTransform(HumanBodyBones.RightHand));
            }

            if (chargeState)
            {
                foreach (var itemToSpawn in spawnOverTime)
                {
                    if (itemToSpawn.useRootTransform)
                    {
                        itemToSpawn.spawnedGameObject = itemToSpawn.Spawn(animator.gameObject.transform,
                            animator.rootPosition, Quaternion.identity,
                            (_isAi ? SpellTarget.Friend : SpellTarget.Enemy));
                    }
                    else
                    {
                        itemToSpawn.spawnedGameObject = itemToSpawn.Spawn(animator.gameObject.transform, _spawnPoint,
                            (_isAi ? SpellTarget.Friend : SpellTarget.Enemy)); // spawn
                    }
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_leftHandParticleInstance)
            {
                Destroy(_leftHandParticleInstance);
                _leftHandParticleInstance = null;
            }

            if (_rightHandParticleInstance)
            {
                Destroy(_rightHandParticleInstance);
                _rightHandParticleInstance = null;
            }

            if (chargeState)
            {
                foreach (var itemToSpawn in spawnOverTime)
                {
                    Destroy(itemToSpawn.spawnedGameObject); // destroy
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!chargeState)
            {
                foreach (var itemToSpawn in spawnOverTime)
                {
                    itemToSpawn.Spawn(animator.gameObject.transform, _spawnPoint, stateInfo.normalizedTime % 1,
                        animator, (_isAi ? SpellTarget.Friend : SpellTarget.Enemy)); // spawn
                }
            }
        }
    }
}
#endif
