using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Level
{
    using static LevelDefinitions;

    // 레벨 설정 UI에 대한 뷰 클래스
    public class LevelView : MonoBehaviour
    {
        #region Fields
        [Header("Level Custom")]
        [SerializeField]
        private Button customButton;
        [SerializeField]
        private RectTransform levelCustomCanvas;
        [SerializeField]
        private LevelInputField widthInputField;
        [SerializeField]
        private LevelInputField heightInputField;
        [SerializeField]
        private LevelInputField mineCountInputField;
        [SerializeField]
        private Button updateButton;

        [Header("Level Preset")]
        [SerializeField]
        private LayoutGroup holder; // 레벨 프리셋 버튼을 생성할 때 사용할 부모객체
        [SerializeField]
        private LevelPresetItem presetPrefab;

        #endregion

        #region Properties

        // events
        // 외부 리스너 바인딩용 이벤트 프로퍼티

        // 가로크기 입력시 호출 이벤트 
        [HideInInspector]
        public UnityEvent<string> OnWidthInputUpdatedEvent => widthInputField.OnValueChangedEvent;
        // 세로크기 입력시 호출 이벤트 
        [HideInInspector]
        public UnityEvent<string> OnHeightInputUpdatedEvent => heightInputField.OnValueChangedEvent;
        // 지뢰숫자 입력시 호출 이벤트
        [HideInInspector]
        public UnityEvent<string> OnMineCountInputFieldUpdatedEvent => mineCountInputField.OnValueChangedEvent;
        // 업데이트 버튼 클릭시 호출 이벤트
        [HideInInspector]
        public UnityEvent<string, string, string> OnUpdateButtonClickedEvent;
        #endregion


        #region Methods

        private void OnEnable()
        {
            customButton.onClick.AddListener(ShowCustomTab);
            updateButton.onClick.AddListener(OnUpdateButtonClicked);
        }
        private void OnDisable()
        {
            customButton.onClick.RemoveListener(ShowCustomTab);
            updateButton.onClick.RemoveListener(OnUpdateButtonClicked);
        }

        // 커스텀 UI 온/오프 함수
        public void ToggleLevelCustomCanvas(bool isOn) => levelCustomCanvas.gameObject.SetActive(isOn);


        // 스크립터블 오브젝트에서 설정한 프리셋 정보 하나를 전달받아 아이템 생성 및 초기화
        public void CreatePresetItem(string presetName, UnityAction onClickCallback)
        {
            var item = Instantiate(presetPrefab, holder.transform);
            item.Initialize(presetName, onClickCallback);
            item.transform.SetSiblingIndex(holder.transform.childCount - 2);
            item.gameObject.SetActive(true);
        }

        // 입력값이 유효하지 않을 경우 툴팁 표시
        public void ShowInvalidInputTooltip(LevelInputType type, string content)
        {
            var target = type switch
            {
                LevelInputType.Width => widthInputField,
                LevelInputType.Height => heightInputField,
                LevelInputType.MineCount => mineCountInputField,
                _ => null
            };
            target?.SetInvalidMessage(content);
            target?.ToggleInvalidTooltip(true);
        }

        public void HideInvalidInputTooltip(LevelInputType type)
        {
            switch (type)
            {
                case LevelInputType.Width:
                    widthInputField?.ToggleInvalidTooltip(false);
                    break;
                case LevelInputType.Height:
                    heightInputField?.ToggleInvalidTooltip(false);
                    break;
                case LevelInputType.MineCount:
                    mineCountInputField?.ToggleInvalidTooltip(false);
                    break;
                default:
                    break;
            };
        }
        
        public void AdjustMineCount(int count) => mineCountInputField.AdjustValue(count);
        public void ShowCustomTab() => levelCustomCanvas.gameObject.SetActive(true);
        public void HideCustomTab() => levelCustomCanvas.gameObject.SetActive(false);
        private void OnUpdateButtonClicked() => OnUpdateButtonClickedEvent?.Invoke(widthInputField.Text, heightInputField.Text, mineCountInputField.Text);

        #endregion

    }
}