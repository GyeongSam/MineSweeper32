using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Level
{
    using static LevelDefinitions;


    /// <summary>
    /// Tooltip 기능이 추가 구현된 인풋 필드 클래스
    /// </summary>
    public class LevelInputField : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField inputfield;
        [SerializeField]
        private int AdjustMessageShowtime = 2000;

        [SerializeField]
        private TMP_Text tooltip;

        public UnityEvent<string> OnValueChangedEvent => inputfield.onValueChanged;

        public string Text => inputfield.text;

        public void SetInvalidMessage(string content)
        {
            tooltip.text = content;
        }
        public void ToggleInvalidTooltip(bool isOn) => tooltip.transform.parent.gameObject.SetActive(isOn);


        // 지뢰 숫자 조정 안내 메세지는 정해진 시간후 자동으로 꺼지도록 설정
        public void AdjustValue(int value)
        {
            inputfield.text = value.ToString();
            SetInvalidMessage(TooManyMineMessage);
            ToggleInvalidTooltip(true);
            Helper().Forget();
            async UniTaskVoid Helper()
            {
                await UniTask.Delay(AdjustMessageShowtime);
                ToggleInvalidTooltip(false);
            }
        }

    }
}
