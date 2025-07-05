using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("타일 선택 이펙트")]
    [SerializeField] private GameObject spawnTileEffectPrefab;

    private List<AllyTile> allTiles = new();             // 모든 Ally 타일 목록
    private GameObject currentSpawnTileEffect;           // 현재 이펙트 객체 참조
    private AllyTile currentSelectedTile;                // 프리뷰로 선택된 타일 캐시

    void Awake()
    {
        // 씬 내 존재하는 모든 AllyTile 수집
        foreach (AllyTile tile in FindObjectsOfType<AllyTile>())
        {
            allTiles.Add(tile);
        }
    }

    /// <summary>
    /// 비어 있는 타일 중 하나를 미리 선택하여 이펙트를 표시하고 캐싱합니다.
    /// </summary>
    /// <returns>선택된 AllyTile</returns>
    public AllyTile PreviewAvailableTile()
    {
        if (IsOccupiedAllTiles())
        {
            return null;
        }
        
        // 현재 비어 있는 타일 필터링
        List<AllyTile> availableTiles = allTiles.Where(tile => !tile.isOccupied).ToList();
        if (availableTiles.Count == 0) return null;

        // 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, availableTiles.Count);
        currentSelectedTile = availableTiles[randomIndex];

        // 새 이펙트 생성
        ShowSpawnTileEffect(currentSelectedTile);

        return currentSelectedTile;
    }

    public AllyTile MoveableTile()
    {
        List<AllyTile> availableTiles = allTiles.Where(tile => !tile.isOccupied).ToList();
        if (availableTiles.Count == 0) return null;
        int randomIndex = Random.Range(0, availableTiles.Count);
        currentSelectedTile = availableTiles[randomIndex];
        return currentSelectedTile;
        
    }

    /// <summary>
    /// 해당 tile의 위치에 spawnTileEffect 생성
    /// </summary>
    /// <param name="tile"></param>
    public void ShowSpawnTileEffect(AllyTile tile)
    {
        // 이전 이펙트가 있다면 제거
        if (currentSpawnTileEffect != null)
            Destroy(currentSpawnTileEffect);
        
        if (spawnTileEffectPrefab != null)
        {
            currentSpawnTileEffect = Instantiate(
                spawnTileEffectPrefab,
                tile.transform.position,
                Quaternion.identity
            );
        }
    }

    /// <summary>
    /// 모든 타일이 가득 차있으면 true, 하나라도 비어 있으면 false를 반환
    /// </summary>
    /// <returns></returns>
    public bool IsOccupiedAllTiles()
    {
        // 1. 모든 AllyTile 컴포넌트를 가진 오브젝트 리스트
        List<AllyTile> allyTiles = new List<AllyTile>(FindObjectsOfType<AllyTile>());

        // 2. isOccupied가 전부 true인지 검사
        bool allOccupied = allyTiles.All(tile => tile.isOccupied);
        if (allOccupied)
        {
            Debug.Log("모든 AllyTile이 이미 점유되어 있습니다!");
        }
        return allOccupied;
    }

    /// <summary>
    /// 현재 프리뷰로 선택된 타일을 반환합니다.
    /// </summary>
    public AllyTile GetPreviewedTile()
    {
        return currentSelectedTile;
    }

    /// <summary>
    /// 타일 프리뷰 이펙트를 제거하고 선택 상태를 초기화합니다.
    /// </summary>
    public void CancelTilePreview()
    {
        if (currentSpawnTileEffect != null)
        {
            Destroy(currentSpawnTileEffect);
            currentSpawnTileEffect = null;
        }

        currentSelectedTile = null;
    }

    /// <summary>
    /// 스폰이 확정된 후, 이펙트만 제거하고 선택된 타일은 유지합니다.
    /// </summary>
    public void ClearSpawnTileEffect()
    {
        if (currentSpawnTileEffect != null)
        {
            Destroy(currentSpawnTileEffect);
            currentSpawnTileEffect = null;
        }
    }

    /// <summary>
    /// 타일 반환 처리 (Ally가 사망했을 때 호출됨)
    /// </summary>
    /// <param name="position">반환할 타일의 위치</param>
    public void FreeTile(Vector3 position)
    {
        foreach (AllyTile tile in allTiles)
        {
            if (tile.transform.position == position)
            {
                tile.isOccupied = false;
                tile.ally = null;
                //ShowSpawnTileEffect(tile);
                break;
            }
        }
    }
    
}
