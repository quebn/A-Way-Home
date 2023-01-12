using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance { get; private set; }

    [SerializeField] private Animator lightningAnimator;
    [HideInInspector] public Tool currentTool;
    [HideInInspector] public Mouse mouse;
    
    private Camera mainCamera;
    private IInteractable currentInteractable;
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


    private bool actionsNotAllowed => ( IsMouseOverUI() ||  PlayerLevelData.Instance.character.isGoingHome);

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
        if (actionsNotAllowed || PlayerLevelData.Instance.levelData.moves <= 0)
            return;
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit2D.collider == null || hit2D.collider.gameObject.tag != Obstacle.TAG)
            return;
        Debug.Log("Interactable obstacle found");
        IInteractable interactable = hit2D.collider.gameObject.GetComponent<IInteractable>();
        if (interactable == null)
        {
            Debug.LogWarning("No interactable obstacle found.");
            return;
        }
        OnClick(hit2D.transform.position, interactable);
    }

    private void OnClick(Vector2 location, IInteractable interactable)
    {
        switch(currentTool)
        {
            case Tool.Lightning:
                lightningAnimator.transform.position = location;
                lightningAnimator.Play("Lightning_Strike");
                break;
        }
        interactable.OnClick();
    }

    private void SetCurrentTool(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused || PlayerLevelData.Instance.character.destinationReached)
            return;
        switch(context.action.name)
        {
            case "Tool1":
                SetToolType(0);
                break;
            case "Tool2":
                SetToolType(1);
                break;
            case "Tool3":
                SetToolType(2);
                break;
            case "Tool4":
                SetToolType(3);
                break;
            case "Tool5":
                SetToolType(4);
                break;
            case "Tool6":
                SetToolType(5);
                break;
            default:
                Debug.Log($"{context.action.name} is not recognize!");
                return;
        }
    }

    public void SetToolType(int index)
    {
        if (index > 5)
            return;
        currentTool = (Tool)index;
        Debug.Log($"Current Tool: {currentTool}");
    }

    private void HoverObstacle()
    {
        if (actionsNotAllowed)
            return;
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit2D.collider == null || hit2D.collider.gameObject.tag != Obstacle.TAG)
        {
            if (currentInteractable != null)
            {
                currentInteractable.OnDehover();
                currentInteractable = null;
            }
            return;
        }
        if (currentInteractable == null)
            currentInteractable = hit2D.collider.gameObject.GetComponent<IInteractable>();
        currentInteractable.OnHover();
    }

    private void StartCharacter(InputAction.CallbackContext context)
    {
        if (PlayerLevelData.Instance.character.isGoingHome)
            return;
        PlayerLevelData.Instance.character.GoHome();
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
        currentTool = Tool.Inspect;
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
