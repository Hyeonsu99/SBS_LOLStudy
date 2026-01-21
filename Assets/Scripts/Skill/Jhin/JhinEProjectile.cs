using UnityEngine;

public class JhinEProjectile : MonoBehaviour
{
    private GameObject _owner;
    private JhinEData _data;
    private int _skillLevel;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _duration;
    private float _timer;

    private const float ARC_HEIGHT = 5.0f;

    public void Initialize(GameObject owner, JhinEData data, int level, Vector3 targetPos)
    {
        _owner = owner;
        _data = data;
        _skillLevel = level;

        _startPos = transform.position;
        _targetPos = targetPos;

        float distance = Vector3.Distance(_startPos, _targetPos);
        _duration = Mathf.Clamp(distance * 0.05f, 0.4f, 1.0f);
        _timer = 0f;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        float t = _timer / _duration;

        if (t >= 1.0f)
        {
            SpawnTrap();
            Destroy(gameObject);
            return;
        }

        Vector3 currentPos = Vector3.Lerp(_startPos, _targetPos, t);
        float height = 4 * ARC_HEIGHT * t * (1 - t); // 포물선 공식
        currentPos.y += height;

        transform.position = currentPos;
        transform.Rotate(Vector3.up * 720f * Time.deltaTime);
    }

    private void SpawnTrap()
    {
        if (_data.TrapPrefab != null)
        {
            GameObject trapObj = Instantiate(_data.TrapPrefab, _targetPos, Quaternion.identity);
            if (trapObj.TryGetComponent(out JhinETrap trap))
            {
                trap.Initialize(_owner, _data, _skillLevel);
            }
        }
    }
}
