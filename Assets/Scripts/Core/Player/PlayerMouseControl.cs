using System;
using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Player
{
    public class PlayerMouseControl : MonoBehaviour
    {
        private Camera _camera;
        //private BulletPool _bulletPool;
        private bool _isEnabled;
        private BulletShoot _bulletShoot;

        public event Action OnFirstClick;
        public event Action OnClick;
        
        private bool _firstClickProcessed = false;

        [Inject]
        public void Construct(Camera cameral)
        {
            _camera = GetComponent<Camera>();
            //_bulletPool = bulletPool;
            _isEnabled = true;
        }

        public void Enable()  => _isEnabled = true;
        public void Disable() => _isEnabled = false;

        public void StartConfigure()
        {
            _firstClickProcessed = false;
        }

        public void Update()
        {
            if (!_isEnabled)
                return;

            if (!Input.GetMouseButtonDown(0)) 
                return;
            
            if (!_firstClickProcessed)
            {
                _firstClickProcessed = true;
                OnFirstClick?.Invoke();
                return;
            }

            OnClick?.Invoke();
        }
    }
}
