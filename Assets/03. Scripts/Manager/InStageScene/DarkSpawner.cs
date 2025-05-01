using System.Collections;
using Sirenix.Serialization;
using UnityEngine;

public class DarkSpawner : MonoBehaviour
{
    [Header("스폰 지점")]
    [SerializeField] private Transform leftSpawnPoint;
    [SerializeField] private Transform rightSpawnPoint;
    [SerializeField] private Transform bossSpawnPoint;
    
    private bool isSpawning = false;
    private Coroutine spawnRoutine;
    /// <summary>
    /// StageData의 DarksCount를 받아오기 위한 임시 변수
    /// </summary>
    private int darkCount = 0;

    public int DarksCount
    {
        get { return darkCount; }
        set { darkCount = value; }
    }

    /// <summary>
    /// Dark 스폰 시작
    /// </summary>
    /// <param name="stageData">해당 스테이지의 Stage Data</param>
    public void StartSpawning(StageData stageData)
    {
        StopSpawning(); // 기존 스폰 중단 먼저
        darkCount = stageData.DarksCount;
        spawnRoutine = StartCoroutine(SpawnRoutine(stageData));
    }
    
    /// <summary>
    /// Dark 스폰 종료
    /// </summary>
    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    /// <summary>
    /// 소환된 Dark가 죽으면 darkCount를 1만큼 감소
    /// </summary>
    public void DecreaseDarkCount()
    {
        darkCount--;
    }

    /// <summary>
    /// Dark 스폰 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnRoutine(StageData stageData)
    {
        if (stageData == null)
        {
            Debug.LogError("스테이지 데이터가 설정되지 않았습니다!");
            yield break;
        }

        // 1. 첫 스폰까지 대기
        yield return new WaitForSeconds(stageData.StartSpawnDelay);

        isSpawning = true;

        // 2. 설정된 수 만큼 Dark 순차 스폰
        for (int i = 0; i < stageData.DarksCount; i++)
        {   
            GameObject darkPrefab = stageData.Darks[Random.Range(0, stageData.Darks.Count)];
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
            
            // 다음 Dark까지 대기
            yield return new WaitForSeconds(stageData.SpawnTerm);
        }

        isSpawning = false;
    }
}
