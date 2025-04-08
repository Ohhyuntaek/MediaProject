using UnityEngine;

public class EnemyWalkState : IState<Enemy>
{
    
    private Transform _destination; 
   
    public void Enter(Enemy enemy)
    {
        _destination = enemy.GetDestination();
        enemy.Animator.SetBool("1_Move",true);
    }

    public void Update(Enemy enemy)
    {
        // 도착지 오브젝트 위치를 목표로 이동
        float step = enemy.MoveSpeed * Time.deltaTime;
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, _destination.position, step);

        // 목표에 도달하면 Idle 상태 등으로 전환(또는 다른 행동 수행)
        if (enemy.IsTargetInRange())
        {
            Debug.Log($"{enemy.name} - 도착지에 도달");
            enemy.ChangeState(new EnemyIdleState());
        }
    }

    public void Exit(Enemy enemy)
    {
        Debug.Log($"{enemy.name} - Walk 상태 종료");
    }
}
