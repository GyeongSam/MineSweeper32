using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Field.Block
{
    // 이미지 리소스를 관리하는 스크립터블 오브젝트
    [CreateAssetMenu(menuName = "ScriptableObjects/BlockResourceContainer")]
    public class BlockResourceContainer : ScriptableObject
    {
        public Sprite UnopendBackground;
        public Sprite EmptySprite;
        public Sprite[] NumberIcons;
        public Sprite MineIcon;
        public Sprite FlagIcon;
        public Sprite FlagXIcon;
        public Sprite PressingSprite;
    }
}

