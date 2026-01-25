using UnityEngine;

public class JhinWeaponHandler : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab; // JhinProjectile 프리팹
    [SerializeField] private Transform _firePoint;         // 총구 위치
    [SerializeField] private float _baseSpeed = 15f;       // 기본 투사체 속도

    private CombatHandler _combatHandler;
    private JhinPassiveHandler _passiveHandler;
    private UnitStat _stat;

    private void Awake()
    {
        _stat = GetComponent<UnitStat>();
        _passiveHandler = GetComponent<JhinPassiveHandler>();
        _combatHandler = GetComponent<CombatHandler>();
    }

    private void OnEnable()
    {
        // CombatHandler의 공격 이벤트 구독
        if (_combatHandler != null)
        {
            _combatHandler.OnAttackPerformed += FireProjectile;
        }
    }

    private void OnDisable()
    {
        if (_combatHandler != null)
        {
            _combatHandler.OnAttackPerformed -= FireProjectile;
        }
    }

    // 공격 신호가 오면 실행되는 함수
    private void FireProjectile()
    {
        // 1. 타겟 및 데이터 검증
        if (_combatHandler.CurrentTarget == null) return;
        if (_projectilePrefab == null) return;

        UnitStat targetStat = _combatHandler.CurrentTarget.GetComponent<UnitStat>();
        if (targetStat == null) return;

        if (_passiveHandler == null)
        {
            _passiveHandler = GetComponent<JhinPassiveHandler>();
        }

        bool isFourthShot = _passiveHandler != null && _passiveHandler.CurrentAmmo == 1;

        Debug.Log(_passiveHandler.CurrentAmmo);

        float currentSpeed = isFourthShot ? _baseSpeed * 1.5f : _baseSpeed;
        float damage = _stat.Current.Get(StatType.AttackDamage);

        Vector3 spawnPos = _firePoint != null ? _firePoint.position : transform.position;
        GameObject go = Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);

        if (go.TryGetComponent(out JhinProjectile proj))
        {
            proj.Initialize(gameObject, _combatHandler.CurrentTarget, currentSpeed, isFourthShot);
        }

        if (isFourthShot)
        {
            Debug.Log("진 4타 발사! (치명타 + 고속 투사체)");
        }
    }
}
