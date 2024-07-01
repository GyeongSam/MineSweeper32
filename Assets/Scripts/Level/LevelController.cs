
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    using static LevelDefinitions;

    /// <summary>
    /// 게임의 레벨 설정 기능 관리용 컨트롤러 클래스
    /// </summary>
    public class LevelController : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private LevelView view;
        [SerializeField]
        private LevelDataContainer dataContainer;   //Model역할 스크립터블 오브젝트
        #endregion

        // 외부 노출용 이벤트 프로퍼티
        public UnityEvent<LevelData> OnCurrentLevelDataChangedEvent => dataContainer.OnCurrentLevelDataChangedEvent;

        #region Unity-Methods

        // 라이프 사이클 관리용 유니티 이벤트 리스너 함수들

        // 초기화 시 Gamemanger에 자신 등록
        private void Awake()
        {
            GameManager.Instance.LevelController = this;
        }

        // 게임 환경 조성 진입점
        private void Start()
        {
            GeneratePresetItems();
            Helper().Forget();
            async UniTaskVoid Helper()
            {
                await UniTask.NextFrame();
                dataContainer.CurrentLevelData = dataContainer.DefaultValue;
            }
        }

        // OnEnable, OnDisable 에서는 주로 모듈 내부 리스너 생명주기 관리
        private void OnEnable()
        {
            view.OnWidthInputUpdatedEvent.AddListener(OnWidthChanged);
            view.OnHeightInputUpdatedEvent.AddListener(OnHeightChanged);
            view.OnMineCountInputFieldUpdatedEvent.AddListener(OnMineCountChanged);
            view.OnUpdateButtonClickedEvent.AddListener(OnUpdateButtonClicked);
        }
        private void OnDisable()
        {
            view.OnWidthInputUpdatedEvent.RemoveListener(OnWidthChanged);
            view.OnHeightInputUpdatedEvent.RemoveListener(OnHeightChanged);
            view.OnMineCountInputFieldUpdatedEvent.RemoveListener(OnMineCountChanged);
            view.OnUpdateButtonClickedEvent.RemoveListener(OnUpdateButtonClicked);
        }
        #endregion

        #region Methods

        // 각 인풋 유효성 검사 및 툴팁 안내 목적 리스너 함수들
        private void OnWidthChanged(string input) => TryParseInput(LevelInputType.Width, input, out int _);
        private void OnHeightChanged(string input) => TryParseInput(LevelInputType.Height, input, out int _);
        private void OnMineCountChanged(string input) => TryParseInput(LevelInputType.MineCount, input, out int _);

        // 모든 인풋을 유효성 검사 후 저장하는 리스너 함수
        private void OnUpdateButtonClicked(string widthInput, string heightInput, string mineCountInput)
        {
            if (TryParseInput(LevelInputType.Width, widthInput, out int width)
                && TryParseInput(LevelInputType.Height, heightInput, out int height)
                && TryParseInput(LevelInputType.MineCount, mineCountInput, out int mineCount))
            {
                // 상급 난이도를 초과하는 경우 자동 조정 및 안내 메세지 출력
                if (mineCount > width * height * 99 / 480)
                {
                    mineCount = width * height * 99 / 480;
                    view.AdjustMineCount(mineCount);
                }
                dataContainer.CurrentLevelData = new(width, height, mineCount);
            }
        }

        // 텍스트 형식 인풋을 int 로 파싱 시도
        // 파싱 에러, 범위 미만, 초과 예외 발생 시 view에 tooltip 전시
        private bool TryParseInput(LevelInputType type, string input, out int value)
        {

            if (!int.TryParse(input, out value))
            {
                view.ShowInvalidInputTooltip(type, ParsingErrorMessage);
                return false;
            }
            int minValue = type switch
            {
                LevelInputType.Width => dataContainer.MinWidth,
                LevelInputType.Height => dataContainer.MinHeight,
                LevelInputType.MineCount => dataContainer.MinMineCount,
                _ => 1
            };
            if (value < minValue)
            {
                view.ShowInvalidInputTooltip(type, $"{InputMessagePrefix}{minValue}{InputUnderflowMessagePostfix}");
                return false;
            }

            int maxValue = type switch
            {
                LevelInputType.Width => dataContainer.MaxWidth,
                LevelInputType.Height => dataContainer.MaxHeight,
                LevelInputType.MineCount => dataContainer.MaxMineCount,
                _ => 100
            };
            if (value > maxValue)
            {
                view.ShowInvalidInputTooltip(type, $"{InputMessagePrefix}{maxValue}{InputOverflowMessagePostfix}");
                return false;
            }
            view.HideInvalidInputTooltip(type);
            return true;
        }

        // Scripable object에서 설정된 데이터를 기반으로 프리셋 목록 생성
        // 진행 내내 존재하므로 프리셋 자체적으로 이벤트 라이프사이클 관리할 수 있으므로 람다함수 사용
        private void GeneratePresetItems()
        {
            var presets = dataContainer?.LevelPresets;
            if (presets != null)
            {
                foreach (var preset in presets)
                {
                    view.CreatePresetItem(preset.name, () =>
                    {
                        dataContainer.CurrentLevelData = preset.levelData;
                        view.HideCustomTab();
                    });
                }
            }
        }
        #endregion

    }
}

