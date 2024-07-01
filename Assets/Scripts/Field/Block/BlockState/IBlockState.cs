
using UnityEngine;

namespace Field.Block.State
{
    // 여러가지 상태를 가질 수 있는 블록이기 때문에
    // 상태패턴을 사용
    // 각종 블록의 상태를 바인딩할 수 있는 인터페이스
    public interface IBlockState
    {
        (Sprite, Sprite, bool) GetBlockResource(int value);
        IBlockState OnMouseRightButtonClicked();
        IBlockState OnMouseLeftButtonPressed();
        IBlockState OnMouseLeftButtonReleasedEvent();
        IBlockState OnDraggingPointerExitedEvent();
    }
}