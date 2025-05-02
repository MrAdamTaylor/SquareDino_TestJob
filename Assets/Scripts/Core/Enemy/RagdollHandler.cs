using System.Collections.Generic;
using UnityEngine;

public class RagdollHandler 
{
    private List<Rigidbody> _rigidbodies;

    public void Initialize(Rigidbody[] rigidbodies)
    {
        _rigidbodies = new List<Rigidbody>(rigidbodies);
        Disable();
    }

    public void Enable()
    {
        for (int i = 0; i < _rigidbodies.Count; i++)
        {
            _rigidbodies[i].isKinematic = false;
        }
    }

    public void Disable()
    {
        for (int i = 0; i < _rigidbodies.Count; i++)
        {
            _rigidbodies[i].isKinematic = true;
        }
    }
}
