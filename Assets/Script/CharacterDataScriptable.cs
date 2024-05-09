using UnityEngine;
using UnityEngine.Serialization;


public enum SkillType
{
    Teammate,
    Competitor
}

public enum SkinType
{
    Normal,
    Special
}

public enum InfluenceRange
{
    Single,
    Multiple,
}

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    public int Id;
    public SkillType SkillType;
    public InfluenceRange Range;
    public int Power;
    public int Health;
    public int Defense;
    public int Energy;
    public Sprite Sprite;
    //public string NameSkill;
    public string Description;
    public string NameCharacter;
}
