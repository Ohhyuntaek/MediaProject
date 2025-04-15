using System;
using UnityEngine;

public class Basic3Projectie : MonoBehaviour
{
    private Enemy _caster;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.9f)
        {
            Destroy(gameObject);
        }
    }
}
