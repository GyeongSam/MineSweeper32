using UnityEngine;

namespace Field.Block.State
{
    //깃발 꽂힌 상태에서의 로직
    public class FlaggedState : IBlockState
    {
        public (Sprite, Sprite, bool) GetBlockResource(int value) => (GameManager.Instance.ResourceContainer.UnopendBackground, GameManager.Instance.ResourceContainer.FlagIcon, false);

        public IBlockState OnMouseRightButtonClicked() => FieldDefinitions.UnopendState;

        public IBlockState OnDraggingPointerExitedEvent() => null;

        public IBlockState OnMouseLeftButtonPressed() => null;

        public IBlockState OnMouseLeftButtonReleasedEvent() => null;
    }
}


