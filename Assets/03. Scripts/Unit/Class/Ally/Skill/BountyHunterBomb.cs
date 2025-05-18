using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyHunterBomb : MonoBehaviour
{
    
    [SerializeField] private LayerMask enemyLayer;

    private Animator _animator;
    private DetectionPatternSO _pattern;
    private Transform _spawnPosition;

    

    /// <summary>
    /// 외부에서 초기화할 때 호출하세요.
    /// </summary>
    public void Init(DetectionPatternSO pattern, Transform spawnPosition)
    {
        _pattern       = pattern;
        _spawnPosition = spawnPosition;
        enemyLayer = LayerMask.GetMask("Enemy", "Boss");
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
        StartCoroutine(FuseAndExplodeRoutine());
    }

    private IEnumerator FuseAndExplodeRoutine()
    {
        
        while (true)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Explode") && state.normalizedTime >= 0.9f)
                break;
            yield return null;
        }
        
        // 5) 폭발 범위 내 적에게 데미지
        ParticleManager.Instance.PlaySkillParticle(AllyType.BountyHunter,transform.position,0);
        ExplodeTargets();

        // 6) 투사체 오브젝트 제거
        Destroy(gameObject);
    }

    
    private void ExplodeTargets()
    {
        // 1) 소환 지점 월드 좌표
        Vector2 worldPos = _spawnPosition.position;

        // 2) 해당 지점이 속한 PolygonCollider2D 얻기
        if (!GridTargetManager.Instance.TryGetColliderAtPosition(worldPos, out PolygonCollider2D hitPoly))
            return;

        // 3) 그 콜라이더를 기반으로 행·열 인덱스 얻기
        if (!GridTargetManager.Instance.TryGetGridIndex(hitPoly, out int baseRow, out int baseCol))
            return;

      
        var filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;
        Collider2D[] buffer = new Collider2D[16];

        
        foreach (var ofs in _pattern.cellOffsets)
        {
            int row = baseRow + ofs.y;
            int col = baseCol + ofs.x;

           
            if (row < 0 || row >= GridTargetManager.Instance.coliderMat.Length) continue;
            if (col < 0 || col >= GridTargetManager.Instance.coliderMat[row].arr_row.Length) continue;

            var poly = GridTargetManager.Instance.coliderMat[row].arr_row[col];
            if (poly == null) continue;

            
            int count = poly.Overlap(filter, buffer);
            for (int i = 0; i < count; i++)
            {
                var col2 = buffer[i];
                if (col2 == null) continue;

                
                if (col2.TryGetComponent<IDamageable>(out var dmg))
                {
                    dmg.TakeDamage(10);
                }
            }
        }
    }

}
