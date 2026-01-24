using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AirborneDebuff : Effect
{
    private float _height = 2.0f; // 얼마나 높이 띄울지 (기본 2미터)
    private Vector3 _startPos;
    private NavMeshAgent _agent;
    private bool _wasAgentEnabled;

    public void Initialize(UnitStat target, float duration, float height, EffectType type)
    {
        _height = height;
        base.Initialize(target, duration, type);
    }

    protected override void Apply()
    {
        // 1. 시작 위치 저장
        _startPos = transform.position;

        // 2. NavMeshAgent 간섭 방지 (바닥으로 끌어당김 방지)
        _agent = GetComponent<NavMeshAgent>();
        if (_agent != null)
        {
            _wasAgentEnabled = _agent.enabled;
            // 아예 끄면 다른 유닛이 밀고 들어올 수 있으므로, updatePosition만 끄거나
            // 간단하게는 Agent를 잠시 비활성화 합니다. (여기선 비활성화 방식 사용)
            _agent.enabled = false;
        }

        // 3. 움직임 제어 (기절과 동일 효과)
        // 별도의 Modifier 없이 UnitStat.HasEffect(Airborne)으로 체크하여 행동 불가 처리

        // 4. 점프 코루틴 시작
        StartCoroutine(JumpRoutine());

        Debug.Log($"{EffectID} 에어본 적용! 높이: {_height}");
    }

    protected override void Remove()
    {
        // 1. 강제로 바닥 위치로 원상복구 (혹시 코루틴이 중간에 끊겼을 경우 대비)
        // Y축만 원래대로 돌리고 X,Z는 유지 (밀려났을 수도 있으니)
        transform.position = new Vector3(transform.position.x, _startPos.y, transform.position.z);

        // 2. NavMeshAgent 복구
        if (_agent != null && _wasAgentEnabled)
        {
            _agent.enabled = true;
            // 복구 후 위치 동기화
            _agent.Warp(transform.position);
        }
    }

    private IEnumerator JumpRoutine()
    {
        float elapsed = 0f;

        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;

            // 0 ~ 1 사이의 진행도
            float t = elapsed / Duration;

            // [포물선 공식] Y = 4 * Height * t * (1 - t)
            // t가 0일 때 0, 0.5일 때 최대 높이(1), 1일 때 0이 됨
            float yOffset = 4 * _height * t * (1 - t);

            // 현재 X, Z 위치는 유지하되 Y만 변경
            // (만약 넉백을 구현하고 싶다면 X, Z도 여기서 Lerp로 이동시키면 됩니다)
            transform.position = new Vector3(transform.position.x, _startPos.y + yOffset, transform.position.z);

            yield return null;
        }

        // 끝나면 Remove()가 호출되면서 바닥으로 착지
    }
}
