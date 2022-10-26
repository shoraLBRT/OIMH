#if EADON_RPG_INVECTOR
namespace Eadon.Rpg.Invector.ClothingSystem
{
    [System.Serializable]
    public class OnSelectClothingEquipArea : UnityEngine.Events.UnityEvent<ClothingEquipArea> { }
    
    [System.Serializable]
    public class ClothingEquippedEvent : UnityEngine.Events.UnityEvent<int> { }
}
#endif
