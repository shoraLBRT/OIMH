#if EADON_RPG_INVECTOR
using System;

namespace Eadon.Rpg.Invector.Character
{
    /// <summary>
    /// Character system attributes updated for the update HUD event.
    /// </summary>
    public class EadonCharacterUpdatedEventArgs : EventArgs
    {
        /// <summary>Current XP.</summary>
        public int Xp { get; set; }

        /// <summary>Current character level.</summary>
        public int Level { get; set; }

        /// <summary>Current HP.</summary>
        public float Life { get; set; }

        /// <summary>Maximum HP.</summary>
        public int LifeMax { get; set; }

        /// <summary>Current mana.</summary>
        public float Mana { get; set; }

        /// <summary>Maximum mana.</summary>
        public int ManaMax { get; set; }

        /// <summary>Maximum stamina.</summary>
        public int StaminaMax { get; set; }
    }
}
#endif
