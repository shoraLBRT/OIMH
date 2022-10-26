#if EADON_RPG_INVECTOR
using UnityEngine;

namespace Eadon.Rpg.Invector.ClothingSystem
{
    [CreateAssetMenu(menuName = "Eadon/Inventory/Items/Position Offset")]
    public class PositionOffset : ScriptableObject
    {
        public Vector3 pos;
        public Vector3 rot;
    }
}
#endif
