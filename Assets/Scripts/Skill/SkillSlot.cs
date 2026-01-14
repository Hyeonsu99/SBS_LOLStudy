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

    public void LevelUp()
    {
        if(_data == null || _level >= _data.MaxLevel) return;
        if(_level > 0) _data.OnUnEquip(_owner);
        _level++;
        _data.OnEquip(_owner, _ownerStat, _level);
    }

    public bool TryCast(GameObject target, Vector3 position)
    {
        if(!IsReady) return false;

        float cost = _data.GetCost(_level);
        float currentMp = _ownerStat.Current.Get(StatType.Mp);

        if (currentMp < cost) return false;

        // 마나 소모 로직 추가
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
