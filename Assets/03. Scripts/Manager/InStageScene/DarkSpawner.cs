using System.Collections;
using Sirenix.Serialization;
using UnityEngine;

public class DarkSpawner : MonoBehaviour
{
    [Header("스폰 지점")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private Transform bossSpawnPoint;
    
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
            
            
            GameObject darkPrefab = currentStage.Darks[Random.Range(0, currentStage.Darks.Count)];
            if (darkPrefab.TryGetComponent<Boss>(out var boss))
            {
                GameObject bossInstance = Instantiate(darkPrefab, bossSpawnPoint.position, Quaternion.identity);
            }
            else
            {
                Transform spawnPoint = Random.value < 0.5f ? leftSpawnPoint : rightSpawnPoint;
           
            
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
            }
           

            // Dark 하나 스폰 알림
            InStageManager.Instance.OnDarkSpawned();
            
            // 다음 Dark까지 대기
            yield return new WaitForSeconds(currentStage.SpawnTerm);
        }

        _spawning = false;
    }
}
