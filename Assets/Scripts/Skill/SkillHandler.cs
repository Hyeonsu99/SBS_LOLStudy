using UnityEngine;

public class SkillHandler : MonoBehaviour
{
    public ChampionData MyChampionData;
    private UnitStat _stat;

    public SkillSlot Slot_Passive;
    public SkillSlot Slot_Q;
    public SkillSlot Slot_W;
    public SkillSlot Slot_E;
    public SkillSlot Slot_R;

    private void Awake()
    {
        _stat = GetComponent<UnitStat>();
    }

    void Start()
    {
        if (MyChampionData == null) return;

        // UI 슬롯들에게 주인(나)과 스탯(내것)을 주입
        InitializeSlot(Slot_Passive, MyChampionData.Passive);
        InitializeSlot(Slot_Q, MyChampionData.Q);
        InitializeSlot(Slot_W, MyChampionData.W);
        InitializeSlot(Slot_E, MyChampionData.E);
        InitializeSlot(Slot_R, MyChampionData.R);
    }

    private void InitializeSlot(SkillSlot slot, SkillData data)
    {
        if (slot != null && data != null)
            slot.Initialize(data, gameObject, _stat);
    }

    public SkillSlot GetSKillSlot(SkillCommand cmd)
    {
        return cmd switch
        {
            SkillCommand.Q => Slot_Q,
            SkillCommand.W => Slot_W,
            SkillCommand.E => Slot_E,
            SkillCommand.R => Slot_R,
            _ => null
        };
    }

    public void Execute(InputContext ctx)
    {
        switch (ctx.skillCommand)
        {
            case SkillCommand.Q:
                Slot_Q.TryCast(ctx.target, ctx.target.transform.position);
                break;
        }

    }
}
