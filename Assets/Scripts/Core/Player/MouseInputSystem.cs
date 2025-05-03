using System;
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

        public event Action OnFirstClick;
        
        private bool _firstClickProcessed = false;

        [Inject]
        public void Construct(Camera camera, BulletPool bulletPool)
        {
            _camera = camera;
            _bulletPool = bulletPool;
            _isEnabled = true;
        }

        public void Enable()  => _isEnabled = true;
        public void Disable() => _isEnabled = false;

        public void StartConfigure()
        {
            _firstClickProcessed = false;
        }

        public void Tick()
        {
            if (!_isEnabled)
                return;

            if (!Input.GetMouseButtonDown(0)) 
                return;
            
            if (!_firstClickProcessed)
            {
                _firstClickProcessed = true;
                OnFirstClick?.Invoke();
                OnFirstClick = null;
                return;
            }

            Vector3 screenPos = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(screenPos);
            _bulletPool.Spawn(_camera.transform.position, Quaternion.LookRotation( ray.direction ));
        }
    }
}
