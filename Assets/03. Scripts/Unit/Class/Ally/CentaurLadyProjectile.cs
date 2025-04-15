using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentaurLadyProjectile : MonoBehaviour
{
    [Header("투사체 이동 설정")]
    public float speed = 5f;           // 투사체 이동 속도
    public Transform spawnPoint;       // 투사체가 이동해야 할 도착 지점 (적군 스폰 지점)
    public LayerMask enemyLayer;       // 적군 레이어

    [Header("넉백 효과 설정")]
    public float knockbackDistance = 0.5f; // 넉백 거리
    public float knockbackDuration = 0.2f; // 넉백 효과 지속 시간

   
    public Ally ownerAlly;
    

    private void Update()
    {
      
        if (spawnPoint != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, spawnPoint.position, step);
            if (Vector3.Distance(transform.position, spawnPoint.position) < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
           
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                
                List<Enemy> hitEnemies = new List<Enemy> { enemy };

                if(ownerAlly != null)
                {
                    ownerAlly.ApplyKnockback(hitEnemies);
                }
                else
                {
                    
                    Transform destination = enemy.GetDestination();
                    if (destination != null)
                    {
                        Vector3 knockDirection = (enemy.transform.position - destination.position).normalized;
                        enemy.transform.position += knockDirection * knockbackDistance;
                    }
                }
            }
            
            
        }
    }
    public void  SetDestination(bool dir)
    {
        if (dir)
        {
            spawnPoint = GameObject.Find("LeftEnemySpawn").transform;
        }
        else
        {
            spawnPoint = GameObject.Find("RightEnemySpawn").transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (spawnPoint != null)
        {
            Gizmos.DrawLine(transform.position, spawnPoint.position);
        }
    }
}
