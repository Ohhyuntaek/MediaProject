using System;
using UnityEngine;
using UnityEngine.UIElements;

public class DawnSpawnManager : MonoBehaviour
{
    [Header("InStage씬에서 스폰할 Dawn")]
    public Dawn spawnedDawn;

    [Header("스폰할 Dawn이 스폰될 장소")] 
    [SerializeField] private GameObject spawnPoint;

    private void Awake()
    {
        SpawnDawn(RuntimeDataManager.Instance.GetSelectedDawn(), spawnPoint);
    }

    /// <summary>
    /// 스폰할 Dawn 설정
    /// </summary>
    /// <param name="dawn"></param>
    // public void SetSpawnedDawn(Dawn dawn)
    // {
    //     this.spawnedDawn = dawn;
    // }

    /// <summary>
    /// 스폰할 Dawn을 Spawn Point에 스폰
    /// </summary>
    /// <param name="dawn"></param>
    /// <param name="spawnPoint"></param>
    public void SpawnDawn(Dawn dawn, GameObject spawnPoint)
    {
        if (dawn == null)
        {
            Debug.LogWarning("Dawn이 비어있습니다.");
            return;
        }
        
        spawnedDawn = Instantiate(dawn.DawnData.Prefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<Dawn>();
    }
}
