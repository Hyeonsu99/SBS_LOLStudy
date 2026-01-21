using UnityEngine;
using UnityEngine.AI;

public class RootDebuff : Effect
{
    private void Awake()
    {
        Type = EffectType.Root;
    }

    protected override void Apply()
    {
        // 이동 불가 상태로 만들기
        if(TryGetComponent(out NavMeshAgent agent))
        {
            if(agent.enabled)
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
            }
        }
    }

    protected override void Remove()
    {
        // 이동 불가 상태 해제
    }
}
