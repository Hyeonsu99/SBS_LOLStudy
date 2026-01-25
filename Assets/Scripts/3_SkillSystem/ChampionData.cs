using UnityEngine;

[CreateAssetMenu(menuName = "Data/Champion Data")]
public class ChampionData : ScriptableObject
{
    // 챔피언 이름이라던가.... 설명이라던가...
    public string Name;
    public GameObject Prefab;

    public Sprite Portrait; // 챔피언 초상화 (UI 표시용)

    [Header("Skill Loadout")]
    public SkillData Passive;
    public SkillData Q;
    public SkillData W;
    public SkillData E;
    public SkillData R;
}
