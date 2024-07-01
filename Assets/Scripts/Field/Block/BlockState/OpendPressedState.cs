using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Field.Block.State
{
    // 개방된 후 눌린 상태
    public class OpendPressedState : IBlockState
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

        public IBlockState OnMouseLeftButtonPressed() => null;

        public IBlockState OnMouseLeftButtonReleasedEvent() => FieldDefinitions.OpendState;

        public IBlockState OnDraggingPointerExitedEvent() => FieldDefinitions.OpendState;
    }
}
