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

    }

    // Update is called once per frame
    void Update()
    {
        if(_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
        }
    }

    public bool TryCast(GameObject target, Vector3 position)
    {
        if (_data == null || _level <= 0) return false;
        if (_currentCooldown > 0) return false;

        float cost = _data.GetCost(_level);
        float currentMp = _ownerStat.Current.Get(StatType.Mp);

        if (currentMp < cost) return false;

        _data.Execute(gameObject, target, position, _level);

        _currentCooldown = _data.GetCooldown(_level);

        return true;
    }

    public void LevelUp()
    {

    }

    public void SetSkill(SkillData newData)
    {

    }
}
