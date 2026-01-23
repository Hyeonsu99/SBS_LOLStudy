using UnityEngine;

public interface IInputOverride
{
    // false면 행동 취소 후 기본 이동 수행, true면 이동 X
    bool OnMoveInput();

    // 스킬 입력이 들어왔을 때 호출
    bool OnSkillInput(SkillCommand command);
}
