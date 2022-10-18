#if EADON_RPG_INVECTOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Magic;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
#if UNITY_EDITOR
#endif

namespace Eadon.Rpg.Invector.Utils
{
    /// <summary>
    /// Static global functions for the spell system.
    /// </summary>
    public class EadonGlobalFunctions : MonoBehaviour
    {
        /// <summary>Enable to output debugging messages in the console, set in magic settings.</summary>
        public static bool debuggingMessages = true;

        public static EadonRpgCharacter currentCharacter;

        /// <summary>
        /// Find the player object, via invector, or if not ready via tag search fall back.
        /// </summary>
        /// <returns>Player game object.</returns>
        public static GameObject FindPlayerInstance()
        {
            var goPotentialPlayers = GameObject.FindGameObjectsWithTag("Player");  // find all tagged player
            return goPotentialPlayers.FirstOrDefault(goMaybePlayer => goMaybePlayer.GetComponent<Animator>());
        }

        /// <summary>
        /// Find list of all targets within a radius.
        /// </summary>
        /// <param name="center">Center of the sphere cast.</param>
        /// <param name="range">Range to search within.</param>
        /// <param name="layers">Layer filter.</param>
        /// <param name="tags">Tag filter.</param>
        /// <param name="checkVisible">Ensure target is visible.</param>
        /// <param name="heightAdjust">Height adjustment for search.</param>
        /// <param name="mustHaveAnimator">Ensure target is alive.</param>
        /// <returns>Transform of the selected target.</returns>
        public static List<Transform> FindAllTargetsWithinRange(Vector3 center, float range, LayerMask layers, string[] tags, bool checkVisible, float heightAdjust, bool mustHaveAnimator)
        {
            // adjust center height for line of sight check
            var v3AdjustedCenter = center;
            v3AdjustedCenter.y = v3AdjustedCenter.y + heightAdjust;

            // find targets within sphere range by tag/layer
            var listTargetsInRange = new List<Transform>();  // empty list
            var cTargets = Physics.OverlapSphere(center, range, layers.value);  // who is close
            if (cTargets == null) return listTargetsInRange; // work complete
            // found some?
            foreach (var t in cTargets)
            {
                if (tags == null || tags.Length == 0 || tags.Contains(t.transform.tag))
                {  // tag matches
                    var bValid = false;  // start valid check
                    if (!mustHaveAnimator)
                    {  // looking for no life?
                        bValid = true;  // all good then
                    }
                    else
                    {  // must be alive?
                        var aTemp = t.transform.GetComponent<Animator>();  // grab animator
                        if (aTemp)
                        { // ensure only the actual character is returned
                            if (aTemp.enabled)
                            {  // and are not dead
                                bValid = true;  // also all good
                            }
                        }
                    }

                    if (!bValid) continue; // target either alive or we don't mind if not
                    if (checkVisible)
                    {
                        // ensure visible via linecast    
                        if (!Physics.Linecast(v3AdjustedCenter, t.transform.position, out var rhHit) ||
                            rhHit.transform != t.transform) continue;
                        listTargetsInRange.Add(t.transform);  // ok add to list 
                        if (debuggingMessages)
                        {
                            Debug.Log("Potential Target " + t.transform.name);
                        }
                    }
                    else
                    {  // targets can be round a corner
                        listTargetsInRange.Add(t.transform);  // ok add to list 
                        if (debuggingMessages)
                        {
                            Debug.Log("Potential Target " + t.transform.name);
                        }
                    }
                }
            }
            return listTargetsInRange;   // work complete
        }
    }
}

#endif
