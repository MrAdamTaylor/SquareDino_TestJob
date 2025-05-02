using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void SetValue(float normalizedValue)
        {
            _image.fillAmount = normalizedValue;
        }
    }
}
