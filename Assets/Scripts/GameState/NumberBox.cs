using UnityEngine.UI;
using UnityEngine;

namespace GameState
{
    /// <summary>
    /// 최대 세자리 정수를 표시해줄 수 있는 간단한 Box형태 UI
    /// </summary>
    public class NumberBox : MonoBehaviour
    {
        [SerializeField]
        private Image e2Image; //100의 자리 숫자
        [SerializeField]
        private Image e1Image; //10의 자리 숫자
        [SerializeField]
        private Image e0Image; //1의 자리 숫자
        public void SetData(int data)
        {
            ResourceContainer container = GameManager.Instance.ResourceContainer;
            if (data < 0)
            {
                data *= -1;
                if (data >= 100)
                {
                    data = 99;
                }
                e2Image.sprite = container.MinusIcon;
                e1Image.sprite = container.GetNumberIcon(data / 10);
                e0Image.sprite = container.GetNumberIcon(data % 10);
            }
            else
            {
                if (data >= 1000)
                {
                    data = 999;
                }
                e2Image.sprite = container.GetNumberIcon(data / 100);
                data %= 100;
                e1Image.sprite = container.GetNumberIcon(data / 10);
                e0Image.sprite = container.GetNumberIcon(data % 10);

            }
        }
    }
}
