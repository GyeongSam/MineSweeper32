using System;
using GameState.Timer;
using Level;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


namespace GameState
{
    /// <summary>
    /// 게임의 상태 관련 enum\
    /// 외부에 노출 및 사용에 유용하도록
    /// 컨트롤러 클래스 바깥에 작성
    /// </summary>
    /// 
    public enum GameStateType
    {
        Ready,
        Playing,
        GameOver,
        Success,
    }

    /// <summary>
    /// 준비, 종료 등 게임 상태 관련 로직을 구성하는 클래스
    /// 상태 전이 관련 로직과, 상태를 전달에 집중할 수 있도록
    /// enum을 기반으로한 상태 패턴으로 로직 구성
    /// </summary>
    public class GameStateController : MonoBehaviour
    {

        #region Fields

        [SerializeField]
        private GameStateView view; //렌더링할 데이터들을 전달할 view
        [SerializeField]
        private GameTimer timer;

        private GameStateType currentState;

        // events
        // 새로운 게임 상태에 돌입할 때 브로드캐스트되는 이벤트
        public UnityEvent<GameStateType> OnGameStateEnteredEvent;
        // 현재 게임 상태를 탈출할 때 브로드캐스트되는 이벤트
        public UnityEvent<GameStateType> OnGameStateExitedEvent;
        #endregion


        #region Unity-Methods

        // 라이프 사이클 관리용 유니티 이벤트 리스너 함수들

        // 초기화 시 Gamemanger에 자신 등록
        private void Awake()
        {
            if (timer == null)
            {
                timer = gameObject.AddComponent<GameTimer>();
            }

            GameManager.Instance.GameStateController = this;
        }

        // Start, OnDestory 에서는 주로 모듈 외부 리스너 생명주기 관리

        private void Start()
        {
            GameManager.Instance.LevelController?.OnCurrentLevelDataChangedEvent?.AddListener(ReadyGame);
            GameManager.Instance.FieldController?.OnMayLeftMineCountChangedEvent?.AddListener(view.SetMineCountData);
        }
        private void OnDestory()
        {
            GameManager.Instance.LevelController?.OnCurrentLevelDataChangedEvent?.RemoveListener(ReadyGame);
            GameManager.Instance.FieldController?.OnMayLeftMineCountChangedEvent?.RemoveListener(view.SetMineCountData);
        }

        // OnEnable, OnDisable 에서는 주로 모듈 내부 리스너 생명주기 관리

        private void OnEnable()
        {
            OnGameStateEnteredEvent.AddListener(OnGameStateEnter);
            OnGameStateExitedEvent.AddListener(OnGameStateExit);
            timer.OnPlayingTimeChangedEvent += OnPlayingTimeChanged;
        }
        private void OnDisable()
        {
            OnGameStateEnteredEvent.RemoveListener(OnGameStateEnter);
            OnGameStateExitedEvent.RemoveListener(OnGameStateExit);
            timer.OnPlayingTimeChangedEvent -= OnPlayingTimeChanged;
        }

        // 타이머(Monobehavior 컴포넌트) 자동 생성용 메소드
        private void OnValidate()
        {
            if (timer == null)
            {
                timer = gameObject.AddComponent<GameTimer>();
            }
        }
        #endregion

        #region Methods

        // 현재 상태를 변경하는 메소드
        // 현재 상태 탈출 이벤트가 브로드캐스트 된 후
        // 다음 상태 돌입 이벤트가 브로드캐스트
        private void ChangeState(GameStateType type)
        {
            OnGameStateExitedEvent?.Invoke(currentState);
            currentState = type;
            OnGameStateEnteredEvent?.Invoke(type);
        }

        // 상태 돌입 이벤트 수신시 로직 매칭용 리스너
        private void OnGameStateEnter(GameStateType type)
        {
            switch (type)
            {
                case GameStateType.Ready:
                    OnReadyStateEnter();
                    break;
                case GameStateType.Playing:
                    OnPlayingStateEnter();
                    break;
                case GameStateType.GameOver:
                    OnGameOverStateEnter();
                    break;
                case GameStateType.Success:
                    OnSuccessStateEnter();
                    break;
                default:
                    break;
            }
        }

        // 상태 탈출 이벤트 수신시 로직 매칭용 리스너
        private void OnGameStateExit(GameStateType type)
        {
            switch (type)
            {
                case GameStateType.Ready:
                    OnReadyStateExit();
                    break;
                case GameStateType.Playing:
                    OnPlayingStateExit();
                    break;
                case GameStateType.GameOver:
                    OnGameOverStateExit();
                    break;
                case GameStateType.Success:
                    OnSuccessStateExit();
                    break;
                default:
                    break;
            }
        }

        // 외부 이벤트에 등록하여 상태를 관리할 리스너 함수들
        private void OnPlayingTimeChanged(float data) => view.SetTimerData((int)data);
        private void ReadyGame(LevelData levelData) => ChangeState(GameStateType.Ready);
        private void StartGame() => ChangeState(GameStateType.Playing);
        private void FinishGame() => ChangeState(GameStateType.GameOver);
        private void WinGame() => ChangeState(GameStateType.Success);
        private void ResetGame() => ChangeState(GameStateType.Ready);


        // 게임 상태별 돌입 및 탈출 시 실행할 리스너 함수들

        // 준비 상태에서 플레이어 인풋을 받아 게임 시작하도록 리스너 부착
        private void OnReadyStateEnter()
        {
            timer.Reset();
            GameManager.Instance.FieldController.OnPlayerInputReceivedEvent += StartGame;
            view.SetSmileButtonImage(GameManager.Instance.ResourceContainer.SmileNormal);
        }

        //부착된 리스너 회수
        private void OnReadyStateExit()
        {
            GameManager.Instance.FieldController.OnPlayerInputReceivedEvent -= StartGame;
        }

        // 게임 진행 돌입 시 종료 조건과 성공 조건 이벤트에 리스너 부착
        private void OnPlayingStateEnter()
        {
            timer.Begin();
            GameManager.Instance.FieldController.OnAllMineFindedEvent.AddListener(WinGame);
            GameManager.Instance.FieldController.OnPlayerMineClickEvent += FinishGame;
        }

        // 부착한 리스너 회수
        private void OnPlayingStateExit()
        {
            GameManager.Instance.FieldController.OnAllMineFindedEvent.RemoveListener(WinGame);
            GameManager.Instance.FieldController.OnPlayerMineClickEvent -= FinishGame;
        }

        private void OnGameOverStateEnter()
        {
            timer.Stop();
            view.SetSmileButtonImage(GameManager.Instance.ResourceContainer.SmileFail);
        }
        private void OnGameOverStateExit()
        {
            timer.Reset();
        }
        private void OnSuccessStateEnter()
        {
            timer.Stop();
            view.SetSmileButtonImage(GameManager.Instance.ResourceContainer.SmileSuccess);
        }
        private void OnSuccessStateExit()
        {
            timer.Reset();
        }

        #endregion











    }

}
