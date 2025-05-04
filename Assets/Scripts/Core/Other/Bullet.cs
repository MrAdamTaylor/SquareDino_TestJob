using System.Collections;
using Core.Configs;
using Core.Enemy;
using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core.Other
{
    public class Bullet : MonoBehaviour
    {
        private BulletConfig _config;
        private BulletPool _bulletPool;
        private Vector3 Direction	=> transform.forward;
        
        private Coroutine _lifetimeCor;
        
        private bool _isConstructed;

        [Inject]
        public void Construct(BulletConfig config, BulletPool bulletPool)
        {
            _config = config;
            _bulletPool = bulletPool;
            _isConstructed = true;
        }

        void OnEnable()
        {
            if (_isConstructed)
                _lifetimeCor = StartCoroutine( Lifetime_Cor() );
        }

        void OnDisable()
        {
            if (_lifetimeCor != null)
            {
                StopCoroutine(_lifetimeCor);
                _lifetimeCor = null; 
            }
        }

        private void Update()
        {
            if (_isConstructed)
            {
                float deltaPos = _config.BulletSpeed * Time.deltaTime;
                Collider collider = Raycast(deltaPos);
            
                if ( collider != null )
                {
                    IDamageable damageable = collider.GetComponentInParent<IDamageable>();

                    damageable?.TakeDamage( _config.BulletDamage );

                    _bulletPool.Return( gameObject );
                }
                Move( deltaPos );
            }
        }
    
        private void Move(float delta)	=> transform.position += Direction * delta;
    
        private Collider Raycast(float delta)
        {
            Vector3 origin	= transform.position;
            Vector3 dir		= Direction;

            Physics.Raycast( 
                origin, 
                dir,
                out RaycastHit hitInfo, 
                delta, 
                LayerMask.GetMask( "Default" ) 
            );

            return hitInfo.collider;
        }
    
        private IEnumerator Lifetime_Cor()
        {
            yield return new WaitForSeconds( _config.BulletLifeTime );

            _bulletPool.Return( gameObject );
        }
    }
}
