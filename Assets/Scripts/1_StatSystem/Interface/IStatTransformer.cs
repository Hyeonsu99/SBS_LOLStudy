using UnityEngine;

public interface IStatTransformer
{
    // type : 변경할 스탯 , value : 변경할 스탯의 값, baseStat : Transfomer에 의해 변경되기 전 값
    float Transform(StatType type, float value, IStat baseStat);
}
