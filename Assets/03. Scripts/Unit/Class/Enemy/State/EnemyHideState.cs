using UnityEngine;

public class EnemyHideState : IState<Enemy>
{
    private Transform _destination;
    private float _hideTime;
    private float _timer = 0f;
    private bool finished = false;

    public EnemyHideState(float rand)
    {
        _hideTime = rand;
    }
    public void Enter(Enemy owner)
    {
         owner.Animator.SetTrigger("5_Hide");
        
    }

    public void Update(Enemy enemy)
    {
        AnimatorStateInfo stateInfo = enemy.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Hide") && stateInfo.normalizedTime > 0.9f && !finished)
        {
            finished = true;
            enemy.PerformHide(_hideTime);
            enemy.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (finished)
        {
            if (_hideTime < 0f)
            {
                enemy.ChangeState(new EnemyWalkState());
            }
            
            _hideTime -= Time.deltaTime;
            float step = enemy.MoveSpeed * Time.deltaTime;
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, enemy.GetDestination().position, step);
            
        }
        
        if (enemy.IsTargetInRange())
        {
            Debug.Log($"{enemy.name} - 도착지에 도달");
            enemy.ChangeState(new EnemyIdleState());
        }
        
        
    }

    public void Exit(Enemy enemy)
    {
        enemy.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
