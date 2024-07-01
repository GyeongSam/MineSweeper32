using UnityEngine;

namespace Field.Block.State
{
    // 개방된 상태 로직
    public class OpendState : IBlockState
    {
        public (Sprite, Sprite, bool) GetBlockResource(int value)
        {
            var container = GameManager.Instance.ResourceContainer;
            var ret = (container.EmptySprite, container.EmptySprite, false);
            if (value > 0)
            {
                ret.Item2 = container.GetNumberIcon(value);
            }
            else if (value == FieldDefinitions.Mine)
            {
                ret.Item2 = container.MineIcon;
            }
            return ret;
        }

        public IBlockState OnMouseRightButtonClicked() => null;

        public IBlockState OnMouseLeftButtonPressed() => FieldDefinitions.OpendPressedState;

        public IBlockState OnMouseLeftButtonReleasedEvent() => null;

        public IBlockState OnDraggingPointerExitedEvent() => null;
    }
}