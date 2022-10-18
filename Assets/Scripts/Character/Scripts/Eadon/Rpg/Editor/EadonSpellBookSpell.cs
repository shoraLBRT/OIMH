#if EADON_RPG_INVECTOR
using Invector.vMelee;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Magic;
using Eadon.Rpg.Invector.Magic.Editor;
using UnityEngine;

namespace Eadon.RPG
{
//    [CreateAssetMenu(fileName = "New Eadon RPG Spell", menuName = "Eadon RPG/New Spell")]

    [Serializable]
    public class EadonSpellBookSpell
    {
        public string SpellName = "";

        /// <summary>Magic ID in the inventory that this entry links to.</summary>
        public int MagicID = 0;
        /// <summary>Mana Cost in the inventory that this entry links to.</summary>
        public int ManaCost = 0;

        [NonSerialized]
        public Texture2D Icon;
        public string IconAssetPath;

        public bool UpperBodyOnly = false;

        public bool valid = true;

        /// <summary>Filter the list based upon damage.</summary>
        [NonSerialized]
        public EadonRpgDamageType DamageType = null;
        public string DamageTypeName;

        [NonSerialized]
        public AnimationClip sectionClip;
        public string sectionClipAssetPath;

        /// <summary>Speed of the cast animation.</summary>
        public float speed = 1;

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
        
        public override string ToString()
        {
            return $"{SpellName}\nMagic ID:{MagicID}\nMana Cost:{ManaCost}\nDamage Type:{DamageTypeName}";
        }

        public List<EadonAnimatorRuntimeSpawner> GetRuntimeSpawners()
        {
            return spawnOverTime.Select(spawnPrefab => spawnPrefab.GetSpawner()).ToList();
        }
    }
}
#endif
