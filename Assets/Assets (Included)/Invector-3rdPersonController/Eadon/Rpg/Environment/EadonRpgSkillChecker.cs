#if EADON_RPG_INVECTOR
using System;
using System.Collections;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.Configuration;
using Eadon.Rpg.Invector.Utils;
using Invector.vCharacterController.vActions;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Eadon.Rpg.Invector.Environment
{
    public enum SkillCheckType
    {
        MustHaveSkill,
        MustHaveSkillAtValue,
        DifficultyBasedRoll
    }

    public class EadonRpgSkillChecker : MonoBehaviour
    {
        [Header("Skill Check Type")]
        public SkillCheckType skillCheckType;
        public EadonRpgSkill skill;
        public int neededValue = 1;
        public int difficultyValue = 1;

        [Header("Configuration")]
        public bool waitForAnimation;

        [Header("Events")]
        public UnityEvent success;
        public UnityEvent failure;

        private vTriggerGenericAction _triggerAction;
        private Coroutine _lastCoroutine;

        private void Awake()
        {
            _triggerAction = GetComponent<vTriggerGenericAction>();
        }

        private void OnEnable()
        {
            _triggerAction.onPressActionInputWithTarget.AddListener(GenericActionHandler);
            _triggerAction.OnCancelActionInput.AddListener(CancelCoroutine);
        }

        private void OnDisable()
        {
            _triggerAction.onPressActionInputWithTarget.RemoveListener(GenericActionHandler);
            _triggerAction.OnCancelActionInput.RemoveListener(CancelCoroutine);
        }

        private void CancelCoroutine()
        {
            StopCoroutine(_lastCoroutine);
        }

        private void GenericActionHandler(GameObject obj)
        {
            _lastCoroutine = StartCoroutine(BeginCheck(obj));
        }

        private IEnumerator BeginCheck(GameObject obj)
        {
            var character = obj.GetComponent<EadonRpgCharacter>();
            if (character == null) yield break;

            // Define and test for inputted Stat
            EadonBaseStatSkillValue skillValue = null;
            if(character.skills.ContainsKey(skill.SkillName))
            {
                skillValue = character.skills[skill.SkillName];
            }

            var genericAction = obj.GetComponent<vGenericAction>();

            if (_triggerAction.inputType == vTriggerGenericAction.InputType.GetButtonTimer)
            {
                while (genericAction.doingAction)
                {
                    yield return null;
                }
            }
            else
            {
                if (waitForAnimation && !string.IsNullOrEmpty(_triggerAction.playAnimation))
                {
                    while (!genericAction.playingAnimation)
                    {
                        yield return null;
                    }

                    while (genericAction.playingAnimation)
                    {
                        yield return null;
                    }
                }
            }

            TestSkill(skillCheckType, character, skillValue, neededValue, difficultyValue);

            yield return null;
        }

        private void TestSkill(SkillCheckType checkType, EadonRpgCharacter character, EadonBaseStatSkillValue skillValue, int needed, int difficulty)
        {
            var result = false;

            if(skillValue == null)
            {
                Debug.Log("Failure - character does not have skill");
            }
            else
            {
                switch (checkType)
                {
                    case SkillCheckType.MustHaveSkill:
                        if (skillValue.value > 0.01)
                        {
                            result = true;
                        }
                        break;
                    case SkillCheckType.MustHaveSkillAtValue:
                        if (skillValue.value >= needed)
                        {
                            result = true;
                        }
                        break;
                    case SkillCheckType.DifficultyBasedRoll:
                        var roll = Random.Range(1, 20);
                        if (skillValue.value + roll >= needed)
                        {
                            result = true;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(checkType), checkType, null);
                }
            }

            if (result)
            {
                Debug.Log("Success");
                success.Invoke();
            }
            else
            {
                Debug.Log("Failure");
                failure.Invoke();
            }
        }
    }
}

#endif
