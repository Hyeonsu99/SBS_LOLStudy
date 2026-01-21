using UnityEngine;

public class JhinEHandler : MonoBehaviour
{
    private JhinEData _data;
    private UnitStat _stat;
    private SkillHandler _skillHandler;

    public void SetUp(JhinEData data)
    {
        _data = data;
        _stat = GetComponent<UnitStat>();
        _skillHandler = GetComponent<SkillHandler>();

        if (_stat != null)
        {
            _stat.OnDeath -= OnDeath;
            _stat.OnDeath += OnDeath;
        }
    }

    private void OnDeath(GameObject killer)
    {
        SkillSlot eSlot = _skillHandler.Slot_E;
        if (eSlot == null || eSlot.Level <= 0) return;

        if (_data.ZonePrefab != null)
        {
            GameObject zoneObj = Instantiate(_data.ZonePrefab, transform.position, Quaternion.identity);
            if (zoneObj.TryGetComponent(out JhinEZone zone))
            {
                zone.Initialize(gameObject, _data, eSlot.Level);
            }
        }
    }

    private void OnDestroy()
    {
        if (_stat != null) _stat.OnDeath -= OnDeath;
    }
}
