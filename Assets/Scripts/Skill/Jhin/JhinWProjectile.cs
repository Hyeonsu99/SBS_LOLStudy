using UnityEngine;

public class JhinWProjectile : MonoBehaviour
{
    private GameObject _owner;
    private UnitStat _ownerStat;

    private Vector3 _direction;
    private float _baseDamage;
    private float _adRatio;
    private float _rootDuration;
    private float _maxDistance;
    private float _minionDamageRatio;

    private float _distanceTraveled;
    private float _speed;

    public void Initialize(GameObject owner, Vector3 dir, float speed,  float baseDmg, float adRatio, float rootDur, float maxDist, float minionRatio)
    {
        _owner = owner;
        _ownerStat = owner.GetComponent<UnitStat>();
        _direction = dir;
        _speed = speed;
        _baseDamage = baseDmg;
        _adRatio = adRatio;
        _rootDuration = rootDur;
        _maxDistance = maxDist;
        _minionDamageRatio = minionRatio;

        // 안전장치 (3초 후 삭제)
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        float moveAmount = _speed * Time.deltaTime;
        transform.position += _direction * moveAmount;

        _distanceTraveled += moveAmount;

        if(_distanceTraveled >= _maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _owner) return;

        if(other.TryGetComponent(out UnitStat targetStat))
        {
            float currentAd = _ownerStat.Current.Get(StatType.AttackDamage);
            float finalDamage = _baseDamage + (currentAd * _adRatio);

            // 챔피언 미니언 구분
            bool isChampion = other.CompareTag("Champion");
            bool isMinion = other.CompareTag("Minion");

            if(isMinion)
            {
                finalDamage = finalDamage * _minionDamageRatio;
            }

            var info = new DamageInfo
            {
                Attacker = _owner,
                Target = other.gameObject,
                RawDamage = finalDamage,
                BonusDamage = 0,
                isCritical = false
            };

            float calDamage = DamageCalculater.CalculateFinalDamage(_ownerStat, targetStat, info);

            targetStat.TakeDamage(finalDamage, _owner);

            if (isChampion) 
            {
                if(targetStat.HasEffect(EffectType.JhinMark))
                {

                    targetStat.RemoveEffect(EffectType.JhinMark.ToString());

                    targetStat.ApplyEffect(EffectType.Root, ModType.Flat, _rootDuration, 0);

                    _owner.GetComponent<JhinPassiveHandler>()?.OnCriticalHit();
                }

                Destroy(gameObject);
            }
        }
    }
}
