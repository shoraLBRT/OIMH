#if EADON_RPG_INVECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using Invector.vMelee;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic.Editor
{
    /// <summary>
    /// Used to determine which of the spell options to create a state from.
    /// </summary>
    // public enum SpellBookEntrySubType
    // {
    //     Casting, Charge, Hold, Release
    // }

    /// <summary>
    /// Which hands (or none) to attach hand particles to.
    /// </summary>
    public enum SpellBookHands
    {
        None, Left, Right, Both
    }

    [Serializable]
    public class EadonSpellBookEntry
    {
        /// <summary>Animation cast clip.</summary>
        [NonSerialized]
        public AnimationClip sectionClip;
        public string sectionClipAssetPath;

        /// <summary>Speed of the cast animation.</summary>
        public float speed;

        /// <summary>Mirror the cast animation.</summary>
        public bool mirror;

        /// <summary>Add foot ik to the cast animation.</summary>
        public bool footIk;

        /// <summary>Cycle offset for the cast animation.</summary>
        public float cycleOffset;

        /// <summary>Prefab of the particle system to attach to the limb.</summary>
        [NonSerialized]
        public GameObject leftHandParticleEffect;
        public string leftHandParticleEffectAssetPath;

        /// <summary>(Optional) The 2nd Prefab of the particle system to attach to second limb.</summary>
        [NonSerialized]
        public GameObject rightHandParticleEffect;
        public string rightHandParticleEffectAssetPath;

        /// <summary>For the charge animator state, destroys all spawned on state exit.</summary>
        public bool chargeState;

        /// <summary>List of all prefabs to spawn within the animator time frame.</summary>
        public List<EadonSpellBookSpawnPrefab> spawnOverTime = new List<EadonSpellBookSpawnPrefab>();

        /// <summary>Sub type, for display alterations, know which type of cast thyself is.</summary>
//        public SpellBookEntrySubType subType;

        /// <summary>Icon for the spell from the inventory.</summary>
        public Sprite icon;

        /// <summary>Name of the spell from the inventory.</summary>
        public string spellName;

        public EadonSpellBookEntry(/*SpellBookEntrySubType type*/)
        {
//            subType = type;
            speed = 1;
        }

//        public string SubTypeLabel => subType.ToString();

        public List<EadonAnimatorRuntimeSpawner> GetRuntimeSpawners()
        {
            return spawnOverTime.Select(spawnPrefab => spawnPrefab.GetSpawner()).ToList();
        }
    }
}

#endif
