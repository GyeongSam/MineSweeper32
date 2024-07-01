using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Field.Block
{
    // 개별 블록 UI 클래스
    // 마우스 관련 인풋을 받아
    // Controller에 넘겨줍니다.
    public class BlockView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {

        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private Image pressedImage;
        private int id;

        // 마우스 입력 관련 이벤트
        // 오른쪽클릭시 호출
        public event Action<int> OnMouseRightButtonClickedEvent;
        // 왼쪽버튼누를 시 호출
        public event Action<int> OnMouseLeftButtonPressedEvent;
        // 왼쪽 버튼 누른 상태 뗄 시 호출
        public event Action<int> OnMouseLeftButtonReleasedEvent;
        // 누른상태로 블록 범위 벗어날 시 호출
        public event Action<int> OnDraggingPointerExitedEvent;

        public void Initialize(int blockId)
        {
            id = blockId;
        }

        public void SetBlockImages((Sprite, Sprite, bool) resource)
        {
            backgroundImage.sprite = resource.Item1;
            iconImage.sprite = resource.Item2;
            pressedImage.gameObject.SetActive(resource.Item3);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnMouseLeftButtonPressedEvent?.Invoke(id);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnMouseRightButtonClickedEvent?.Invoke(id);
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerPress != null)
            {
                OnMouseLeftButtonPressedEvent?.Invoke(id);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerPress != null)
            {
                OnDraggingPointerExitedEvent?.Invoke(id);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnMouseLeftButtonReleasedEvent?.Invoke(id);
            }
        }
    }
}
