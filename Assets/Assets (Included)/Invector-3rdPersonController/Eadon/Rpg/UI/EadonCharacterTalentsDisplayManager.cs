#if EADON_RPG_INVECTOR
using CogsAndGoggles.Library.Extensions;
using Eadon.Rpg.Invector.Configuration;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.Character
{
    public class EadonCharacterTalentsDisplayManager : MonoBehaviour
    {
        public GameObject talentsPanel;
        public GameObject availableTalentsPanel;
        public Text talentsLeftText;
        public Text talentNameText;
        public Text talentDescriptionText;
        public Text talentPrerequisitesText;
        public GameObject talentPrefab;
        public GameObject availableTalentPrefab;

        private EadonRpgCharacter _character;

        // Start is called before the first frame update
        private void OnEnable()
        {
            var playerGameObject = GetComponentInParent<vThirdPersonController>();
            if (playerGameObject == null) return;

            _character = playerGameObject.GetComponent<EadonRpgCharacter>();
            if (_character == null) return;

            DisplayTalents();
        }

        private void DisplayTalents()
        {
            talentsLeftText.text = $"({_character.talentsAvailable} available)";
            
            talentsPanel.transform.Clear();
            availableTalentsPanel.transform.Clear();

            foreach (var talent in _character.eadonRpgCharacterConfig.AvailableTalents)
            {
                if (_character.acquiredTalents.Contains(talent.TalentName))
                {
                    var talentBlock = Instantiate(talentPrefab, talentsPanel.transform);
                    var talentPanel = talentBlock.GetComponent<TalentPanel>();
                    talentPanel.talent = talent;
                    talentPanel.displayManager = this;
                    talentPanel.talentNameText.text = talent.TalentName;
                    talentPanel.talentIcon.sprite = talent.TalentIcon;
                }
                else
                {
                    var talentBlock = Instantiate(availableTalentPrefab, availableTalentsPanel.transform);
                    var talentPanel = talentBlock.GetComponent<AvailableTalentPanel>();
                    talentPanel.talent = talent;
                    talentPanel.displayManager = this;
                    talentPanel.talentNameText.text = talent.TalentName;
                    talentPanel.talentIcon.sprite = talent.TalentIcon;
                    if (_character.talentsAvailable > 0 && talent.CanBeAcquiredBy(_character))
                    {
                        talentPanel.getButton.interactable = true;
                        talentPanel.buttonLabel.text = "Get";
                    }
                    else
                    {
                        talentPanel.getButton.interactable = false;
                        talentPanel.buttonLabel.text = "N/A";
                    }
                }
            }
        }

        public void ShowDescription(EadonRpgTalent talent)
        {
            talentNameText.text = talent.TalentName;
            talentDescriptionText.text = talent.TalentDescription;
            talentPrerequisitesText.text = talent.GetRequirements(true);
        }

        public void GetTalent(EadonRpgTalent talent)
        {
            talent.ApplyTo(_character, false, false);
            _character.RebuildModifiers(false);
            _character.talentsAvailable--;
            DisplayTalents();
        }
    }
}
#endif
