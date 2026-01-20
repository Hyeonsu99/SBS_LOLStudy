using UnityEngine;

public class RootDebuff : Effect
{
    private void Awake()
    {
        Type = EffectType.Root;
    }

    protected override void Apply()
    {
        // 이동 불가 상태로 만들기
    }

    protected override void Remove()
    {
        // 이동 불가 상태 해제
    }
}
