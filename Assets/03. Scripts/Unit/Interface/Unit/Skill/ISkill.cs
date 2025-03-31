using UnityEngine;

public interface ISkill<T> where T : MonoBehaviour
{
    void Activate(T owner);
}