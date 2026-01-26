using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ChampionSelectManager : MonoBehaviour
{
    public SkillSlot SceneSlot_Passive;
    public SkillSlot SceneSlot_Q;
    public SkillSlot SceneSlot_W;
    public SkillSlot SceneSlot_E;
    public SkillSlot SceneSlot_R;

    public CinemachineCamera ChampionCam;
    public CinemachineCamera UltimateCam;

    public Transform SpawnPoint;

    public GameObject SelectPanel;

    public List<ChampionData> ChampionList;
    public List<Button> ChampionButtons;

    public event Action<GameObject> OnChampionSpawned;

    private GameObject _currentInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectPanel.SetActive(true);

        for (int i = 0; i < ChampionButtons.Count; i++)
        {
            int index = i;
            if (index < ChampionList.Count)
            {
                if (ChampionList[index] == null)
                    continue;

                ChampionButtons[index].onClick.AddListener(() => SpawnChampion(index));
            }
        }
    }

    public void SpawnChampion(int index)
    {
        if (index >= ChampionList.Count) return;

        ChampionData data = ChampionList[index];

        if (_currentInstance != null) Destroy(_currentInstance);

        if (data.Prefab != null)
        {
            _currentInstance = Instantiate(data.Prefab, SpawnPoint.position, Quaternion.identity);
            _currentInstance.name = data.Name;

            if (_currentInstance.TryGetComponent(out SkillHandler skillHandler) && _currentInstance.TryGetComponent(out UnitStat stat))
            {
                skillHandler.Slot_Passive = SceneSlot_Passive;
                skillHandler.Slot_Q = SceneSlot_Q;
                skillHandler.Slot_W = SceneSlot_W;
                skillHandler.Slot_E = SceneSlot_E;
                skillHandler.Slot_R = SceneSlot_R;

                // 슬롯 자체를 초기화 (데이터 주입 + 주인 설정)
                // 매개변수: (스킬데이터, 주인오브젝트, 주인스탯) - SkillSlot.cs에 정의된 Initialize 호출
                if (SceneSlot_Passive) SceneSlot_Passive.Initialize(data.Passive, _currentInstance, stat);
                if (SceneSlot_Q) SceneSlot_Q.Initialize(data.Q, _currentInstance, stat);
                if (SceneSlot_W) SceneSlot_W.Initialize(data.W, _currentInstance, stat);
                if (SceneSlot_E) SceneSlot_E.Initialize(data.E, _currentInstance, stat);
                if (SceneSlot_R) SceneSlot_R.Initialize(data.R, _currentInstance, stat);
            }

            if(_currentInstance.TryGetComponent(out PlayerController controller))
            {
                controller._defaultCam = ChampionCam.gameObject;
                controller._ultimateCamera = UltimateCam.gameObject;
            }


            // 카메라는 변경 예정
            if (ChampionCam != null)
            {
                ChampionCam.Follow = _currentInstance.transform;
                ChampionCam.LookAt = _currentInstance.transform;
            }

            if (UltimateCam != null)
            {
                UltimateCam.Follow = _currentInstance.transform;
                UltimateCam.LookAt = _currentInstance.transform;
            }


            OnChampionSpawned?.Invoke(_currentInstance);
            SelectPanel.SetActive(false);
        }
    }
}
