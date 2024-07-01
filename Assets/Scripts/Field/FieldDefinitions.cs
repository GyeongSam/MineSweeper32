using System.Collections.Generic;
using Field.Block.State;
using Level;
using UnityEngine;

namespace Field
{
    public static class FieldDefinitions
    {
        public const int Mine = -1;
        public const int Unknown = -1;

        public static LevelData CurrentLevelData;
        public static int BlockCount;

        // BlockState 관련 정적 클래스들
        public static FlaggedState FlaggedState = new();
        public static UnopendState UnopendState = new();
        public static UnopendPressedState UnopendPressedState = new();
        public static OpendState OpendState = new();
        public static OpendPressedState OpendPressedState = new();


        // 좌표 변환용 배열
        private static readonly int[] di = new[] { 1, 0, -1, 0, 1, 1, -1, -1 };
        private static readonly int[] dj = new[] { 0, 1, 0, -1, 1, -1, 1, -1 };

        // Id to 좌표 변환 용 확장 메소드
        public static (int, int) ToPosition(this int value) => (value / CurrentLevelData.width, value % CurrentLevelData.width);

        // 좌표 to Id 변환 용 확장 메소드
        public static int ToId(this (int, int) value) => value.Item1 * CurrentLevelData.width + value.Item2;

        // 좌표 to 주변 좌표 용 확장 메소드
        public static (int, int) MoveTo(this (int, int) value, int direction) => (value.Item1 + di[direction], value.Item2 + dj[direction]);

        // 좌표의 범위 유효성 확인용 확장 메소드
        public static bool IsInRange(this (int, int) value) => value.Item1 >= 0 && value.Item1 < CurrentLevelData.height && value.Item2 >= 0 && value.Item2 < CurrentLevelData.width;

        // 이동할 좌표의 유효성 확인 시도 확장 메소드
        public static bool TryMoveTo(this (int, int) value, int direction, out (int, int) next)
        {
            next = value.MoveTo(direction);
            return next.IsInRange();
        }

        // BlockState의 유효성 확인용 확장 메소드
        public static bool IsValid(this (IBlockState, int) value) => value.Item1 != null;


        // 랜덤 지뢰 뽑기용 정적 메소드
        // idToExclude에 입력 한 id는 제외
        // 시작부터 지뢰인 경우를 제외하기 위함
        public static List<int> GenerateRandomIdList(int idToExclude)
        {
            List<int> ret = new()
            {
                Capacity = CurrentLevelData.mineCount
            };
            int i = 0;
            while (ret.Count < CurrentLevelData.mineCount)
            {
                if (i != idToExclude)
                {
                    ret.Add(i);
                }
                i++;
            }
            for (; i < BlockCount; ++i)
            {
                if (i != idToExclude)
                {
                    int j = Random.Range(0, i + 1);
                    if (j < CurrentLevelData.mineCount)
                    {
                        ret[j] = i;
                    }
                }
            }
            return ret;
        }

    }
}
