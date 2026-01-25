using UnityEngine;

public class JhinProjectile : MonoBehaviour
{
    private GameObject _owner;
    private CombatHandler _ownerCombat;
    private GameObject _target;

    private float _speed;
    private bool _isCrit;

    public void Initialize(GameObject owner, GameObject target, float speed, bool isCrit)
    {
        _owner = owner;
        _ownerCombat = owner.GetComponent<CombatHandler>();
        _target = target;
        _speed = speed;
        _isCrit = isCrit;

        Destroy(gameObject, 5f);
    }
    private void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = _target.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        transform.LookAt(targetPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (_owner.TryGetComponent(out JhinPassiveHandler passive))
        {
            passive.ProcessFourthShot = _isCrit; // 플래그 설정
            _ownerCombat.ApplyDamage(_target, _isCrit); // 데미지 계산 (DecorateDamage 호출됨)
            passive.ProcessFourthShot = false; // 플래그 초기화
        }
        else
        {
            _ownerCombat.ApplyDamage(_target, _isCrit);
        }

        Destroy(gameObject);
    }
}
