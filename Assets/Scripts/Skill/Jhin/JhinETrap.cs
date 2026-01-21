using UnityEngine;

public class JhinETrap : MonoBehaviour
{
    private GameObject _owner;
    private JhinEData _data;
    private int _skillLevel;
    private UnitIdentity _ownerIdentity;

    private int _currentHealth = 6;
    private bool _isActivated = false;

    [SerializeField] private MeshRenderer _renderer;

    public void Initialize(GameObject owner, JhinEData data, int level)
    {
        _owner = owner;
        _data = data;
        _skillLevel = level;
        _ownerIdentity = owner.GetComponent<UnitIdentity>();

        Destroy(gameObject, _data.TrapDuration);
    }

    public void OnAttackHit(bool isRanged)
    {
        if (_isActivated) return;
        int damage = isRanged ? 2 : 3;
        _currentHealth -= damage;
        if (_currentHealth <= 0) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivated) return;

        if (other.TryGetComponent(out UnitIdentity targetId))
        {
            if (!_ownerIdentity.IsEnemy(targetId)) return;

            if (targetId.Type != UnitType.Structure)
            {
                ActivateTrap();
            }
        }
    }

    private void ActivateTrap()
    {
        _isActivated = true;
        if (_data.ZonePrefab != null)
        {
            GameObject zoneObj = Instantiate(_data.ZonePrefab, transform.position, Quaternion.identity);
            if (zoneObj.TryGetComponent(out JhinEZone zone))
            {
                zone.Initialize(_owner, _data, _skillLevel);
            }
        }
        Destroy(gameObject);
    }
}
