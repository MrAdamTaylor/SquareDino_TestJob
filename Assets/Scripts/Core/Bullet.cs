using System.Collections;
using Core.Configs;
using Core.Enemy;
using Core.ObjectPool;
using Infrastructure.DI.Injector;
using UnityEngine;

namespace Core
{
    public class Bullet : MonoBehaviour
    {
        private bool _isConstructed;

        private BulletConfig _config;
        private BulletPool _bulletPool;
        private Vector3 Direction	=> transform.forward;
        Coroutine _lifetimeCor;
        void OnEnable()		=> _lifetimeCor = StartCoroutine( Lifetime_Cor() );
        void OnDisable()	=> StopCoroutine( _lifetimeCor );
    
        [Inject]
        public void Construct(BulletConfig config, BulletPool bulletPool)
        {
            _config = config;
            _bulletPool = bulletPool;
            _isConstructed = true;
        }

        private void Update()
        {
            if (_isConstructed)
            {
                float deltaPos = _config.bulletSpeed * Time.deltaTime;
                Collider collider = Raycast(deltaPos);
            
                if ( collider != null )
                {
                    IDamagable damagable = collider.GetComponentInParent<IDamagable>();

                    damagable?.TakeDamage( _config.bulletDamage );

                    _bulletPool.Return( gameObject );
                }
                Move( deltaPos );
            }
        }
    
        void Move(float delta)			=> transform.position += Direction * delta;
    
        Collider Raycast(float delta)
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
    
        IEnumerator Lifetime_Cor()
        {
            yield return new WaitForSeconds( _config.bulletLifeTime );

            _bulletPool.Return( gameObject );
        }
    }
}
