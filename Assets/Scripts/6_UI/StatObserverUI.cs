using TMPro;
using UnityEngine;

public class StatObserverUI : MonoBehaviour
{
    [Header("Text Component")]
    public TextMeshProUGUI Text_AD;
    public TextMeshProUGUI Text_Armor;
    public TextMeshProUGUI Text_AS;
    public TextMeshProUGUI Text_CritAmount;
    public TextMeshProUGUI Text_AP;
    public TextMeshProUGUI Text_Resist;
    public TextMeshProUGUI Text_AH; //
    public TextMeshProUGUI Text_MS;

    private UnitStat _targetStat;

    public void Bind(UnitStat newStat)
    {
        // 1. 기존 구독 해제 (이전 챔피언)
        if (_targetStat != null)
        {
            _targetStat.OnStatChanged -= UpdateUI;
        }

        // 2. 새 타겟 설정
        _targetStat = newStat;

        // 3. 구독 신청 및 초기화
        if (_targetStat != null)
        {
            _targetStat.OnStatChanged += UpdateUI;
            _targetStat.RefreshAllStats(); // 초기값 즉시 갱신
        }
    }

    private void UpdateUI(StatType type, float value)
    {
        switch (type)
        {
            case StatType.AttackDamage:
                if (Text_AD) Text_AD.text = $"{value:F0}";
                break;
            case StatType.Armor:
                if (Text_Armor) Text_Armor.text = $"{value:F0}";
                break;
            case StatType.AttackSpeed:
                if (Text_AS) Text_AS.text = $"{value:F2}";
                break;
            case StatType.CriticalAmount:
                if (Text_CritAmount) Text_CritAmount.text = $"{value:F0}";
                break;
            case StatType.AbilityPower:
                if (Text_AP) Text_AP.text = $"{value:F0}"; 
                break;
            case StatType.MagicResist:
                if (Text_Resist) Text_Resist.text = $"{value:F0}";
                break;
            case StatType.AbilityHaste:
                if (Text_AH) Text_AH.text = $"{value:F0}";
                break;
            case StatType.MoveSpeed:
                if (Text_MS) Text_MS.text = $"{value:F0}";
                break;
        }
    }
}
