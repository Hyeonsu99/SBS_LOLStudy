using UnityEngine;

public class JhinMarkDebuff : Effect
{
    private void Awake()
    {
        Type = EffectType.JhinMark;
    }

    protected override void Apply()
    {
        Debug.Log("표식 생성");
    }

    protected override void Remove()
    {
        Debug.Log("표식 사라짐");
    }
}
