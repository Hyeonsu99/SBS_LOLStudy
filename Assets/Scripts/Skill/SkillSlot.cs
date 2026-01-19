using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private SkillData _data;
    [SerializeField] private int _level = 0;
    private float _currentCooldown;

    private GameObject _owner;
    private UnitStat _ownerStat;

    // 스킬 레벨
    public int Level => _level;
    // 스킬 사용 가능 여부
    public bool IsReady => _level > 0 && _currentCooldown <= 0;
    // 남은 쿨타임 비율
    public float CooldownRatio => _data != null ? _currentCooldown / _data.GetCooldown(_level) : 0f;

    public float CurrentCooldown => _currentCooldown;

    public void Initialize(SkillData newData, GameObject owner, UnitStat stat)
    {
        _data = newData;
        _owner = owner;
        _ownerStat = stat;
       
        _level = 0;
        _currentCooldown = 0;

        if(_data != null && _data.Type_Delivery == DeliveryType.Passive)
        {
            LevelUp();
        }
    }

    public float GetRange()
    {
        if (_data == null) return 0f;
        return _data.GetRange(_level);
    }

    public void LevelUp()
    {
        if(_data == null || _level >= _data.MaxLevel) return;
        if(_level > 0) _data.OnUnEquip(_owner);
        _level++;
        _data.OnEquip(_owner, _ownerStat, _level);
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

        float cost = _data.GetCost(_level);
        float currentMp = _ownerStat.CurrentMP;

        if (currentMp < cost) return false;

        // 마나 소모 로직 추가
        _ownerStat.RestoreMP(-cost);

        _data.Execute(_owner, target, position, _level);
        _currentCooldown = _data.GetCooldown(_level);
        
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
    }
}
