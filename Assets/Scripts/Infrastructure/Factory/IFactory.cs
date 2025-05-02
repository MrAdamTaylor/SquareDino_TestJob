using UnityEngine;

namespace Infrastructure.Factory
{
    public interface IFactory
    {
        public GameObject Create(Vector3 position, Transform parent);
    }
}