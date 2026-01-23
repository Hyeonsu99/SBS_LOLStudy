using Sirenix.OdinInspector;
using UnityEngine;

public class EXPHandler : MonoBehaviour
{
    private UnitStat _stat;
    private SkillHandler _skillHandler;

    public int CurrentLevel = 1;
    public float CurrentExp = 0;

    public int SkillPoint = 0;

    public float _expTable;

    private void Awake()
    {
        _stat = GetComponent<UnitStat>();
        _skillHandler = GetComponent<SkillHandler>();

        _expTable = _expTable + 180 + (100 * CurrentLevel);
    }

    public void AddExp(float amount)
    {
        if (CurrentLevel >= 18) return;

        CurrentExp += amount;

        while(CurrentLevel < 18 && CurrentExp >= _expTable)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        CurrentLevel++;
        SkillPoint++;

        _expTable = _expTable + 180 + (100 * CurrentLevel);

        _stat.UpdateLevel(CurrentLevel);
    }

    public void UseSkillPoint() => SkillPoint--;

    [Button(ButtonSizes.Medium)]
    public void TestExpADD()
    {
        AddExp(50);
    }
}
