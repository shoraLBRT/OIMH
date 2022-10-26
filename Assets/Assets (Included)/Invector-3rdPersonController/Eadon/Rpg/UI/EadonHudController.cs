#if EADON_RPG_INVECTOR
using System.Collections;
using System.Collections.Generic;
using Eadon.Rpg.Invector.Character;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.UI;

namespace Eadon.Rpg.Invector.UI
{
    public class EadonHudController : MonoBehaviour
    {
        public Slider manaSlider;
        public Image levelUpImage;
        public Text levelText;
        public Text raceText;
        public Text classText;
        public Text alignmentText;
        public Text characterName;
        public Text xp;
        public Text xpToNext;
        public GameObject characterLevelUpVfx;

        private GameObject _currentLevelUpVfx;

        private static EadonHudController _instance;
        protected EadonRpgCharacter Character;
        
        public static EadonHudController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<EadonHudController>();
                }
                return _instance;
            }
        }

        public virtual void ShowLevelUpImage()
        {
            if (levelUpImage != null)
            {
                levelUpImage.enabled = true;
                levelUpImage.gameObject.SetActive(true);
                vHUDController.instance.ShowText("You levelled up!");
            }
            
            if (Character == null)
            {
                return;
            }

            if (_currentLevelUpVfx != null)
            {
                Destroy(_currentLevelUpVfx);
            }
            if (characterLevelUpVfx != null)
            {
                _currentLevelUpVfx = Instantiate(characterLevelUpVfx, Character.transform);
                _currentLevelUpVfx.transform.localPosition = Vector3.zero;
            }
        }

        public virtual void HideLevelUpImage()
        {
            if (levelUpImage != null)
            {
                levelUpImage.enabled = true;
                levelUpImage.gameObject.SetActive(false);
            }
            if (_currentLevelUpVfx != null)
            {
                Destroy(_currentLevelUpVfx);
            }
        }

        public void UpdateManaSlider(EadonRpgCharacter character)
        {
            Character = character;
            if (manaSlider != null)
            {
                manaSlider.minValue = 0;
                
                if (character.maxMana != manaSlider.maxValue)
                {
                    manaSlider.maxValue = Mathf.Lerp(manaSlider.maxValue, character.maxMana, 2f * Time.fixedDeltaTime);
                    manaSlider.onValueChanged.Invoke(manaSlider.value);
                }
                manaSlider.value = Mathf.Lerp(manaSlider.value, character.currentMana, 2f * Time.fixedDeltaTime);
            }
        }

        public virtual void UpdateCharacterInfo(EadonRpgCharacter character)
        {
            Character = character;
            if (levelText != null)
            {
                levelText.text = "" + character.currentLevel;
            }
            if (raceText != null)
            {
                raceText.text = "" + character.currentRace;
            }
            if (levelText != null)
            {
                classText.text = "" + character.currentClass;
            }
            if (levelText != null)
            {
                alignmentText.text = "" + character.currentAlignment;
            }
            if (levelText != null)
            {
                characterName.text = "" + character.characterName;
            }
            if (xp != null)
            {
                xp.text = "" + character.currentXp;
            }
            if (xpToNext != null)
            {
                xpToNext.text = "" + character.xpToNextLevel;
            }
        }
    }
}

#endif
