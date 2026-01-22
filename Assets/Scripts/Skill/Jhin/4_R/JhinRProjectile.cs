using UnityEngine;

public class JhinRProjectile : MonoBehaviour
{
    private GameObject _owner;
    private UnitStat _ownerStat;
    private UnitIdentity _ownerIdentity;

    // 데미지 관련 변수
    private float _baseDamage;      // 64 / 128 / 192
    private float _adRatio;         // 0.25
    private float _maxMultiplier;   // 4.0 (최대 4배)

    private float _slowPercent;
    private float _slowDuration;
    private bool _isFourthShot;     // 4타 여부

    private Vector3 _direction;
    private float _speed;
    private float _maxDistance;
    private float _traveled;

    public void Initialize(GameObject owner, Vector3 dir, float speed, float maxDist,
                           float baseDmg, float adRatio, float maxMult,
                           float slowPer, float slowDur, bool isFourth)
    {
        _owner = owner;
        _ownerStat = owner.GetComponent<UnitStat>();
        _ownerIdentity = owner.GetComponent<UnitIdentity>();

        _direction = dir;
        _speed = speed;
        _maxDistance = maxDist;

        _baseDamage = baseDmg;
        _adRatio = adRatio;
        _maxMultiplier = maxMult;

        _slowPercent = slowPer;
        _slowDuration = slowDur;
        _isFourthShot = isFourth;

        Destroy(gameObject, 4f); // 안전장치
    }

    void Update()
    {
        float move = _speed * Time.deltaTime;
        transform.position += _direction * move;
        _traveled += move;

        if (_traveled >= _maxDistance) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner) return;

        // 투사체 로직: 챔피언에 닿으면 멈추고 데미지
        if (other.TryGetComponent(out UnitIdentity targetId) && other.TryGetComponent(out UnitStat targetStat))
        {
            if (!_ownerIdentity.IsEnemy(targetId)) return;

            ApplyEffects(targetStat);
            Destroy(gameObject);
        }
    }

    private void ApplyEffects(UnitStat target)
    {
        // 1. 둔화 적용
        target.ApplyEffect(EffectType.SlowDebuff, ModType.PercentAdd, _slowDuration, _slowPercent);

        // 2. [핵심] 데미지 계산 로직

        // A. 기본 데미지 산출 (깡뎀 + AD계수)
        float currentAd = _ownerStat.Current.Get(StatType.AttackDamage);
        float rawDamage = _baseDamage + (currentAd * _adRatio);

        // B. 잃은 체력 비례 증폭 (1배 ~ 4배)
        float currentHp = target.CurrentHP;
        float maxHp = target.Current.Get(StatType.Hp);

        // 잃은 체력 비율 (0.0 = 풀피, 1.0 = 딸피)
        float missingPercent = 1f - (currentHp / maxHp);
        missingPercent = Mathf.Clamp01(missingPercent);

        // Lerp를 사용하여 1배에서 4배 사이값 계산
        // 예: 체력 100% -> multiplier 1
        // 예: 체력 50%  -> multiplier 2.5
        // 예: 체력 0%   -> multiplier 4
        float multiplier = Mathf.Lerp(1.0f, _maxMultiplier, missingPercent);

        float finalRawDamage = rawDamage * multiplier;

        // C. 4타 확정 치명타 (2배)
        if (_isFourthShot)
        {
            finalRawDamage *= 2.0f;
        }

        // 데미지 정보 생성
        DamageInfo info = new DamageInfo
        {
            Attacker = _owner,
            Target = target.gameObject,
            RawDamage = finalRawDamage,
            Type = DamageType.Physical, // 물리 피해
            isCritical = _isFourthShot  // 4타는 치명타 판정
        };

        // 방어력 등 최종 연산 후 적용
        float calcDamage = DamageCalculater.CalculateFinalDamage(_ownerStat, target, info);
        target.TakeDamage(calcDamage, _owner);
    }
}
