using System;
using Infrastructure.DI.Injector;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator _animator;
    private Health _health;
    private RagdollHandler _ragdollHandler;
    
    [Inject]
    public void Construct(Health health,RagdollHandler ragdollHandler)
    {
        _animator = GetComponent<Animator>();
        _health = health;
        _ragdollHandler = ragdollHandler;
        _ragdollHandler.Initialize(GetComponentsInChildren<Rigidbody>());
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
            Kill();
    }

    private void Kill()
    {
        _animator.enabled = false;
        _ragdollHandler.Enable();
    }
}

public class Health
{
}
