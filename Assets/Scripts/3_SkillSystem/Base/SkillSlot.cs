using System.Collections;
using TMPro;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private SkillData _data;
    [SerializeField] private int _level = 0;

    private float _currentCooldown;
    private float _chargeTimer;
    private int _currentCharges;

    private GameObject _owner;
    private UnitStat _ownerStat;
    private SkillHandler _skillHandler;

    private const float RANGE_UNIT_SCALE = 100f;

    //스킬 데이터
    public SkillData Data => _data;
    // 스킬 레벨
    public int Level => _level;
    // 스킬 사용 가능 여부
    public bool IsReady
    {
        get
        {
            if(_level <= 0) return false;
            if(UseChargeSystem) return _currentCharges > 0;
            return _currentCooldown <= 0;
        }
    }
    // 충전형 스킬인지 판단
    public bool UseChargeSystem => _data != null && _data.MaxCharges > 0;
    // 현재 충전된 개수
    public int CurrentCharges => _currentCharges;   
    // 남은 쿨타임 비율
    public float CooldownRatio
    {
        get
        {
            if (_data == null) return 0f;

            if(UseChargeSystem)
            {
                return _currentCharges == _data.MaxCharges ? 0f : _chargeTimer / _data.GetCooldown(_level);
            }
            return _currentCooldown / _data.GetCooldown(_level);
        }
    }
    // 현재 쿨타임
    public float CurrentCooldown => _currentCooldown;

    public TargetType TargetType => _data != null ? _data.Type_Target : TargetType.None;
    public bool IsUnitTargeting => TargetType == TargetType.Unit;

    public void Initialize(SkillData newData, GameObject owner, UnitStat stat)
    {
        _data = newData;
        _owner = owner;
        _ownerStat = stat;
        _skillHandler = owner.GetComponent<SkillHandler>();
        _level = 0;
        _currentCooldown = 0;

        if(UseChargeSystem)
        {
            _currentCharges = _data.MaxCharges;
            _chargeTimer = 0f;
        }

        if(_data != null && _data.Type_Delivery == DeliveryType.Passive)
        {
            LevelUp();
        }
    }

    public float GetRange()
    {
        if (_data == null) return 0f;
        return _data.GetRange(_level) / RANGE_UNIT_SCALE;
    }

    public void LevelUp()
    {
        if(_data == null || _level >= _data.MaxLevel) return;

        bool firstLearn = _level == 0;
        if (!firstLearn) _data.OnUnEquip(_owner);

        _level++;
        _data.OnEquip(_owner, _ownerStat, _level);

        if(firstLearn && UseChargeSystem)
        {
            _currentCharges = _data.MaxCharges;
        }
    }

    public bool CanLevelUp(int playerLevel, int currentSkillPoint)
    {
        if (currentSkillPoint <= 0 || _data == null || _level >= _data.MaxLevel) return false;

        // 스킬 ID 작성할 때 슬롯 이름 넣어주기로 판단
        bool isUltimate = _data.name.Contains("R") || _data.name.Contains("Ult");

        if(isUltimate)
        {
            if (_level == 0 && playerLevel < 6) return false;
            if (_level == 1 && playerLevel < 11) return false;
            if (_level == 2 && playerLevel < 16) return false;
        }

        return true;
    }

    public void RequestLevelUp(EXPHandler expHandler)
    {
        if(CanLevelUp(expHandler.CurrentLevel, expHandler.SkillPoint))
        {
            expHandler.UseSkillPoint();
            LevelUp();
        }
    }

    public bool TryCast(GameObject target, Vector3 position)
    {
        if(!IsReady) return false;
        if (_skillHandler.IsCasting) return false;

        if (_ownerStat.IsRoot && _data.IsMovementSkill) return false;

        if(_ownerStat.HasEffect(EffectType.Silence) || _ownerStat.HasEffect(EffectType.Stun)) return false;

        float cost = _data.GetCost(_level);
        if (_ownerStat.CurrentMP < cost) return false;

        // 마나 소모 로직 추가
        _ownerStat.RestoreMP(-cost);

        ApplyCooldown();

        // 시전 시간이 있으면 코루틴 시작, 없으면 즉시 발동
        if (_data.CastTime > 0)
        {
            StartCoroutine(CastCoroutine(target, position));
        }
        else
        {
            _data.Execute(_owner, target, position, _level);
        }

        return true;
    }

    private IEnumerator CastCoroutine(GameObject target, Vector3 position)
    {
        // 1. 시전 상태 설정
        _skillHandler.IsCasting = true;

        // 3. 시전 시작 훅 (시각 효과 등)
        //_data.OnCastStart(_owner, position, _level);
        float elapsed = 0f;
        while (elapsed < _data.CastTime)
        {
            // 매 프레임 상태 체크: 사망, 기절, 침묵 시 캐스팅 취소
            if (_ownerStat.IsDead ||
                _ownerStat.HasEffect(EffectType.Stun) ||
                _ownerStat.HasEffect(EffectType.Silence))
            {
                _skillHandler.IsCasting = false;
                _data.OnCastFinished(_owner);
                Debug.Log($"[{_data.name}] 시전이 방해받아 취소되었습니다.");
                yield break; // 코루틴 종료 (Execute 실행 안 됨)
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 5. 스킬 실발동
        _data.Execute(_owner, target, position, _level);

        // 6. 종료 처리
        //_data.OnCastFinished(_owner);
        _skillHandler.IsCasting = false;
    }

    private void ApplyCooldown()
    {
        if (UseChargeSystem)
        {
            if (_currentCharges == _data.MaxCharges)
            {
                _chargeTimer = _data.GetCooldown(_level);
            }
            _currentCharges--;
        }
        else
        {
            _currentCooldown = _data.GetCooldown(_level);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!UseChargeSystem && _currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }

        if(UseChargeSystem && _currentCharges < _data.MaxCharges)
        {
            _chargeTimer -= Time.deltaTime;

            if(_chargeTimer <= 0)
            {
                _currentCharges++;

                if (_currentCharges < _data.MaxCharges)
                {
                    _chargeTimer = _data.GetCooldown(_level);
                }
                else
                {
                    _chargeTimer = 0f;
                }
            }
        }
    }
}
