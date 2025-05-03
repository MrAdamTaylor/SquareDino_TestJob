using System.Collections.Generic;
using UnityEngine;

namespace Core.Other
{
    public class TagRadiusDetector : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private List<string> _tags;

        private void OnDrawGizmos()
        {
            Vector3 center = transform.position;
            Gizmos.DrawWireSphere(center, _radius);
        }
    
        public List<(string tag, Transform transform)> Scan()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _layerMask);
            List<(string tag, Transform transform)> results = new();

            for (int i = 0; i < colliders.Length; i++)
            {
                string tag = colliders[i].tag;
                if (_tags.Contains(tag))
                    results.Add((tag, colliders[i].transform));
            }

            return results;
        }
    }
}
