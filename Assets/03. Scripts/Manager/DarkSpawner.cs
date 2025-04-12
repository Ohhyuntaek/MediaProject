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
            if (spawnPoint == leftSpawnPoint)
            {
                darkPrefab.GetComponent<Enemy>().SetDestinationWhenSpawn(false);
            }
            else
            {
                darkPrefab.GetComponent<Enemy>().SetDestinationWhenSpawn(true);
            }
            
            

            // Dark 인스턴스 생성
            Instantiate(darkPrefab, spawnPoint.position, Quaternion.identity);
            

            // 다음 Dark까지 대기
            yield return new WaitForSeconds(currentStage.SpawnTerm);
        }

        _spawning = false;
    }
}
