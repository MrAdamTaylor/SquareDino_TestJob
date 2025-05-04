using System;
using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Player
{
    public class BulletShoot 
    {
        private BulletPool _bulletPool;
        private Camera _camera;
        private PlayerMouseControl _playerMouseControl;

        [Inject]
        public void Construct(PlayerMouseControl playerMouseControl, Camera camera, BulletPool bulletPool)
        {
            _bulletPool = bulletPool;
            _camera = camera;
            _playerMouseControl = playerMouseControl;
            _playerMouseControl.OnClick += Shoot;
        }

        private void Shoot()
        {
            Vector3 screenPos = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(screenPos);
            _bulletPool.Spawn(_camera.transform.position, Quaternion.LookRotation( ray.direction ));
        }

        ~BulletShoot()
        {
            _playerMouseControl.OnClick -= Shoot;
        }
    }
}