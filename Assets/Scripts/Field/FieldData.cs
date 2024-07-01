
using System.Collections.Generic;
using UnityEngine;
using Field.Block.State;
using System;
using UnityEngine.Events;

namespace Field
{
    using static FieldDefinitions;

    public class FieldData
    {
        [SerializeField]
        private List<(IBlockState, int)> blockStateList = new();

        public (IBlockState, int) GetBlockState(int id)
        {
            if (id < 0 || id >= BlockCount)
            {
                return (null, default);
            }
            return blockStateList[id];
        }
        public event Action<int, (IBlockState, int)> OnBlockStateChangedEvent;
        public UnityEvent OnAllMineFindedEvent = new();
        public UnityEvent<int> OnMayLeftMineCountChangedEvent = new();

        private int flagCount = 0;
        private int openedBlockCount = 0;

        public void ChangeBlockState(IBlockState newState, int id)
        {
            if (id < 0 || id >= BlockCount)
            {
                return;
            }
            var nowState = blockStateList[id].Item1;
            blockStateList[id] = (newState, blockStateList[id].Item2);
            OnBlockStateChangedEvent?.Invoke(id, blockStateList[id]);

            if (nowState == FlaggedState)
            {
                flagCount -= 1;
            }
            if (newState == FlaggedState)
            {
                flagCount += 1;
            }
            OnMayLeftMineCountChangedEvent?.Invoke(CurrentLevelData.mineCount - flagCount);
            if (newState == OpendState && nowState != OpendPressedState)
            {
                openedBlockCount += 1;
                if (openedBlockCount + CurrentLevelData.mineCount == BlockCount)
                {
                    Debug.Log(OnAllMineFindedEvent == null);
                    OnAllMineFindedEvent?.Invoke();
                }
            }
        }

        public void Initialize()
        {
            flagCount = 0;
            openedBlockCount = 0;
            bool isShort = BlockCount > blockStateList.Count;
            int shortIdx = isShort ? blockStateList.Count : BlockCount;

            // 현재 할 수 있는 만큼 초기화
            for (int i = 0; i < shortIdx; ++i)
            {
                blockStateList[i] = (UnopendState, 0);
            }
            // 부족할 경우
            if (isShort)
            {
                // 리스트의 반복 재할당을 방지하기 위해 용량을 미리 늘려줍니다
                if (blockStateList.Capacity < BlockCount)
                {
                    blockStateList.Capacity = BlockCount;
                }
                // 부족한 양만큼 추가해줍니다
                while (blockStateList.Count < BlockCount)
                {
                    blockStateList.Add((UnopendState, 0));
                }
            }
        }

        // 하나의 지뢰를 심는 함수
        // 주위모든 지뢰가 아닌칸에 +1처리 포함
        public void PlantMine(int id)
        {
            blockStateList[id] = (UnopendState, Mine);
            var pos = id.ToPosition();
            for (int d = 0; d < 8; ++d)
            {
                if (pos.TryMoveTo(d, out (int, int) next))
                {
                    int neerId = next.ToId();
                    var state = blockStateList[neerId];
                    if (state.Item2 != Mine)
                    {
                        blockStateList[neerId] = (UnopendState, state.Item2 + 1);
                    }
                }
            }
        }
    }
}

