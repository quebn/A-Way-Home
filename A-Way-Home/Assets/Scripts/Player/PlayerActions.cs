using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance { get; private set; }

    [HideInInspector] public string currentTool;
    [HideInInspector] public Mouse mouse;

    private Camera mainCamera;
    private PlayerInput playerInput;
    private InputAction performAction; 
    private InputAction revealPath;
    private InputAction undoAction;
    private InputAction tool1; 
    private InputAction tool2; 
    private InputAction tool3; 
    private InputAction tool4; 
    private InputAction tool5; 
    private InputAction tool6; 
    private InputAction start;
    private InputAction reset;

    private IObstacle currentObstacle;

    private void Start()
    {
        Initialize();
        SubscribeFunctions();
        if (Instance == null)
            Instance = this;
    }

    private void FixedUpdate()
    {
        HoverObstacle();
    }

    private void OnDisable()
    {
        UnsubscribeFunctions();
    }

    public void Undo()
    {
        Debug.LogWarning("Undo not implemented");
    }

    public void PerformAction(InputAction.CallbackContext context)
    {
        // Perform action should perform the desired action depending on the current Tool
        //      Tool1: Interact/None    => Obstacles/Objects that responds to interact like Bat changing states when interacted.
        //      Tool2: Destroy          => Obstacles/Objects that responds to destroy will be destroyed.
        if (IsMouseOverUI())
            return;
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit2D.collider == null || hit2D.collider.gameObject.tag != "Obstacle")
            return;
        Debug.Log("GameObject with Obstacle component found");
        IObstacle obstacle = hit2D.collider.gameObject.GetComponent<IObstacle>();
        if (obstacle == null)
        {
            Debug.LogWarning("No Obstacle component attached.");
            return;
        }
        obstacle.OnClick();
    }

    private void SetCurrentTool(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused || PlayerLevelData.Instance.character.isHome)
            return;
        switch(context.action.name)
        {
            case "Tool1":
            case "Tool2":
            case "Tool3":
            case "Tool4":
            case "Tool5":
            case "Tool6":
                SetToolType(context.action.name);
                break;
            default:
                Debug.Log($"{context.action.name} is not recognize!");
                return;
        }
    }

    public void SetToolType(string tool)
    {
        currentTool = tool;
        Debug.Log($"Current Tool: {currentTool}");
    }

    private void HoverObstacle()
    {
        if (IsMouseOverUI())
            return;
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit2D.collider == null || hit2D.collider.gameObject.tag != "Obstacle")
        {
            if (currentObstacle != null)
            {
                currentObstacle.OnDehover();
                currentObstacle = null;
            }
            return;
        }
        if (currentObstacle == null)
            currentObstacle = hit2D.collider.gameObject.GetComponent<IObstacle>();
        currentObstacle.OnHover();
    }

    private void StartCharacter(InputAction.CallbackContext context)
    {
        if (PlayerLevelData.Instance.character.isGoingHome)
            return;
        PlayerLevelData.Instance.character.InitCharacter();
    }

    private void UndoAction(InputAction.CallbackContext context)
    {
        Undo();
    }

    private void RestartLevel(InputAction.CallbackContext context)
    {
        if (context.started && !GameEvent.isPaused)
            GameEvent.RestartGame();      
    }

    private void RevealPath(InputAction.CallbackContext context)
    {
        Debug.LogWarning("Unimplemented!");
        if (GameEvent.isPaused)
            return;
        // PlayerLevelData.Instance.character.DisplayPath();
    }

    private static bool IsMouseOverUI(){
        // IsPointerOverGameobject is having a warning when used in new input system 
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void Initialize()
    {
        playerInput = GetComponent<PlayerInput>();
        currentTool = "Tool1";
        mouse = Mouse.current;
        mainCamera = Camera.main;
        Debug.Assert(playerInput != null, "GetComponent failed!");
        performAction  = playerInput.actions["PerformAction"];
        revealPath      = playerInput.actions["RevealPath"];
        undoAction      = playerInput.actions["Undo"];
        tool1           = playerInput.actions["Tool1"];
        tool2           = playerInput.actions["Tool2"];
        tool3           = playerInput.actions["Tool3"];
        tool4           = playerInput.actions["Tool4"];
        tool5           = playerInput.actions["Tool5"];
        tool6           = playerInput.actions["Tool6"];
        start           = playerInput.actions["Start"];
        reset           = playerInput.actions["Reset"];
    }

    private void SubscribeFunctions()
    {
        performAction.started  += PerformAction;
        revealPath.started      += RevealPath;
        undoAction.started      += UndoAction;
        tool1.started           += SetCurrentTool;
        tool2.started           += SetCurrentTool;
        tool3.started           += SetCurrentTool;
        tool4.started           += SetCurrentTool;
        tool5.started           += SetCurrentTool;
        tool6.started           += SetCurrentTool;
        start.started           += StartCharacter;
        reset.started           += RestartLevel;
    }

    private void UnsubscribeFunctions()
    {
        performAction.started  -= PerformAction;
        revealPath.started      -= RevealPath;
        undoAction.started      -= UndoAction;
        tool1.started           -= SetCurrentTool;
        tool2.started           -= SetCurrentTool;
        tool3.started           -= SetCurrentTool;
        tool4.started           -= SetCurrentTool;
        tool5.started           -= SetCurrentTool;
        tool6.started           -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
