#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Utils;
using UnityEditor;
using UnityEngine;

namespace Eadon.Rpg.Invector.Editor
{
    public class EadonRefreshConditions
    {
        [MenuItem("Invector/Eadon RPG/Refresh Conditions")]
        public static void RefreshConditions()
        {
            var selectedGameObject = Selection.activeGameObject;
            if (selectedGameObject != null)
            {
                var character = selectedGameObject.GetComponent<EadonRpgCharacterBase>();
                if (character != null)
                {
                    for(var index = 0; index < character.eadonRpgCharacterConfig.DamageTypes.Length; index++)
                    {
                        var damageType = character.eadonRpgCharacterConfig.DamageTypes[index];

                        var displayGameObjectTransform = selectedGameObject.transform.Find($"Conditions/{damageType.DamageTypeName}");
                        if (displayGameObjectTransform != null)
                        {
                            character.conditions[damageType.DamageTypeName].display = displayGameObjectTransform.gameObject;
                        }
                    }
                }
            }
        }
    }
}
#endif
