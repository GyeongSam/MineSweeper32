using UnityEngine;

namespace Field.Block.State
{
    // 개방되기 전 상태 로직
    public class UnopendState : IBlockState
    {
        public (Sprite, Sprite, bool) GetBlockResource(int value) => (GameManager.Instance.ResourceContainer.UnopendBackground, GameManager.Instance.ResourceContainer.EmptySprite, false);
        public IBlockState OnMouseRightButtonClicked() => FieldDefinitions.FlaggedState;

        public IBlockState OnMouseLeftButtonPressed() => FieldDefinitions.UnopendPressedState;

        public IBlockState OnMouseLeftButtonReleasedEvent() => FieldDefinitions.OpendState;

        public IBlockState OnDraggingPointerExitedEvent() => null;
    }
}
