#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Configuration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.Character
{
    public class TalentPanel : MonoBehaviour, IPointerClickHandler
    {
        public Text talentNameText;
        public Image talentIcon;
        [HideInInspector]
        public EadonRpgTalent talent;
        [HideInInspector]
        public EadonCharacterTalentsDisplayManager displayManager;

        public void OnPointerClick(PointerEventData eventData)
        {
            displayManager.ShowDescription(talent);
        }
    }
}
#endif
