using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    [SerializeField] private SkillData _data;
    [SerializeField] private int _level = 0;
    private float _currentCooldown;

    private UnitStat _ownerStat;

    public SkillData Data => _data;
    public int Level => _level;

    public bool IsReady => _level > 0 && _currentCooldown <= 0;

    private void Awake()
    {
        _ownerStat = GetComponent<UnitStat>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    public void Initialize(SkillData newData)
    {
        if(_data != null && _level > 0)
        {
            _data.RemovePassive(gameObject);
        }

        _data = newData;
        _level = 0;
        _currentCooldown = 0;
    }

    public void InitializePassive(SkillData newData)
    {
        Initialize(newData);
        // 1레벨로 기본 적용
        LevelUp();
    }

    public void LevelUp()
    {
        if(_data == null || _level >= _data.MaxLevel) return;

        if(_level > 0) _data.RemovePassive(gameObject);

        _level++;

        _data.ApplyPassive(gameObject, _level);
    }


    public bool TryCast(GameObject target, Vector3 position)
    {
        if (_data == null || _level <= 0) return false;
        if (_currentCooldown > 0) return false;

        float cost = _data.GetCost(_level);
        float currentMp = _ownerStat.Current.Get(StatType.Mp);

        if(cost > 0)
        {
            if (currentMp < cost) return false;

            // 마나 소모 로직 추가
        }

        _data.Execute(gameObject, target, position, _level);

        _currentCooldown = _data.GetCooldown(_level);

        return true;
    }


    public void SetSkill(SkillData newData)
    {

    }
}
