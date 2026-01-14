using UnityEngine;

public interface IStatTransformer
{
    // type : 확인할 스탯 , value : 현재까지 계산된 값, baseStat : 레벨업만 적용된 기본값, chain : 모디파이어가 적용된 전체 스탯 
    float Transform(StatType type, float value, IStat baseStat , IStat chain);
}
