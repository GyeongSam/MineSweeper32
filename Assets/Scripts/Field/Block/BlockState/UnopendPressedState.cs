using UnityEngine;

namespace Field.Block.State
{
    // 개방되기 전 눌린상태 로직
    public class UnopendPressedState : IBlockState
    {
        public (Sprite, Sprite, bool) GetBlockResource(int value) => (GameManager.Instance.ResourceContainer.UnopendBackground, GameManager.Instance.ResourceContainer.EmptySprite, true);
        public IBlockState OnMouseRightButtonClicked() => FieldDefinitions.FlaggedState;
        public IBlockState OnMouseLeftButtonPressed() => null;
        public IBlockState OnMouseLeftButtonReleasedEvent() => FieldDefinitions.OpendState;
        public IBlockState OnDraggingPointerExitedEvent() => FieldDefinitions.UnopendState;
    }
}
