#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using Eadon.Rpg.Invector.Character;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    /// <summary>
    /// Stores elemental damage for the character damage mitigation.
    /// </summary>
    public class EadonMagicObjectDamage : MonoBehaviour
    {
        /// <summary>Elemental magic to apply alongside the base physical damage from the invector damage script.</summary>
        [Tooltip("Elemental magic to apply alongside the base physical damage from the invector damage script")]
        public List<EadonMagicDamageOverTime> damage = new List<EadonMagicDamageOverTime>();

        /// <summary>Apply damage within a specified radius on hit, set to greater than zero to enable.</summary>
        [Tooltip("Apply damage within a specified radius on hit, set to greater than zero to enable")]
        public float aoeRadius;

        /// <summary>Radius target assignment, note this is auto updated when fired from the animator.</summary>
        [Tooltip("Radius target assignment, note this is auto updated when fired from the animator")]
        public SpellTarget aoeTarget;
    }
}
#endif

