#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Character;
using UnityEngine;

namespace Eadon.Rpg.Invector.Magic
{
    public abstract class EadonSpellBase : MonoBehaviour
    {
        [HideInInspector]
        public GameObject spellcaster;
        [HideInInspector]
        public GameObject spellParent;

        public float destroyTime;
        public bool useSpellScale;

        protected float timer;
        protected float spellScale;
        protected EadonRpgCharacter character;
        
        protected virtual void Start()
        {
            timer = 0;
            if (spellcaster == null) return;
            character = spellcaster.GetComponent<EadonRpgCharacter>();
            spellScale = character ? character.CalculateSpellScale() : 1f;
        }

        private void LateUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= destroyTime)
            {
                BeforeDestruction();
                Destroy(spellParent);
            }
        }

        protected virtual void BeforeDestruction()
        {
            
        }
    }
}
#endif
