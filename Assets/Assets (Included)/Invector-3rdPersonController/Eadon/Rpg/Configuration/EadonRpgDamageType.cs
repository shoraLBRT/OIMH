#if EADON_RPG_INVECTOR
using UnityEngine;

namespace Eadon.Rpg.Invector.Configuration
{
    [CreateAssetMenu(fileName = "New Eadon RPG Damage Type", menuName = "Eadon RPG/New Damage Type")]

    public class EadonRpgDamageType : ScriptableObject
    {
        [SerializeField]
        private string damageType = "";

        [SerializeField]
        private GameObject conditionEffect = null;

        public string DamageTypeName
        {
            get
            {
                return damageType;
            }
        }

        public GameObject DamageTypeConditionEffect
        {
            get
            {
                return conditionEffect;
            }
        }
    }
}
#endif
