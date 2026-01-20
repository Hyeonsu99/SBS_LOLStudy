using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
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

    public bool IsSkillInRange(InputContext ctx)
    {
        SkillSlot slot = GetSKillSlot(ctx.skillCommand);
        if(slot == null) return false;

        if(slot.TargetType == TargetType.Direction || slot.TargetType == TargetType.None)
        {
            return true;
        }

        float range = slot.GetRange();

        Vector3 targetpos = ctx.target != null ? ctx.target.transform.position : ctx.position;
        
        targetpos.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetpos);

        return distance <= range;
    }

    public bool IsSkillReady(SkillCommand cmd)
    {
        SkillSlot slot = GetSKillSlot(cmd);
        return slot != null && slot.IsReady;
    }

    public void Execute(InputContext ctx)
    {
        SkillSlot slot = GetSKillSlot(ctx.skillCommand);

        if (slot != null)
        {
            slot.TryCast(ctx.target, ctx.position);
        }
    }
}
