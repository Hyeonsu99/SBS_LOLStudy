using UnityEngine;
using UnityEngine.UI;

public class SkillObserverUI : MonoBehaviour
{
    public Image Passive;
    public Image Q_Skill;
    public Image W_Skill;
    public Image E_Skill;
    public Image R_Skill;

    public void Bind(GameObject champion)
    {
        // 챔피언에서 SkillHandler 추출
        var handler = champion.GetComponent<SkillHandler>();
        if (handler == null) return;

        // 각 슬롯의 데이터에서 아이콘을 가져와 설정
        SetIcon(Passive, handler.Slot_Passive?.Data);
        SetIcon(Q_Skill, handler.Slot_Q?.Data);
        SetIcon(W_Skill, handler.Slot_W?.Data);
        SetIcon(E_Skill, handler.Slot_E?.Data);
        SetIcon(R_Skill, handler.Slot_R?.Data);
    }

    private void SetIcon(Image img, SkillData data)
    {
        if (img == null) return;

        // 데이터가 있고 아이콘이 있으면 교체
        if (data != null && data.Icon != null) // SkillData에 public Sprite Icon; 이 있다고 가정
        {
            img.sprite = data.Icon;
            img.enabled = true; // 빈 이미지는 숨김 처리 해제
        }
        else
        {
            // 스킬이 없으면 투명하게 처리하거나 기본 이미지
            img.enabled = false;
            // 또는 img.sprite = DefaultIcon;
        }
    }
}
