using UnityEngine;

// 공격 제약 조건 인터페이스 
public interface IAttackConstraint
{
    // 공격 가능 여부
    bool CanAttack();

    // 공격 성공 시 호출 
    void OnAttack();
}
