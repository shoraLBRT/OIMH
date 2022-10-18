#if EADON_RPG_INVECTOR
using System.IO;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Environment;
using Eadon.Rpg.Invector.Magic;
using UnityEngine;
using UnityEditor;

namespace Eadon.RPG
{
    /// <summary>
    /// Unity menu item links to add the core classes to the active game object
    /// </summary>
    public partial class EadonRpgComponents
    {
        [MenuItem("Invector/Eadon RPG/Character Components/Character Equip Attributes")]
        private static void CharacterEquipAttributesMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<EadonCharacterEquipmentAttributes>();
            else
                Debug.Log("No Active Selection");
        }

        [MenuItem("Invector/Eadon RPG/Spell Components/Magic Projectile")]
        private static void MagicProjectileMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<EadonMagicProjectile>();
            else
                Debug.Log("No Active Selection");
        }

        [MenuItem("Invector/Eadon RPG/Spell Components/Magic Healing")]
        private static void MagicHealingMenu()
        {
            if (Selection.activeGameObject)
                Selection.activeGameObject.AddComponent<EadonAreaHealingSpell>();
            else
                Debug.Log("No Active Selection");
        }
    }
}

#endif
