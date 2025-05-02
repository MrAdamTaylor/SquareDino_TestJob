

using Infrastructure.DI.Injector;
using UnityEngine;

public class MouseInputSystem 
{
    private Camera _camera;
    //private readonly IBulletSpawner _bulletSpawner;
    private bool _isEnabled;

    [Inject]
    public void Construct(Camera camera)
    {
        _camera = camera;
        //_bulletSpawner = bulletSpawner;
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
            //_bulletSpawner.Spawn(_camera.transform.position, Quaternion.LookRotation(ray.direction));
        }
    }

    public void Enable()  => _isEnabled = true;
    public void Disable() => _isEnabled = false;
}
