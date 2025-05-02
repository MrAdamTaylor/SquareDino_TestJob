using System;
using UnityEngine;

namespace Core.ObjectPool
{
    public class GameObjectPool : PoolBase<GameObject>
    {
        public GameObjectPool(Func<GameObject> func, int preloadCount)
            :base(()=> NonCreatePreload(func),GetAction, ReturnAction, preloadCount) {}
    
        private static GameObject NonCreatePreload(Func<GameObject> func)
        {
            GameObject prefab = func.Invoke();
            return prefab;
        }

        private static void GetAction(GameObject gObject) => gObject.SetActive(true);
    
        private static void ReturnAction(GameObject gObject) => gObject.SetActive(false);
    }
}


