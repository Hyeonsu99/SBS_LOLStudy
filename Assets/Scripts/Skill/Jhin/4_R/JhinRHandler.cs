using System.Collections;
using UnityEngine;

public class JhinRHandler : MonoBehaviour
{
    private JhinRData _data;
    private SkillHandler _skillHandler;
    private PlayerController _playerController;

    public bool IsActive { get; private set; } = false;
    private int _currentShots = 0;
    private float _lastFireTime;
    private Vector3 _aimCenterDirection; // 부채꼴의 중심 방향
    private int _skillLevel;

    public void SetUp(JhinRData data, SkillHandler skillHandler, PlayerController controller)
    {
        _data = data;
        _skillHandler = skillHandler;
        _playerController = controller;
    }

    public void StartUltimate(Vector3 targetPos, int level)
    {
        if (IsActive) return;

        IsActive = true;
        _currentShots = 4;
        _skillLevel = level;
        _lastFireTime = Time.time; // 시전 직후 딜레이

        // 방향 설정
        _aimCenterDirection = (targetPos - transform.position).normalized;
        _aimCenterDirection.y = 0;

        // [카메라] 줌 아웃 + 오프셋 이동
        Vector3 camTargetPos = transform.position + (_aimCenterDirection * _data.CameraOffsetDistance);
        //_playerController.SetCameraOverride(true, camTargetPos, _data.CameraZoomSize);

        StartCoroutine(DurationCoroutine());

        Debug.Log("커튼 콜 시작!");
    }

    public void Fire()
    {
        // 연사 딜레이 및 잔탄 체크
        if (Time.time < _lastFireTime + _data.FireDelay) return;
        if (_currentShots <= 0) return;

        _lastFireTime = Time.time;
        _currentShots--;

        bool isFourth = (_currentShots == 0);

        // 마우스 방향으로 발사
        Vector3 fireDir = GetMouseDirection();

        // 투사체 생성
        GameObject go = Instantiate(_data.ProjectilePrefab, transform.position + Vector3.up, Quaternion.LookRotation(fireDir));
        if (go.TryGetComponent(out JhinRProjectile proj))
        {
            int idx = Mathf.Clamp(_skillLevel - 1, 0, _data.BaseDamages.Length - 1);

            // 데이터 주입
            proj.Initialize(
                owner: gameObject,
                dir: fireDir,
                speed: 40f, // 투사체 속도 (데이터화 가능)
                maxDist: _data.Range,
                baseDmg: _data.BaseDamages[idx],
                adRatio: _data.AdRatio,
                maxMult: _data.MaxDamageMultiplier, // 4.0
                slowPer: _data.SlowPercentage,
                slowDur: _data.SlowDuration,
                isFourth: isFourth
            );
        }

        // 4발 다 쏘면 종료
        if (_currentShots <= 0)
        {
            EndUltimate();
        }
    }

    public void Cancel()
    {
        if (!IsActive) return;
        Debug.Log("커튼 콜 취소");
        EndUltimate();
    }

    private void EndUltimate()
    {
        IsActive = false;
        StopAllCoroutines();

        // 카메라 복귀
        //_playerController.SetCameraOverride(false, Vector3.zero, 0);

        // 쿨타임 시작 (스킬 사용이 완전히 끝난 시점부터 쿨타임)
        if (_skillHandler.Slot_R != null)
        {
            // SkillSlot에 StartCooldown 메서드가 있다면 호출
            // _skillHandler.Slot_R.StartCooldown(); 
        }
    }

    private IEnumerator DurationCoroutine()
    {
        yield return new WaitForSeconds(_data.MaxDuration);
        Cancel(); // 시간 초과
    }

    private Vector3 GetMouseDirection()
    {
        // 마우스 위치로 발사하되, 각도 제한 로직이 필요하다면 여기서 _aimCenterDirection과 비교
        Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            Vector3 dir = (hit.point - transform.position).normalized;
            dir.y = 0;
            return dir;
        }
        return transform.forward;
    }
}
