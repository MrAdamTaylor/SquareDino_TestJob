using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Player
{
    public class MouseInputSystem 
    {
        private Camera _camera;
        private  BulletPool _bulletPool;
        private bool _isEnabled;

        [Inject]
        public void Construct(Camera camera, BulletPool bulletPool)
        {
            _camera = camera;
            _bulletPool = bulletPool;
            _isEnabled = true;
        }

        public void Tick()
        {
            if (!_isEnabled)
                return;

            if (Input.GetMouseButtonDown(0)) 
            {
                Vector3 screenPos = Input.mousePosition;
                Ray ray = _camera.ScreenPointToRay(screenPos);
                Debug.Log("Clicked");
                _bulletPool.Spawn(_camera.transform.position, Quaternion.LookRotation( ray.direction ));
            }
        }

        public void Enable()  => _isEnabled = true;
        public void Disable() => _isEnabled = false;
    }
}
