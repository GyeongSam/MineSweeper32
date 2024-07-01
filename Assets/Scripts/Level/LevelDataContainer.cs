using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    // 레벨에 관련된 여러 기본 데이터를 모아놓은 클래스
    // 인스펙터를 통해 쉽고 빠르게 관리할 수 있도록 ScriptableObject 상속
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelDataContainer")]
    public class LevelDataContainer : ScriptableObject
    {
        #region Fields
        //환경 설정용 인스펙터 노출 변수들
        [SerializeField]
        private LevelPresetData[] levelPresets;
        [SerializeField]
        private LevelData maxValue = new(100, 100, 10000);
        [SerializeField]
        private LevelData minValue = new(1, 1, 1);
        [SerializeField]
        private LevelData defaultValue = new(9, 9, 10);

        private LevelData currentLevelData;

        //events
        // 현재 레벨 데이터가 변할 시 호출
        public UnityEvent<LevelData> OnCurrentLevelDataChangedEvent;
        #endregion

        #region Properties

        // 현재 레벨 데이터 프로퍼티
        // 변경시 이벤트 브로드캐스트
        public LevelData CurrentLevelData
        {
            get => currentLevelData;
            set
            {
                currentLevelData = value;
                OnCurrentLevelDataChangedEvent?.Invoke(value);
            }
        }
        public LevelData DefaultValue => defaultValue;

        // 원본 데이터 변조 방지를 위한 프리셋 배열 복사본 반환 프로퍼티
        public LevelPresetData[] LevelPresets => levelPresets.ToArray();

        // 사용자 입력 수치 검증용 프로퍼티들
        public int MaxWidth => maxValue.width;
        public int MinWidth => minValue.width;
        public int MaxHeight => maxValue.height;
        public int MinHeight => minValue.height;
        public int MaxMineCount => maxValue.mineCount;
        public int MinMineCount => minValue.mineCount;

        #endregion
    }
}
