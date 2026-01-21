using Sirenix.OdinInspector;
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

        Vector3 targetPos;

        // 1. 유닛 타겟팅 스킬이고 + 타겟 오브젝트가 존재할 때만 -> 타겟의 위치 사용
        if (slot.TargetType == TargetType.Unit && ctx.target != null)
        {
            targetPos = ctx.target.transform.position;
        }
        // 2. 그 외 (지점 타겟팅이거나, 타겟이 없는 경우) -> 클릭한 좌표(ctx.position) 사용
        else
        {
            targetPos = ctx.position;
        }

        // 높이 보정 (Y축 차이 무시)
        targetPos.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, targetPos);

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

    [BoxGroup("Skill Test")]
    [Button(ButtonSizes.Medium)]
    public void TestSkillLevelUp(SkillCommand cmd)
    {
        var slot = GetSKillSlot(cmd);
        if (slot != null) 
        { 
            slot.LevelUp(); 
            Debug.Log($"{cmd} 스킬 레벨업! 현재 스킬 레벨 {slot.Level}");
        }
    }
}
