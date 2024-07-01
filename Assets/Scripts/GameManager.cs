using Field;
using GameState;
using Level;
using UnityEngine;
using Utility;

/// <summary>
/// 이 클래스는 각종 모듈의 접근성을 제공하는 싱글톤 게임매니저 클래스입니다.
/// 확장성 고려를 위해 모듈 조합형 로직을 채택했습니다.
/// 씬에 등록된 모듈들이 Awake()를 통해 스스로 GameManager에 등록됩니다.
/// Start()에서 상호작용을 위한 이벤트 바인딩이 처리됩니다.
/// 이벤트 호출을 기반으로 상호작용합니다.
/// </summary>
public class GameManager : Singleton<GameManager>

{
    [SerializeField]
    private ResourceContainer resourceContainer;

    // 준비, 종료 등 게임의 상태를 관리하는 컨트롤러입니다.
    public GameStateController GameStateController { get; set; }

    // 넓이, 높이, 지뢰 수 등 레벨을 관리하는 컨트롤러입니다.
    public LevelController LevelController { get; set; }

    // 현재 레벨에서 게임플레이 요소들을 관리하는 컨트롤러입니다.
    public FieldController FieldController { get; set; }

    //게임의 전반적인 이미지 리소스들을 관리하는 스크립트 오브젝트 컨테이너 입니다
    public ResourceContainer ResourceContainer => resourceContainer;



}
