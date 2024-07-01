
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Field.Block;
using Field.Block.State;
using UnityEngine;
using UnityEngine.UI;
using GameState;
using System.Threading;
using UnityEngine.Events;

namespace Field
{
    using static FieldDefinitions;

    // 필드에 관련된 요소들을 컨트롤하는 클래스
    // 전반적인 인풋 및 게임 진행 정보에 관련된 정보들을 화면에 나타내는 역할
    public class FieldController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private FieldView fieldView;
        [SerializeField]
        private RectTransform holderTransform;
        [SerializeField]
        private GridLayoutGroup holder;     // 블록들의 부모
        [SerializeField]
        private BlockView blockPrefab;

        private FieldData data = new();             // Field의 Model 역할
        private List<BlockView> blockViews = new(); // block 들의 뷰 관리용 리스트
        private int lastPressedBlockId = Unknown;
        private int lastInteractBlockId = Unknown;
        private CancellationTokenSource token;
        //events

        // 플레이어의 동작이 발생할 때 broadcast하는 이벤트
        // 현재는 준비 상태 -> 게임 시작 상태 처리 목적
        // 추후 리플레이 기능 구현시 동작의 정보를 포함해서 전송
        public event Action OnPlayerInputReceivedEvent;
        public event Action OnPlayerMineClickEvent;
        // 전달받은 레벨 기반으로 블록들을 구성한 후 블록 영역의 크기를 broadcast하는 이벤트
        // 전체적으로 유연한 UI 구성 목적 이벤트
        public event Action<Vector2> OnFieldSizeChangedEvent;

        #endregion


        #region Properties

        // Field관련 주요 이벤트를 외부에서 리슨할 수 있도록 외부로 노출시키는 이벤트 프로퍼티
        public UnityEvent OnAllMineFindedEvent => data.OnAllMineFindedEvent;
        public UnityEvent<int> OnMayLeftMineCountChangedEvent => data.OnMayLeftMineCountChangedEvent;

        #endregion

        #region Unity-Methods
        // 라이프 사이클 관리용 유니티 이벤트 리스너 함수들

        // 초기화 시 Gamemanger에 자신 등록
        private void Awake()
        {
            GameManager.Instance.FieldController = this;
        }

        // Start, OnDestory 에서는 주로 모듈 외부 리스너 생명주기 관리
        private void Start()
        {
            GameManager.Instance.LevelController?.OnCurrentLevelDataChangedEvent?.AddListener(OnLevelChanged);
            GameManager.Instance.GameStateController?.OnGameStateEnteredEvent.AddListener(OnGameStateChanged);
        }
        private void OnDestroy()
        {
            GameManager.Instance.LevelController?.OnCurrentLevelDataChangedEvent?.RemoveListener(OnLevelChanged);
            GameManager.Instance.GameStateController?.OnGameStateEnteredEvent.RemoveListener(OnGameStateChanged);
        }

        // OnEnable, OnDisable 에서는 주로 모듈 내부 리스너 생명주기 관리
        private void OnEnable()
        {
            OnFieldSizeChangedEvent += fieldView.Resize;
            data.OnBlockStateChangedEvent += OnBlockStateChanged;
        }
        private void OnDisable()
        {
            OnFieldSizeChangedEvent -= fieldView.Resize;
            data.OnBlockStateChangedEvent -= OnBlockStateChanged;
        }
        #endregion


        #region Methods


        // id에 해당하는 블록을 newState로 갱신
        // 새로운 state의 OnEnterState 로직에 따라 UI 셋팅 진행

        public void OnBlockStateChanged(int id, (IBlockState, int) blockState)
        {
            blockViews[id].SetBlockImages(blockState.Item1.GetBlockResource(blockState.Item2));
        }


        // Level 설정 변경 이벤트 수신 리스너 함수
        private void OnLevelChanged(Level.LevelData levelData)
        {
            CurrentLevelData = levelData;
            BlockCount = CurrentLevelData.width * CurrentLevelData.height;

            data.Initialize();
            holder.constraintCount = levelData.width;
            InitializeBlockViewCount();
            Helper().Forget();
            async UniTaskVoid Helper()
            {
                await UniTask.NextFrame();
                OnFieldSizeChangedEvent?.Invoke(holderTransform.sizeDelta);
            }
        }

        // block 내부에서 마우스 오른쪽 버튼 클릭 이벤트 수신 시 호출되는 메소드
        private void OnMouseRightButtonClicked(int id)
        {
            var state = data.GetBlockState(id);
            if (!state.IsValid())
            {
                return;
            }
            OnPlayerInputReceivedEvent?.Invoke();
            lastInteractBlockId = id;
            var newState = state.Item1.OnMouseRightButtonClicked();
            if (newState != null)
            {
                data.ChangeBlockState(newState, id);
            }
        }

        // block 내부에서 마우스 왼쪽 버튼 눌림 상태 이벤트 수신 시 호출되는 메소드
        private void OnMouseLeftButtonPressed(int id) => OnMouseLeftButtonPressed(id, true);

        private void OnMouseLeftButtonPressed(int id, bool isDirectPress)
        {
            var state = data.GetBlockState(id);
            if (!state.IsValid())
            {
                return;
            }
            if (isDirectPress)
            {
                lastPressedBlockId = id;
            }
            var newState = state.Item1.OnMouseLeftButtonPressed();
            if (isDirectPress && newState == OpendPressedState)
            {
                if (state.Item2 > 0)
                {
                    for (int d = 0; d < 8; ++d)
                    {
                        if (id.ToPosition().TryMoveTo(d, out (int, int) neer))
                        {
                            int neerId = neer.ToId();
                            OnMouseLeftButtonPressed(neerId, false);
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            if (newState != null)
            {
                data.ChangeBlockState(newState, id);
            }

        }

        // 마우스 왼쪽 버튼 놓음 이벤트 수신 시 호출되는 메소드
        private void OnMouseLeftButtonReleased(int id)
        {
            if (lastPressedBlockId == Unknown)
            {
                return;
            }
            lastInteractBlockId = lastPressedBlockId;
            lastPressedBlockId = Unknown;
            OnPlayerInputReceivedEvent?.Invoke();
            OnMouseLeftButtonReleased(lastInteractBlockId, true);
        }
        private void OnMouseLeftButtonReleased(int id, bool isDirect)
        {
            var state = data.GetBlockState(id);
            if (!state.IsValid())
            {
                return;
            }
            var newState = state.Item1.OnMouseLeftButtonReleasedEvent();
            if (newState == null)
            {
                return;
            }
            data.ChangeBlockState(newState, id);

            if (state.Item2 == Mine)
            {
                ShowFailResult(id);
                return;
            }
            if (state.Item2 == 0)
            {
                BfsHelper(id);
            }
            else if (isDirect && state.Item1 == OpendPressedState)
            {
                int flagCount = 0;
                for (int d = 0; d < 8; ++d)
                {
                    if (id.ToPosition().TryMoveTo(d, out (int, int) next))
                    {
                        int nextId = next.ToId();
                        var nextstate = data.GetBlockState(nextId);
                        OnDraggingPointerExited(nextId, false);
                        if (nextstate.Item1 == FlaggedState)
                        {
                            flagCount += 1;
                        }
                    }
                }
                if (flagCount != state.Item2)
                {
                    return;
                }
                for (int d = 0; d < 8; ++d)
                {
                    if (id.ToPosition().TryMoveTo(d, out (int, int) next))
                    {
                        int nextId = next.ToId();
                        OnMouseLeftButtonReleased(nextId, false);
                    }
                }
            }

            void BfsHelper(int startId)
            {
                Queue<(int, int)> q = new();
                var startPos = startId.ToPosition();
                q.Enqueue(startPos);

                while (q.Count != 0)
                {
                    var now = q.Dequeue();
                    for (int d = 0; d < 8; ++d)
                    {
                        if (now.TryMoveTo(d, out (int, int) next))
                        {
                            int nextId = next.ToId();
                            var state = data.GetBlockState(nextId);
                            if (!state.IsValid() || state.Item1 == OpendState || state.Item1 == OpendPressedState)
                            {
                                continue;
                            }
                            data.ChangeBlockState(OpendState, nextId);
                            if (state.Item2 == 0)
                            {
                                q.Enqueue(next);
                            }
                        }
                    }
                }
            }
        }

        // 왼쪽버튼 눌린상태로 block 범위 탈출 시 호출되는 메소드
        private void OnDraggingPointerExited(int id) => OnDraggingPointerExited(id, true);

        private void OnDraggingPointerExited(int id, bool isDirectExit)
        {
            if (isDirectExit)
            {
                lastPressedBlockId = Unknown;
            }
            var state = data.GetBlockState(id);
            if (!state.IsValid())
            {
                return;
            }
            var newState = state.Item1.OnDraggingPointerExitedEvent();
            if (newState == null)
            {
                return;
            }
            data.ChangeBlockState(newState, id);

            if (isDirectExit && newState == OpendState)
            {
                for (int d = 0; d < 8; ++d)
                {
                    if (id.ToPosition().TryMoveTo(d, out (int, int) next))
                    {
                        int nextId = next.ToId();
                        OnDraggingPointerExited(nextId, false);
                    }
                }
            }
        }

        // 현재 레벨에 맞게 블록 개수를 설정하는 메소드
        // 잦은 GC 활동을 억제하기 위해
        // 당장 필요없는 블록은 비활성화 처리, 추후 필요시 활성화 처리
        private void InitializeBlockViewCount()
        {
            while (blockViews.Count < BlockCount)
            {
                var block = Instantiate(blockPrefab, holder.transform);
                block.Initialize(blockViews.Count);
                blockViews.Add(block);
                block.gameObject.SetActive(false);
            }
            for (int i = 0; i < blockViews.Count; ++i)
            {
                var block = blockViews[i];
                bool isOn = i < BlockCount;
                if (block.gameObject.activeSelf && !isOn)
                {
                    block.OnMouseRightButtonClickedEvent -= OnMouseRightButtonClicked;
                    block.OnMouseLeftButtonPressedEvent -= OnMouseLeftButtonPressed;
                    block.OnMouseLeftButtonReleasedEvent -= OnMouseLeftButtonReleased;
                    block.OnDraggingPointerExitedEvent -= OnDraggingPointerExited;
                    block.gameObject.SetActive(false);
                }
                else if (!block.gameObject.activeSelf && isOn)
                {
                    block.OnMouseRightButtonClickedEvent += OnMouseRightButtonClicked;
                    block.OnMouseLeftButtonPressedEvent += OnMouseLeftButtonPressed;
                    block.OnMouseLeftButtonReleasedEvent += OnMouseLeftButtonReleased;
                    block.OnDraggingPointerExitedEvent += OnDraggingPointerExited;
                    block.gameObject.SetActive(true);
                }
            }
        }
        private void InitializeBlocksState()
        {
            for (int id = 0; id < BlockCount; ++id)
            {
                data.ChangeBlockState(UnopendState, id);
            }
        }

        private void OnGameStateChanged(GameStateType type)
        {
            switch (type)
            {
                case GameStateType.Ready:
                    token?.Cancel();
                    InitializeBlocksState();
                    fieldView.ToggleInputBlockingImage(false);
                    break;
                case GameStateType.Playing:
                    GenerateMines();
                    break;
                case GameStateType.GameOver:
                    fieldView.ToggleInputBlockingImage(true);
                    break;
                case GameStateType.Success:
                    fieldView.ToggleInputBlockingImage(true);
                    break;
            }
        }

        public void ShowFailResult(int startBlockId)
        {
            OnPlayerMineClickEvent?.Invoke();
            token = new();
            PlayRoutine().Forget();
            async UniTaskVoid PlayRoutine()
            {
                Queue<(int, int)> q = new();
                bool[,] v = new bool[CurrentLevelData.height, CurrentLevelData.width];
                var startPos = startBlockId.ToPosition();
                q.Enqueue(startPos);
                v[startPos.Item1, startPos.Item2] = true;

                while (q.Count != 0 && !token.IsCancellationRequested)
                {
                    var now = q.Dequeue();
                    for (int d = 0; d < 8; ++d)
                    {
                        if (now.TryMoveTo(d, out (int, int) next) && !v[next.Item1, next.Item2])
                        {
                            v[next.Item1, next.Item2] = true;
                            int nextId = next.ToId();
                            var state = data.GetBlockState(nextId);
                            q.Enqueue(next);

                            if (state.Item2 != Mine && state.Item1 == FlaggedState)
                            {
                                blockViews[nextId].SetBlockImages(
                                    (GameManager.Instance.ResourceContainer.UnopendBackground,
                                    GameManager.Instance.ResourceContainer.FlagXIcon,
                                    false));
                                await UniTask.Delay(100);
                            }
                            else if (state.Item2 == Mine && state.Item1 == UnopendState)
                            {
                                blockViews[nextId].SetBlockImages(
                                    (GameManager.Instance.ResourceContainer.EmptySprite,
                                    GameManager.Instance.ResourceContainer.MineIcon,
                                    false));
                                await UniTask.Delay(100);
                            }

                        }
                    }
                }
            }
        }


        private void GenerateMines()
        {
            var targets = GenerateRandomIdList(lastInteractBlockId);
            foreach (int target in targets)
            {
                data.PlantMine(target);
            }
        }
        #endregion
    }
}
