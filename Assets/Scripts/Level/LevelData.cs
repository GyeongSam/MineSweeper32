namespace Level
{
    [System.Serializable]
    public struct LevelData
    {
        public int width;
        public int height;
        public int mineCount;

        public LevelData(int width, int height, int mineCount)
        {
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
        }
    }
    [System.Serializable]
    public struct LevelPresetData
    {
        public string name;
        public LevelData levelData;
    }
}
