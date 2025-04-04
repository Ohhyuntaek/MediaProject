using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyWalkState : IState<Enemy>
{
    private Vector3Int _currentCell;
    private Vector3 _targetPosition;
    private bool _targetAssigned = false;

    public void Enter(Enemy enemy)
    {
        // 적이 현재 위치한 셀 좌표를 계산 후, 그 셀의 중심에 스냅합니다.
        _currentCell = enemy.EnemyPathTilemap.WorldToCell(enemy.transform.position);
        _targetPosition = enemy.EnemyPathTilemap.GetCellCenterWorld(_currentCell);
        enemy.transform.position = _targetPosition;
        _targetAssigned = false;

        Debug.Log($"{enemy.name} - Walk 상태 시작, 현재 셀: {_currentCell}");
    }

    public void Update(Enemy enemy)
    {
        // 여기서는 간단하게 오른쪽으로 한 칸씩 이동하는 예시를 보여줍니다.
        // 실제 게임에서는 경로 탐색(예: A* 알고리즘 등)을 통해 다음 목표 셀을 결정할 수 있습니다.
        if (!_targetAssigned)
        {
            // 예시: 오른쪽으로 한 칸 이동 (타일맵 구조에 따라 조정 필요)
            Vector3Int nextCell = _currentCell + new Vector3Int(1, 0, 0);
            // 타일이 존재하는지 확인 (없으면 경로 상으로 이동 불가)
            if (enemy.EnemyPathTilemap.HasTile(nextCell))
            {
                _targetPosition = enemy.EnemyPathTilemap.GetCellCenterWorld(nextCell);
                _targetAssigned = true;
                Debug.Log($"{enemy.name} - 다음 셀: {nextCell}, 목표 위치: {_targetPosition}");
            }
            else
            {
                // 다음 셀이 없으면 대기 상태로 전환하거나 다른 처리를 할 수 있음
                Debug.Log($"{enemy.name} - 다음 셀 없음, Idle 상태 전환");
                enemy.ChangeState(new EnemyIdleState());
            }
        }

        // 목표 위치를 향해 이동 (타일 중앙 좌표로)
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, _targetPosition, enemy.EnemyData.MoveSpeed * Time.deltaTime);

        // 목표 위치에 도달하면, 현재 셀 업데이트 및 다음 목표 할당 플래그 초기화
        if (Vector3.Distance(enemy.transform.position, _targetPosition) < 0.1f)
        {
            _currentCell += new Vector3Int(1, 0, 0);  // 오른쪽으로 한 칸 이동
            enemy.transform.position = enemy.EnemyPathTilemap.GetCellCenterWorld(_currentCell);
            _targetAssigned = false;
            Debug.Log($"{enemy.name} - 셀 도착: {_currentCell}");
        }
    }

    public void Exit(Enemy enemy)
    {
        // 필요 시 클린업 처리
        Debug.Log($"{enemy.name} - Walk 상태 종료");
    }
}
