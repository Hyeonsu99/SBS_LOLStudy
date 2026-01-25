using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ChampionSelectManager SelectManager;

    public StatObserverUI StatPanel;
    public SkillObserverUI SkillPanel;

    private void Start()
    {
        if (SelectManager != null)
        {
            // 챔피언이 스폰될 때마다 OnChampionChanged 실행
            SelectManager.OnChampionSpawned += OnChampionChanged;
        }
    }

    private void OnDestroy()
    {
        if (SelectManager != null)
            SelectManager.OnChampionSpawned -= OnChampionChanged;
    }

    private void OnChampionChanged(GameObject newChampion)
    {
        // 1. 스탯 UI에 새 유닛 연결
        if (newChampion.TryGetComponent(out UnitStat stat))
        {
            StatPanel.Bind(stat);
        }

        // 2. 스킬 UI에 새 유닛 연결
        SkillPanel.Bind(newChampion);

        Debug.Log($"UI 갱신 완료: {newChampion.name}");
    }
}
