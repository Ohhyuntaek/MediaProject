
using UnityEngine;

public class BountyHunterSpecialAttackState : IState<Ally>
{
    private Transform _hitPos;
    private bool _finished = false;
    public BountyHunterSpecialAttackState(Transform hitpos)
    {
        _hitPos = hitpos;
    }
    public void Enter(Ally ally)
    {
       ally.Animator.SetTrigger("5_SpecialAttack");
       
    }

    public void Update(Ally ally)
    {
       
    }

    public void TranstionTo(Ally ally)
    {
        AnimatorStateInfo stateInfo = ally.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SpecailAttack") && stateInfo.normalizedTime > 0.9f && !_finished)
        {
            _finished = true;
            SpawnBomb(ally);
        }
    }

    public void Exit(Ally ally)
    {
        
    }

    public void SpawnBomb(Ally ally)
    {
        GameObject bombPrefeb = ally.UnitData.SkillEffect[0];
        GameObject bomb = bombPrefeb;
        bomb.AddComponent<BountyHunterBomb>();
        bomb.GetComponent<BountyHunterBomb>().Init(ally.UnitData.DetectionPatternSo, _hitPos);
        GameObject.Instantiate(bomb, _hitPos.position, Quaternion.identity);

    }
}
