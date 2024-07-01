using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Level
{
    /// <summary>
    /// 레벨 구성을 정보를 담고 있는 프리셋 UI 클래스
    /// </summary>
    public class LevelPresetItem : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        public TMP_Text title;

        public void Initialize(string presetName, UnityAction onClickCallback)
        {
            title.text = presetName;
            button.onClick.AddListener(onClickCallback);
        }

        // 익명 함수를 바인딩하여 뷰 자체적으로 리스너 라이프 사이클 관리
        private void OnDestroy()
        {
            button.onClick?.RemoveAllListeners();
        }


    }

}
