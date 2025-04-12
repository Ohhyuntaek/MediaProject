using System.Collections;
using UnityEngine;

public class AllySpecialAttackState : IState<Ally>
{
    private bool _attackTriggered = false;
    private bool dir = false;
    private bool finished = false;

    public AllySpecialAttackState(bool _dir)
    {
        dir = _dir;
    }
    
    public void Enter(Ally owner)
    {
        
        owner.Animator.SetTrigger("5_SpecialAttack");
        owner.SetFinalSkill(true);
    }

    public void Update(Ally owner)
    {
        
        AnimatorStateInfo stateInfo = owner.Animator.GetCurrentAnimatorStateInfo(0);

        
        if (stateInfo.IsName("SpecialAttack") && stateInfo.normalizedTime >= 0.9f &&!finished)
        {
            finished = true;
            owner.StartCoroutine(SpawnProjectileAfterDelay(owner, 0.3f,dir));
            owner.ChangeState(new AllyIdleState(1 / owner.UnitData.AttackSpeed));
            
        }
    }

    public void Exit(Ally owner)
    {
        
    }

    
    private IEnumerator SpawnProjectileAfterDelay(Ally owner, float delay,bool dir)
    {
        Vector3 spawnPosition;
        if (dir)
        {
            spawnPosition = GameObject.Find("RightDestination").transform.position;
        }
        else
        {
            spawnPosition = GameObject.Find("LeftDestination").transform.position;
        }
        yield return new WaitForSeconds(delay);
        if (!_attackTriggered)
        {
            _attackTriggered = true;

           
            GameObject projectilePrefab = owner.UnitData.SkillEffect[0];
            if (projectilePrefab == null)
            {
                
                yield break;
            }
            
            
            GameObject projectileInstance = GameObject.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectileInstance.GetComponent<CentaurLadyProjectile>().SetDestination(dir);
            
        }
    }
}
