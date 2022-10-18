#if EADON_RPG_INVECTOR
using Eadon.Rpg.Invector.Configuration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.Character
{
    public class AvailableTalentPanel : MonoBehaviour, IPointerClickHandler
    {
        public Text talentNameText;
        public Image talentIcon;
        [HideInInspector]
        public EadonRpgTalent talent;
        [HideInInspector]
        public EadonCharacterTalentsDisplayManager displayManager;
        public Button getButton;
        public Text buttonLabel;

        public void GetTalent()
        {
            displayManager.GetTalent(talent);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            displayManager.ShowDescription(talent);
        }
    }
}
#endif
