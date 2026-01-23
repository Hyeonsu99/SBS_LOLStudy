using UnityEngine;

public class RegenHandler : MonoBehaviour
{
    private UnitStat _stat;
    private float _timer;
    private const float _regenInterval = 0.5f;

    private void Awake()
    {
        _stat = GetComponent<UnitStat>();
    }
    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(_timer > _regenInterval)
        {
            _timer = 0f;
            ApplyRegen();
        }
    }

    private void ApplyRegen()
    {
        float hpRegenAmount = _stat.Current.Get(StatType.HpRegen) / 10f;
        float mpRegenAmount = _stat.Current.Get(StatType.MpRegen) / 10f;

        if(hpRegenAmount > 0) _stat.RestoreHP(hpRegenAmount);
        if(mpRegenAmount > 0) _stat.RestoreMP(mpRegenAmount);
    }
}
