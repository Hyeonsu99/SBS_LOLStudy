using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public string EffectID { get; protected set; }
    public EffectType Type { get; protected set; }

    public float Duration;
    public float RemainTime { get; protected set; }
    public bool IsExpired => RemainTime <= 0f;

    protected UnitStat targetStat;
    protected StatModifier modifier;

    public virtual void Initialize(UnitStat stat, float duration, EffectType type, string customID = null)
    {
        targetStat = stat;
        Duration = duration;
        RemainTime = duration;
        Type = type;

        EffectID = string.IsNullOrEmpty(customID) ? $"{Type}_{Time.time}_{Random.Range(1000, 9999)}" : customID;

        Apply();
    }

    // 효과 적용 함수
    protected abstract void Apply();

    protected virtual void Update()
    {
        RemainTime -= Time.deltaTime;

        if(IsExpired)
        {
            Remove();
            Destroy(this);
        }
    }

    protected virtual void OnDestroy()
    {
        Remove();
    }

    // 효과 제거 함수
    protected abstract void Remove();

    public virtual void Refresh(float duration)
    {
        RemainTime = duration;

        Debug.Log($"{EffectID} 효과 갱신!");
    }

    public StatModifier GetModifier() => modifier;
}
