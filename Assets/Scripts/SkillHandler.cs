using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    public ChampionData MyChampionData;

    public SkillSlot Slot_Passive;
    public SkillSlot Slot_Q;
    public SkillSlot Slot_W;
    public SkillSlot Slot_E;
    public SkillSlot Slot_R;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 데이터 주입
        if(MyChampionData != null)
        {
            if(MyChampionData.Passive != null)
                Slot_Passive.InitializePassive(MyChampionData.Passive);

            Slot_Q.Initialize(MyChampionData.Q);
            Slot_W.Initialize(MyChampionData.W);
            Slot_E.Initialize(MyChampionData.E);
            Slot_R.Initialize(MyChampionData.R);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
