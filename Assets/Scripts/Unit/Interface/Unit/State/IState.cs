using UnityEngine;

public interface IState<T> where T : MonoBehaviour
{
   void Enter(T owner);
   void Update(T owner);
   void Exit(T owner);
}
