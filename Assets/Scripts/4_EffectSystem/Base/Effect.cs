using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public string EffectID { get; protected set; }
    public EffectType Type { get; protected set; }

    public float Duration;
    public float RemainTime { get; protected set; }
    public bool IsExpired => RemainTime <= 0f;

    protected UnitStat _targetStat;
    protected StatModifier _modifier;

    public virtual void Initialize(UnitStat stat, float duration, EffectType type, string customID = null)
    {
        _targetStat = stat;
        Duration = duration;
        RemainTime = duration;
        Type = type;

        EffectID = string.IsNullOrEmpty(customID) ? $"{Type}_{Time.time}_{Random.Range(1000, 9999)}" : customID;

        Apply();
    }



    protected virtual void Update()
    {
        RemainTime -= Time.deltaTime;

        if(IsExpired)
        {
            Remove();
            Destroy(this);
        }
    }

    public void Terminate()
    {
        // 타겟 스탯과 Modifier 정보가 있다면 즉시 리스트에서 제거 요청
        if (_targetStat != null && !string.IsNullOrEmpty(_modifier.ID))
        {
            _targetStat.RemoveModifier(_modifier);
        }
    }



    // 효과 적용 함수
    protected abstract void Apply();
    // 효과 제거 함수
    protected abstract void Remove();

    public virtual void Refresh(float duration)
    {
        RemainTime = duration;

        Debug.Log($"{EffectID} 효과 갱신!");
    }
    protected virtual void OnDestroy()
    {
        Remove();
    }

    public StatModifier GetModifier() => _modifier;
}
