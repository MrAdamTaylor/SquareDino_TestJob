using System.Collections.Generic;
using UnityEngine;

namespace Core.Enemy
{
    public class RagdollHandler 
    {
        private List<Rigidbody> _rigidbodies = new();
        private Rigidbody _mainBone;
        private Transform _transform;
        private float _enemyThrowForce;

        public void Initialize(Rigidbody[] rigidbodies, Rigidbody mainBone, Transform transform, float throwForce)
        {
            _mainBone = mainBone;
            _rigidbodies = new List<Rigidbody>(rigidbodies);
            _transform = transform;
            _enemyThrowForce = throwForce;
            Disable();
        }

        public void Enable()
        {
            for (int i = 0; i < _rigidbodies.Count; i++)
            {
                _rigidbodies[i].isKinematic = false;
            }
        }

        public void ThrowRagdoll(Transform forceSubjectTransform)
        {
            Vector3 forceDir = (_transform.position - forceSubjectTransform.position).normalized;
            Vector3 force = forceDir * _enemyThrowForce;
            
            _mainBone.AddForce(force, ForceMode.VelocityChange);
        }

        public void Disable()
        {
            for (int i = 0; i < _rigidbodies.Count; i++)
            {
                _rigidbodies[i].isKinematic = true;
            }
        }
    }
}
