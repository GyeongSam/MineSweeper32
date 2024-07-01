namespace Level
{
    /// <summary>
    /// 레벨에 사용되는 상수 및 enum 정의
    /// </summary>
    public static class LevelDefinitions
    {
        // 외부에서 사용안하므로 클래스 내부 선언
        public enum LevelInputType
        {
            Width,
            Height,
            MineCount
        };

        public const string ParsingErrorMessage = "숫자를 입력하세요.";
        public const string InputMessagePrefix = "값은 ";
        public const string InputOverflowMessagePostfix = " 이하여야 합니다.";
        public const string InputUnderflowMessagePostfix = " 이상이어야 합니다.";
        public const string TooManyMineMessage = "지뢰 수 자동조절!";
    }

}
