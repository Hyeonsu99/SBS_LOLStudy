using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public string EffectID { get; protected set; }
    public string EffectType { get; protected set; }
    public float Duration;
    public float RemainTime { get; protected set; }
    public bool IsExpired => RemainTime <= 0f;

    protected PlayerStat targetStat;
    protected StatModifier modifier;

    public virtual void Initialize(PlayerStat stat, float duration)
    {
        targetStat = stat;
        Duration = duration;
        RemainTime = duration;

        EffectID = $"{EffectType}_{Time.time}_{Random.Range(1000, 9999)}";

        Apply();
    }

    // 효과 적용 함수
    protected abstract void Apply();

    protected virtual void Update()
    {
        RemainTime -= Time.deltaTime;

        if(IsExpired)
        {
            Destroy(this);
        }
    }

    // 효과 제거 함수
    protected abstract void Remove();

    public virtual void Refresh(float duration)
    {
        RemainTime = duration;

        Debug.Log($"{EffectID} 효과 갱신!");
    }
}
