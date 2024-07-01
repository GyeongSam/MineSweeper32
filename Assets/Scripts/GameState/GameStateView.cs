using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameState
{

    /// <summary>
    /// 진행, 종료, 성공 등
    /// 게임의 전반적인 상태 관련 요소들을
    /// 시각적으로 나타내주는 UI 클래스
    /// </summary>
    public class GameStateView : MonoBehaviour
    {
        [SerializeField]
        private Button stateButton;
        [SerializeField]
        private Image smileImage;
        [SerializeField]
        private NumberBox mayLeftMineCountBox;  //남은 지뢰숫자 관련 UI
        [SerializeField]
        private NumberBox timerBox;     //남은 시간 관련 UI


        // 외부 리스너 함수 바인딩 제공 목적 이벤트 프로퍼티
        [HideInInspector]
        public UnityEvent OnStateButtonClickedEvent => stateButton.onClick;

        // 화면에 그려주는 역할 함수들
        // 데이터를 전달받아 각종 UI 요소들을 변경
        public void SetMineCountData(int data) => mayLeftMineCountBox.SetData(data);
        public void SetTimerData(int data) => timerBox.SetData(data);
        public void SetStateButtonImage(Image image) => stateButton.image = image;
        public void SetSmileButtonImage(Sprite soruce) => smileImage.sprite = soruce;
        public void SetTime(int time) => timerBox.SetData(time);
        public void SetMineLeftCount(int count) => timerBox.SetData(count);

    }
}
