using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyHunterBomb : MonoBehaviour
{
    
    [SerializeField] private LayerMask enemyLayer;

    private Animator _animator;
    private DetectionPatternSO _pattern;
    private Transform _spawnPosition;

    private const float FuseDuration = 1.5f;

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
        // 퓨즈 → 폭발 순서
        StartCoroutine(FuseAndExplodeRoutine());
    }

    private IEnumerator FuseAndExplodeRoutine()
    {
        // 1) 도화선 점화 애니메이션
        _animator.SetTrigger("Fuse");

        // 2) FuseDuration 동안 대기
        yield return new WaitForSeconds(FuseDuration);

        // 3) EXPLOSION 트리거
        _animator.SetTrigger("Explode");

        // 4) 폭발 애니메이션이 90% 이상 재생될 때까지 대기
        while (true)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Explode") && state.normalizedTime >= 0.9f)
                break;
            yield return null;
        }

        // 5) 폭발 범위 내 적에게 데미지
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

        // 4) ContactFilter & 버퍼 준비
        var filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;
        Collider2D[] buffer = new Collider2D[16];

        // 5) 패턴 만큼 확장하며 각 슬롯의 PolygonCollider2D 가져와서 내부 적 감지
        foreach (var ofs in _pattern.cellOffsets)
        {
            int row = baseRow + ofs.y;
            int col = baseCol + ofs.x;

            // 범위 체크
            if (row < 0 || row >= GridTargetManager.Instance.coliderMat.Length) continue;
            if (col < 0 || col >= GridTargetManager.Instance.coliderMat[row].arr_row.Length) continue;

            var poly = GridTargetManager.Instance.coliderMat[row].arr_row[col];
            if (poly == null) continue;

            // 6) 해당 폴리곤 내부의 모든 Collider2D 검사
            int count = poly.Overlap(filter, buffer);
            for (int i = 0; i < count; i++)
            {
                var col2 = buffer[i];
                if (col2 == null) continue;

                // 7) IDamageable 인터페이스 구현체에 데미지 호출
                if (col2.TryGetComponent<IDamageable>(out var dmg))
                {
                    dmg.TakeDamage(10);
                }
            }
        }
    }

}
