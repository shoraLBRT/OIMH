#if EADON_RPG_INVECTOR
using System;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public enum SpellTarget
    {
        Any,
        Friend,
        Enemy
    }

    [Serializable]
    public class EadonRuntimeSpawner
    {
        public SpellTarget target;
        public GameObject prefab;
        public Vector3 offset;
        public Vector3 angle;
        public bool keepParent;
        public float destructionTimeOut;
        public AudioClip spawnAudioClip;
        public AudioSource audioSource;

#if UNITY_EDITOR
        [HideInInspector] public bool bInitialised;

        public virtual void New()
        {
            if (bInitialised) return;
            bInitialised = true;
            destructionTimeOut = 2;
        }
#endif

        public GameObject Spawn(Transform sender, Transform spawnPoint, SpellTarget whichTarget)
        {
            target = whichTarget;
            var goInstance = UnityEngine.Object.Instantiate(prefab, spawnPoint);
            goInstance = SetupInstance(sender, goInstance);

            return goInstance;
        }

        public GameObject Spawn(Transform sender, Vector3 position, Quaternion rotation, SpellTarget whichTarget)
        {
            target = whichTarget;
            var goInstance = UnityEngine.Object.Instantiate(prefab, position, rotation);
            goInstance = SetupInstance(sender, goInstance);

            return goInstance;
        }

        private GameObject SetupInstance(Transform sender, GameObject instance)
        {
            instance.SetActive(false);
            if (keepParent)
            {
                instance.transform.parent = sender;
                instance.transform.localPosition = Vector3.zero;
            }
            else
            {
                instance.transform.parent = null;
            }

            if (offset != Vector3.zero)
            {
                instance.transform.position = instance.transform.position + offset;
            }

            if (angle != Vector3.zero)
            {
                instance.transform.localRotation =
                    Quaternion.Euler(angle) * instance.transform.localRotation;
            }
            var projectile = instance.GetComponentInChildren<EadonMagicProjectile>();
            if (projectile != null)
            {
                projectile.overrideDamageSender = sender;
            }
            var spell = instance.GetComponentInChildren<EadonSpellBase>();
            if (spell != null)
            {
                spell.spellParent = instance;
            }
            instance.SetActive(true);
            
            return instance;
        }
    }

    [Serializable]
    public class EadonAnimatorRuntimeSpawner : EadonRuntimeSpawner
    {
        public float spawnStartTime;
        public float spawnEndTime;
        public int numberToSpawn;
        public bool useRootTransform;

        private float _spawnRate;
        [HideInInspector] public int spawnedSoFar = 999;
        [HideInInspector] public GameObject spawnedGameObject;
        [HideInInspector] public bool showAdvancedOptions;

#if UNITY_EDITOR
        public override void New()
        {
            if (bInitialised) return;
            base.New();
            spawnStartTime = 0.5f;
            spawnEndTime = 0.5f;
            numberToSpawn = 1;
        }
#endif

        public GameObject Spawn(Transform sender, Transform spawnPoint, float time, Animator animator, SpellTarget whichTarget)
        {
            if (time < 0.1 && spawnedSoFar >= numberToSpawn)
            {
                spawnedSoFar = 0;
                _spawnRate = (spawnEndTime - spawnStartTime) / numberToSpawn;
            }
            else if (spawnedSoFar < numberToSpawn)
            {
                if (!(time > spawnStartTime + _spawnRate * spawnedSoFar)) return null;
                spawnedSoFar += 1;
                if (prefab)
                {
                    spawnedGameObject = useRootTransform ? Spawn(sender, animator.rootPosition, Quaternion.identity, whichTarget) : Spawn(sender, spawnPoint, whichTarget);

                    var spell = (EadonSpellBase) spawnedGameObject.GetComponentInChildren(typeof(EadonSpellBase));
                    if (spell != null)
                    {
                        spell.spellcaster = sender.gameObject;
                    }
                }

                if (!spawnAudioClip || !audioSource || !audioSource.isActiveAndEnabled) return spawnedGameObject;
                if (audioSource.isPlaying) return spawnedGameObject;
                audioSource.clip = spawnAudioClip;
                audioSource.Play();

                return spawnedGameObject;
            }

            return null;
        }
    }
}
#endif
