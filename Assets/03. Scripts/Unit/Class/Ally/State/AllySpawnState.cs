using System.Collections;
using UnityEngine;

public class AllySpawnState : IState<Ally>
{
    private bool finished = false;
    public void Enter(Ally ally)
    {
        ally.Animator.SetBool("1_Move",false);
        //TODO : 1.파티클 키기
    }

    public void Update(Ally ally)
    {
        TransitionTo(ally);
    }

    public void TransitionTo(Ally ally)
    {
        
      

        if (!finished)
        {
            ally.ForTest();
            finished = true;
        }
             
         
        
        
    }

    

    public void Exit(Ally ally)
    {
       
    }
}
