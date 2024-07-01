using System.Linq;
using UnityEngine;



// 게임의 리소스를 전반전으로 관리하는 스크립터블 오브젝트
// 특히 다양한 교체가 일어날 수 있는 

[CreateAssetMenu(menuName = "ScriptableObjects/ResourceContainer")]
public class ResourceContainer : ScriptableObject
{
    public Sprite UnopendBackground => unopendBackground;
    public Sprite EmptySprite => emptySprite;
    public Sprite GetNumberIcon(int id) => id < numberIcons.Count() ? numberIcons[id] : emptySprite;
    public Sprite MineIcon => mineIcon;
    public Sprite FlagIcon => flagIcon;
    public Sprite FlagXIcon => flagXIcon;
    public Sprite MinusIcon => minusIcon;
    public Sprite SmileNormal => smileNormal;
    public Sprite SmileSuccess => smileSuccess;
    public Sprite SmileFail => smileFail;

    [SerializeField]
    private Sprite unopendBackground;
    [SerializeField]
    private Sprite emptySprite;
    [SerializeField]
    private Sprite[] numberIcons;
    [SerializeField]
    private Sprite mineIcon;
    [SerializeField]
    private Sprite flagIcon;
    [SerializeField]
    private Sprite flagXIcon;
    [SerializeField]
    private Sprite minusIcon;
    [SerializeField]
    private Sprite smileNormal;
    [SerializeField]
    private Sprite smileSuccess;
    [SerializeField]
    private Sprite smileFail;

}
