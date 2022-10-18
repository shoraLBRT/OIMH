#if EADON_RPG_INVECTOR
using System;
using UnityEngine;
using UnityEngine.UI;

public enum StatDisplayType
{
    Stat,
    Skill,
    Resistance
}
    
public class StatSkillDisplayPanel : MonoBehaviour
{
    public Text statName;
    public Text statValue;
    public Button statIncreaseButton;
    public StatDisplayType type;
    public string value;
    public EadonCharacterDisplayManager displayManager;
        
    public void IncreaseStatSkill()
    {
        switch (type)
        {
            case StatDisplayType.Stat:
                displayManager.IncreaseStat(value);
                break;
            case StatDisplayType.Skill:
                displayManager.IncreaseSkill(value);
                break;
            case StatDisplayType.Resistance:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

#endif
