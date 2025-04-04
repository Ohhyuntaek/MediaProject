using UnityEngine;

public class EnemyWalkState : IState<Enemy>
{
    
    private Transform _destination; 
    public EnemyWalkState(Transform destination)
    {
        _destination = destination;
    }
    public void Enter(Enemy enemy)
    {
        enemy.Animator.SetBool("1_Move",true);
    }

    public void Update(Enemy enemy)
    {
        if (_destination == null)
        {
            // 도착지 미지정 시, 그냥 오른쪽으로 단순 전진하는 방식(예시)
            enemy.transform.position += enemy.transform.right * enemy.EnemyData.MoveSpeed * Time.deltaTime;
            return;
        }

        // 도착지 오브젝트 위치를 목표로 이동
        float step = enemy.EnemyData.MoveSpeed * Time.deltaTime;
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, _destination.position, step);

        // 목표에 도달하면 Idle 상태 등으로 전환(또는 다른 행동 수행)
        if (Vector3.Distance(enemy.transform.position, _destination.position) < 0.1f)
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
