using System;
using Unity.VisualScripting;
using UnityEngine;

public class TestCollider : MonoBehaviour
{
   public void OnTriggerEnter2D(Collider2D other)
   {
      //Debug.Log(other.name+" 이 들어왓다 잘되네");
   }
}
