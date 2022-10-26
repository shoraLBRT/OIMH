#if EADON_RPG_INVECTOR
using System.Collections.Generic;
using CogsAndGoggles.Library.Extensions;
using Eadon.Rpg.Invector.Character;
using Eadon.Rpg.Invector.UI;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.UI;

public class EadonCharacterDisplayManager : MonoBehaviour
{
    public Text raceText;
    public Text classText;
    public Text alignmentText;
    public Text levelText;
    public Text xpText;
    public Text xpToNextText;
    public Text armourText;
    public Text bonusDamageText;
    public Text weightText;
    public Text statPointsLeftText;
    public Text skillPointsLeftText;
    public GameObject statsPanel;
    public GameObject skillsPanel;
    public GameObject resistancesPanel;
    public GameObject statPanelPrefab;
    private EadonRpgCharacter _character;

    private readonly List<StatSkillDisplayPanel> _statPanels = new List<StatSkillDisplayPanel>();
    private readonly List<StatSkillDisplayPanel> _skillPanels = new List<StatSkillDisplayPanel>();
    private readonly List<StatSkillDisplayPanel> _resistancesPanels = new List<StatSkillDisplayPanel>();

    // Start is called before the first frame update
    private void OnEnable()
    {
        var playerGameObject = GetComponentInParent<vThirdPersonController>();
        if (playerGameObject == null) return;

        _character = playerGameObject.GetComponent<EadonRpgCharacter>();
        if (_character == null) return;
        
        DisplayCharacter();
    }

    private void DisplayCharacter()
    {
        EadonHudController.Instance.HideLevelUpImage();

        raceText.text = _character.currentRace.RaceName;
        classText.text = _character.currentClass.ClassName;
        alignmentText.text = _character.currentAlignment.AlignmentName;

        armourText.text = $"{_character.currentArmour} (current) / {_character.armourBonus} (bonus)";
        bonusDamageText.text = _character.currentDamageBonus.ToString();
        weightText.text = $"{_character.currentEquipmentLoad} / {_character.maxEquipmentLoad}";

        levelText.text = _character.currentLevel.ToString();
        xpText.text = _character.currentXp.ToString();
        xpToNextText.text = _character.xpToNextLevel.ToString();

        statPointsLeftText.text = $"({_character.unspentStatPoints} points left)";
        skillPointsLeftText.text = $"({_character.unspentSkillPoints} points left)";

        _statPanels.Clear();
        statsPanel.transform.Clear();
        foreach (var stat in _character.eadonRpgCharacterConfig.Stats)
        {
            if (_character.stats.ContainsKey(stat.StatName) && stat.StatName.ToLower() != "mana")
            {
                var statBlock = Instantiate(statPanelPrefab, statsPanel.transform, true);
                statBlock.transform.localScale = Vector3.one;
                var statSkillPanel = statBlock.GetComponent<StatSkillDisplayPanel>();
                statSkillPanel.statName.text = stat.StatName;
                statSkillPanel.value = stat.StatName;
                statSkillPanel.type = StatDisplayType.Stat;
                statSkillPanel.displayManager = this;
                var characterStat = _character.stats[stat.StatName];
                var characterStatsModTotal = _character.statsModifierTotals[stat.StatName];
                statSkillPanel.statValue.text =
                    $"{_character.GetTotalStatValue(stat.StatName)/*characterStat.value + characterStatsModTotal.value)*/} ({characterStat.value} + {characterStatsModTotal.value} + {_character.GetStatSkillBuffTotal(stat.StatName)})";
                _statPanels.Add(statSkillPanel);
            }
        }

        _skillPanels.Clear();
        skillsPanel.transform.Clear();
        foreach (var skill in _character.eadonRpgCharacterConfig.Skills)
        {
            if (_character.skills.ContainsKey(skill.SkillName))
            {
                var statBlock = Instantiate(statPanelPrefab, skillsPanel.transform, true);
                var statSkillPanel = statBlock.GetComponent<StatSkillDisplayPanel>();
                statBlock.transform.localScale = Vector3.one;
                statSkillPanel.statName.text = skill.SkillName;
                statSkillPanel.value = skill.SkillName;
                statSkillPanel.type = StatDisplayType.Skill;
                statSkillPanel.displayManager = this;
                var characterSkill = _character.skills[skill.SkillName];
                var characterSkillsModTotal = _character.skillsModifiersTotals[skill.SkillName];
                statSkillPanel.statValue.text =
                    $"{characterSkill.value + characterSkillsModTotal.value} ({characterSkill.value} + {characterSkillsModTotal.value})";
                _skillPanels.Add(statSkillPanel);
            }
        }

        _resistancesPanels.Clear();
        resistancesPanel.transform.Clear();
        foreach (var damageType in _character.eadonRpgCharacterConfig.GetDamageTypeNames())
        {
            if (_character.resistances.ContainsKey(damageType))
            {
                var statBlock = Instantiate(statPanelPrefab, resistancesPanel.transform, true);
                var statSkillPanel = statBlock.GetComponent<StatSkillDisplayPanel>();
                statBlock.transform.localScale = Vector3.one;
                statSkillPanel.statName.text = damageType;
                statSkillPanel.value = damageType;
                statSkillPanel.type = StatDisplayType.Resistance;
                statSkillPanel.displayManager = this;
                statSkillPanel.statIncreaseButton.gameObject.SetActive(false);
                var characterResistance = _character.resistances[damageType];
                var characterResistancesModTotal = _character.resistancesModifiersTotals[damageType];
                statSkillPanel.statValue.text =
                    $"{characterResistance.value + characterResistancesModTotal.value} ({characterResistance.value} + {characterResistancesModTotal.value})";
                _resistancesPanels.Add(statSkillPanel);
            }
        }

        UpdateStatIncreaseButtons();
        UpdateSkillIncreaseButtons();
    }

    private void UpdateStatIncreaseButtons()
    {
        foreach (var statPanel in _statPanels)
        {
            statPanel.statIncreaseButton.gameObject.SetActive(_character.unspentStatPoints > 0);
        }
    }

    private void UpdateSkillIncreaseButtons()
    {
        foreach (var statPanel in _skillPanels)
        {
            statPanel.statIncreaseButton.gameObject.SetActive(_character.unspentSkillPoints > 0);
        }
    }

    public void IncreaseStat(string statName)
    {
        _character.IncreaseStat(statName);
        DisplayCharacter();
    }

    public void IncreaseSkill(string skillName)
    {
        _character.IncreaseSkill(skillName);
        DisplayCharacter();
    }
}
#endif
