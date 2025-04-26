using System.Collections;
using Sirenix.Serialization;
using UnityEngine;

public class DarkSpawner : MonoBehaviour
{
    [Header("스폰 지점")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    
    [Header("현재 스테이지 데이터")]
    [SerializeField] private StageData currentStage;
    
    private bool _spawning = false;

    public void StartSpawning(StageData stage)
    {
        currentStage = stage;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (currentStage == null)
        {
            Debug.LogError("스테이지 데이터가 설정되지 않았습니다!");
            yield break;
        }

        // 1. 첫 스폰까지 대기
        yield return new WaitForSeconds(currentStage.StartSpawnDelay);

        _spawning = true;

        // 2. 설정된 수 만큼 Dark 순차 스폰
        for (int i = 0; i < currentStage.DarksCount; i++)
        {
            // 랜덤으로 왼쪽/오른쪽 중 하나 선택
            Transform spawnPoint = Random.value < 0.5f ? leftSpawnPoint : rightSpawnPoint;
            GameObject darkPrefab = currentStage.Darks[Random.Range(0, currentStage.Darks.Count)];
            
            // Dark 인스턴스 생성
            GameObject darkInstance = Instantiate(darkPrefab, spawnPoint.position, Quaternion.identity);

            // 인스턴스에 대해 SetDestinationWhenSpawn 호출
            Enemy enemy = darkInstance.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (spawnPoint == leftSpawnPoint)
                    enemy.SetDestinationWhenSpawn(false);
                else
                    enemy.SetDestinationWhenSpawn(true);
            }
            else
            {
                Debug.LogWarning("DarkPrefab에 Enemy 컴포넌트가 없습니다!");
            }

            // Dark 하나 스폰 알림
            InGameManager.Instance.OnDarkSpawned();
            
            // 다음 Dark까지 대기
            yield return new WaitForSeconds(currentStage.SpawnTerm);
        }

        _spawning = false;
    }
}
