using System;
using UnityEngine;

public class Basic3Projectie : MonoBehaviour
{
    private Enemy _caster;
    private Animator _animator;
    private Dawn _target;
    private bool _finished = false;

    private void Awake()
    {   
        
        _target = GameObject.FindFirstObjectByType<Dawn>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime > 0.9f &&!_finished)
        {
            _finished = true;
            _caster.DealToPlayer();
            Destroy(gameObject);
        }
    }

    public void SetCaster(Enemy enemy)
    {
        _caster = enemy;
    }
}
