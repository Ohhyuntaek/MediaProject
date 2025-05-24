using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BountyHunterBomb : MonoBehaviour
{
    
    [SerializeField] private LayerMask enemyLayer;
    List<Vector2Int> NomalRange =new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(0,1),new Vector2Int(1,1)};
    List<Vector2Int> RainforceRange =new List<Vector2Int>{new Vector2Int(0,0),new Vector2Int(1,0),new Vector2Int(0,1),new Vector2Int(1,1),new Vector2Int(-1,0),new Vector2Int(-1,1)};
    private Animator _animator;
    private List<Vector2Int> _pattern;
    
    private Transform _spawnPosition;
    private UnitData _unitData;

    private bool dir;
    
    

    /// <summary>
    /// 외부에서 초기화할 때 호출하세요.
    /// </summary>
    public void Init(UnitData unitData, Transform spawnPosition,bool isRainforce)
    {
        _unitData = unitData;
        if (isRainforce)
        {
            _pattern = RainforceRange;
        }
        else
        {
            _pattern = NomalRange;
        }
       
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
        Vector2 worldPos = _spawnPosition.position;

      
        if (!GridTargetManager.Instance.TryGetColliderAtPosition(worldPos, out PolygonCollider2D hitPoly))
            return;
        if (!GridTargetManager.Instance.TryGetGridIndex(hitPoly, out int baseRow, out int baseCol))
            return;

       
        var damaged = new HashSet<IDamageable>();

       
        var filter = new ContactFilter2D();
        filter.SetLayerMask(enemyLayer);
        filter.useTriggers = true;
        Collider2D[] buffer = new Collider2D[16];

       
        bool isDown = hitPoly.gameObject.name.Contains("Down");

        
        foreach (var ofs in _pattern)
        {
            int row = baseRow + (isDown ? -ofs.y : ofs.y);
            int col = baseCol + ofs.x;

            // 범위 검사
            if (row < 0 || row >= GridTargetManager.Instance.coliderMat.Length) continue;
            if (col < 0 || col >= GridTargetManager.Instance.coliderMat[row].arr_row.Length) continue;

            var poly = GridTargetManager.Instance.coliderMat[row].arr_row[col];
            if (poly == null) continue;

            // 6) 해당 폴리곤 내부의 모든 콜라이더 검사
            int count = poly.Overlap(filter, buffer);
            for (int i = 0; i < count; i++)
            {
                var col2 = buffer[i];
                if (col2 == null) continue;

                // 7) IDamageable 구현체만, 한 번만 처리
                if (col2.TryGetComponent<IDamageable>(out var dmg)
                    && damaged.Add(dmg))   // Add가 true일 때만 최초 추가
                {
                    dmg.TakeDamage(_unitData.BaseAttack*2f);
                }
            }
        }
    }


}
