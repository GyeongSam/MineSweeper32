using UnityEngine;
using UnityEngine.UI;

namespace Field
{
    public class FieldView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;
        [SerializeField]
        private Image inputBlockingImage;

        public void Resize(Vector2 sizeDelta)
        {
            rectTransform.sizeDelta = sizeDelta;
        }
        public void ToggleInputBlockingImage(bool isOn) => inputBlockingImage.gameObject.SetActive(isOn);
    }

}
