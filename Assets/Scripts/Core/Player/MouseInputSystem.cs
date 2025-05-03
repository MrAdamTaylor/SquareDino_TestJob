using System;
using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Player
{
    public class MouseInputSystem 
    {
        public bool CanControlPlayer => _canControlPlayer;
        
        private Camera _camera;
        private  BulletPool _bulletPool;
        private bool _isEnabled;
        private bool _canControlPlayer;

        public event Action OnFirstClick;
        
        private bool _firstClickProcessed = false;
        
        [Inject]
        public void Construct(Camera camera, BulletPool bulletPool)
        {
            _camera = camera;
            _bulletPool = bulletPool;
            _isEnabled = true;
        }

        public void StartConfigure()
        {
            _firstClickProcessed = false;
            _canControlPlayer = false;
        }

        public void Tick()
        {
            if (!_isEnabled)
                return;

            if (Input.GetMouseButtonDown(0)) 
            {
                if (!_firstClickProcessed)
                {
                    _firstClickProcessed = true;
                    _canControlPlayer = true;
                    OnFirstClick?.Invoke();
                    OnFirstClick = null;
                    return;
                }

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
