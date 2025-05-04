using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        private Camera _mainCamera;

        private bool _isConstructed;

        public void Construct(Camera camera)
        {
            _mainCamera = camera;
            _isConstructed = true;
        }

        public void SetValue(float normalizedValue)
        {
            _image.fillAmount = normalizedValue;
        }
        
        private void Update()
        {
            if (_isConstructed)
            {
                Quaternion rotation = _mainCamera.transform.rotation;
                transform.LookAt(transform.position + rotation * Vector3.back, rotation * Vector3.up);
            }
        }
    }
}
