using UnityEngine;

public class EnemyWalkState : IState<Enemy>
{
    
    private Transform _destination;
    private float _walkTimer=0f;
    private float rand;
    public void Enter(Enemy enemy)
    {
        _destination = enemy.GetDestination();
        enemy.Animator.SetBool("1_Move",true);
        rand =  Random.Range(2f, 4f);
        
    }

    public void Update(Enemy enemy)
    {
        _walkTimer += Time.deltaTime;
        float step = enemy.MoveSpeed * Time.deltaTime;
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, _destination.position, step);
        
        if (enemy.IsTargetInRange())
        {
            Debug.Log($"{enemy.name} - 도착지에 도달");
            enemy.ChangeState(new EnemyIdleState());
        }
        else
        {
            if (_walkTimer > rand && enemy.EnemyData.EnemyName == "Basic3")
            {
                float hideTime = Random.Range(3f, 5f);
                enemy.ChangeState(new EnemyHideState(hideTime));
            }
        }
    }

    public void Exit(Enemy enemy)
    {   
        enemy.Animator.SetBool("1_Move",false);
        Debug.Log($"{enemy.name} - Walk 상태 종료");
    }
}
