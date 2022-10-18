using System.ComponentModel;
namespace Invector.vItemManager {
     public enum vItemType {
       [Description("")] Consumable=0,
       [Description("Melee")] MeleeWeapon=1,
       [Description("Shooter")] ShooterWeapon=2,
       [Description("(VALUE)")] Ammo=3,
       [Description("")] Archery=4,
       [Description("")] Builder=5,
       [Description("")] Defense=6,
       [Description("")] Spell=7,
       [Description("")] Money=8,
       [Description("")] Clothing=9,
       [Description("")] ClothingSet=10
     }
     public enum vItemAttributes {
       [Description("")] Health=0,
       [Description("")] Stamina=1,
       [Description("<i>Damage</i> : <color=red>(VALUE)</color>")] Damage=2,
       [Description("")] StaminaCost=3,
       [Description("")] DefenseRate=4,
       [Description("")] DefenseRange=5,
       [Description("(VALUE)")] AmmoCount=6,
       [Description("")] MaxHealth=7,
       [Description("")] MaxStamina=8,
       [Description("(VALUE)")] SecundaryAmmoCount=9,
       [Description("")] SecundaryDamage=10,
       [Description("")] MagicID=11,
       [Description("")] ManaCost=12,
       [Description("")] DamageType=13,
       [Description("")] BuyPrice=14,
       [Description("")] SellPrice=15,
       [Description("")] ClothingHolder=16,
       [Description("")] ClothingID=17,
       [Description("")] ItemWeight=18
     }
}
