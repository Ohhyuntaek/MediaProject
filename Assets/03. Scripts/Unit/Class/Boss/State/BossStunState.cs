using System.Collections;
using UnityEngine;
using TMPro;

public class BossStunState : IState<Boss>
{
    private float _duration =5F;
    
   

    public void Enter(Boss boss)
    {
        // 이동 애니메이션 끄기
        boss.Animator.SetBool("1_Move", false);

        // StunText 활성화
        boss.StunText.gameObject.SetActive(true);
        boss.StunText.enabled = true;
        Debug.Log("왜 안켜지지");
        // 카운트다운 코루틴 실행
        boss.StartCoroutine(StunCountdown(boss));
    }

    private IEnumerator StunCountdown(Boss boss)
    {
      
        int remaining = Mathf.CeilToInt(_duration);

        while (remaining > 0)
        {
            
            boss.StunText.text = remaining.ToString();

            
            yield return new WaitForSeconds(1f);
            remaining--;
        }

        
        boss.StunText.gameObject.SetActive(false);

       
        boss.InitializeAttack();

        
        boss.ChangeState(new BossIdleState());
    }

    public void Update(Boss boss)
    {
        
    }

    public void Exit(Boss boss)
    {
       
       
    }
}