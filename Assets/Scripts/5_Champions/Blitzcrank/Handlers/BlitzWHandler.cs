using System.Collections;
using UnityEngine;

public class BlitzWHandler : MonoBehaviour
{
    UnitStat _ownerStat;
    BlitzWData _data;

    private Coroutine _runningCoroutine;

    public void SetUp(UnitStat stat, BlitzWData data)
    {
        _ownerStat = stat;
        _data = data;
    }

    public void Activate(float asAmount, float msAmount)
    {
        if(_runningCoroutine != null )
        {
            StopCoroutine(_runningCoroutine);
        }

        _runningCoroutine = StartCoroutine(OverdriveCoroutine(asAmount, msAmount));
    }

    private IEnumerator OverdriveCoroutine(float asAmount, float msAmount)
    {
        string asBuffId = _ownerStat.ApplyEffect(EffectType.AttackSpeedBuff, ModType.PercentAdd, _data.Duration, asAmount);
        string msBuffId = _ownerStat.ApplyEffect(EffectType.SpeedBuff, ModType.PercentMul, _data.Duration, msAmount);
        
        yield return new WaitForSeconds(_data.Duration);

        _ownerStat.RemoveEffect(asBuffId);
        _ownerStat.RemoveEffect(msBuffId);

        yield return new WaitForSeconds(0.1f);

        _ownerStat.ApplyEffect(EffectType.SlowDebuff, ModType.PercentMul, _data.EndSlowDuration, _data.EndSlowPercent);

        _runningCoroutine = null;
    }
}
