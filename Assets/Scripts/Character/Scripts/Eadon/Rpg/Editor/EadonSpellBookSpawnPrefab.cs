#if EADON_RPG_INVECTOR
using System;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic.Editor
{
    [Serializable]
    public class EadonSpellBookSpawnPrefab
    {
        [NonSerialized]
        public GameObject prefab;
        public string prefabAssetPath;
        public float spawnStartTime;
        public float spawnEndTime;
        public int numberToSpawn;
        public float destructionTimeOut;
        [NonSerialized]
        public AudioClip spawnAudioClip;
        public string spawnAudioClipAssetPath;
        [NonSerialized]
        public AudioSource audioSource;
        public string audioSourceAssetPath;
        public Vector3 offset;
        public Vector3 angle;
        public bool keepParent;
        public bool useRootTransform;

        public EadonAnimatorRuntimeSpawner GetSpawner()
        {
            var spawner = new EadonAnimatorRuntimeSpawner
            {
                prefab = prefab,
                spawnStartTime = spawnStartTime,
                spawnEndTime = spawnEndTime,
                numberToSpawn = numberToSpawn,
                destructionTimeOut = destructionTimeOut,
                spawnAudioClip = spawnAudioClip,
                audioSource = audioSource,
                offset = offset,
                angle = angle,
                keepParent = keepParent,
                useRootTransform = useRootTransform,
            };

            return spawner;
        }
    }
}
#endif
