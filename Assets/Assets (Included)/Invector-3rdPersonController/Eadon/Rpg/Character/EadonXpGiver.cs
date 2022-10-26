#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Utils;
using Invector.vCharacterController;
using UnityEngine;

namespace Eadon.Rpg.Invector.Character
{
    public class EadonXpGiver : MonoBehaviour
    {
        public int xpToGive;

        /// <summary>
        /// Call this method on NPC/Monster/Enemy/ect death to assign XPs to the player
        /// </summary>
        public void GiveXpToPlayer()
        {
            var player = EadonGlobalFunctions.FindPlayerInstance();
            if (player != null)
            {
                var playerCharacter = player.GetComponent<EadonRpgCharacter>();
                if (playerCharacter != null)
                {
                    playerCharacter.AddXp(xpToGive);
                }
            }
        }
    }
}
#endif
