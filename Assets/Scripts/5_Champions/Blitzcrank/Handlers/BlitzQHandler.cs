using UnityEngine;

public class BlitzQHandler : MonoBehaviour
{
    private UnitStat _ownerStat;
    private BlitzQData _data;

    // 자신에게 건 속박 효과의 ID를 저장 (나중에 풀기 위해)
    private string _selfRootEffectID;

    public void SetUp(UnitStat stat, BlitzQData data)
    {
        _ownerStat = stat;
        _data = data;
    }

    public void Fire(Vector3 targetPos, float baseDamage, int level)
    {
        // 1. 방향 계산 (Y축 고정)
        Vector3 dir = (targetPos - transform.position).normalized;
        dir.y = 0;

        // 2. 투사체 생성 (몸체 중심 + 약간 위)
        Vector3 spawnPos = transform.position + Vector3.up;
        GameObject go = Instantiate(_data.ProjectilePrefab, spawnPos, Quaternion.LookRotation(dir));

        if (go.TryGetComponent(out BlitzQProjectile proj))
        {
            // 투사체 초기화 (돌아오면 OnFistReturned 함수 실행하도록 콜백 전달)
            proj.Initialize(gameObject, dir, _data, baseDamage, level, OnFistReturned);
        }

        // 3. [핵심] 투사체가 날아갔다 돌아올 때까지 자신 속박 (Self-Root)
        // 지속시간은 넉넉하게 3초로 잡고, 콜백으로 수동 해제함
        _selfRootEffectID = _ownerStat.ApplyEffect(EffectType.Root, ModType.Flat, 3.0f, 1);

        // 시전 방향 바라보기
        transform.rotation = Quaternion.LookRotation(dir);
    }

    // 투사체가 돌아왔거나 파괴되었을 때 호출됨
    private void OnFistReturned()
    {
        if (!string.IsNullOrEmpty(_selfRootEffectID))
        {
            _ownerStat.RemoveEffect(_selfRootEffectID);
            _selfRootEffectID = null;
        }
    }

    private void OnDestroy()
    {
        // 핸들러 파괴 시에도 속박이 남아있지 않도록 안전장치
        OnFistReturned();
    }
}
