using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance { get; private set; }

    [SerializeField] private Animator animatorLighting;
    [SerializeField] private Animator animatorExplosion;
    [SerializeField] private int toolCount;
    [SerializeField] private List<Texture2D> mouseTextures;
    [HideInInspector] public Tool currentTool;

    private Mouse mouse;
    private Camera mainCamera;
    private PlayerInput playerInput;
    private InputAction performAction; 
    private InputAction revealPath;
    private InputAction undoAction;
    private List<InputAction> tools;
    private InputAction start;
    private InputAction reset;
    private Vector2 currentTileOrigin;
    private List<Node> currentTileNodes;
    private List<IInteractable> currentInteractables;

    public Vector3 mouseWorldPos => mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
    private bool actionsNotAllowed => ( IsMouseOverUI() ||  Character.instance.isMoving);
    // private bool uninteractable => this.currentTool == Tool.PlaceMode;

    private void Start()
    {
        Initialize();
        SubscribeFunctions();
        if (Instance == null)
            Instance = this;
    }

    private void FixedUpdate()
    {
        Hover();
    }

    private void OnDisable()
    {
        UnsubscribeFunctions();
    }

    public void Undo()
    {
        Debug.Assert(false, "ERROR: Should not be called!");
        // if(Character.instance.isMoving || actionList.Count < 1)
        //     return;
        // ActionData data = actionList.Last<ActionData>();
        // // data.GetObstacle().OnUndo(data);
        // actionList.Remove(data);
        // Debug.Log("Undo Action was Pressed!");
        // GameData.IncrementPlayerMoves(1);
        // Character.instance.GetPath();
        // add a penalty reducing the time by n amount every time player undo an action.
    }

    public void PerformAction(InputAction.CallbackContext context)
    {
        if (actionsNotAllowed || GameData.levelData.moves <= 0)
            return;
        switch(currentTool)
        {
            // case Tool.Inspect:
                // Inspect();
                // break;
            case Tool.Lightning:
                LightningAnimation(this.currentTileOrigin);
                InteractNodes();
                GameData.IncrementPlayerMoves(-1);
                break;
            case Tool.Tremor:
                InteractNodes();
                break;
            case Tool.Grow:
                InteractObject();
                break;
            case Tool.Command:
                InteractObject();
                break;
        }
        Character.instance.GetPath();
    }

    private void InteractNodes()
    {
        Node.TriggerNodesObstacle(currentTileNodes);
    }

    // private void Place()
    // {

    // }

    private void InteractObject()
    {
        if(currentInteractables.Count < 1)
            return;
        currentInteractables[0].OnInteract();
        // currentInteractable = hit2D.collider.gameObject.GetComponent<IInteractable>();
        // if (currentInteractable == null)
            // return;
        // actionList.Add(new ActionData((Obstacle)currentInteractable, currentTool));
    }

    private float LightningAnimation(Vector2 location)
    {
        animatorLighting.transform.position = location;
        animatorLighting.Play("Lightning_Strike");
        animatorExplosion.Play("SmallExplosion_Destroy");
        return animatorLighting.GetNextAnimatorStateInfo(0).length;
    }

    private void SetCurrentTool(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused || Character.instance.destinationReached)
            return;
        int toolNumber = context.action.name[context.action.name.Length - 1] - '0';
        SetCurrentTool(toolNumber - 1);
    }

    public void SetCurrentTool(int index)
    {
        Tool newTool = (Tool)index;
        if (index > 5 || index < 0)
            return;
        if(currentTool != newTool)
            OnChangeTool();
        currentTool = newTool;
        Cursor.SetCursor(mouseTextures[index], Vector2.zero, CursorMode.Auto);

    }

    private void OnChangeTool()
    {
        switch(currentTool)
        {
            case Tool.Lightning:
            case Tool.Tremor:
                currentTileOrigin = new Vector2();
                Obstacle.DehighlightInteractables(currentInteractables);
                Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility, Character.instance);
                currentInteractables = new List<IInteractable>();
                currentTileNodes = new List<Node>();
                break;
        }
        Debug.Log("Change");
    }

    private void Hover()
    {
        if(actionsNotAllowed)
            return;
        switch(currentTool)
        {
            case Tool.Lightning:
                HighlightTile(1, 1, Node.colorCyan);
                break;
            case Tool.Command:
            case Tool.Grow:
                HighlightObject();
                break;
            case Tool.Tremor:
                HighlightTile(2, 2, Node.colorYellow);
                break;
        }
    }

    private void HighlightObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit2D.collider == null || hit2D.collider.gameObject.tag != Obstacle.TAG)
        {
            if (currentInteractables.Count > 0)
            {
                currentInteractables[0].OnDehighlight();
                currentInteractables = new List<IInteractable>();
            }
            return;
        }
        if(currentInteractables.Count < 1)
            currentInteractables.Add(hit2D.collider.gameObject.GetComponent<IInteractable>());
        currentInteractables[0].OnHighlight();
        // Debug.Log("Hovering on Object");
    }

    private void HighlightTile(int tileWidth, int tileHeight, Color color)
    {
        Vector2 origin = NodeGrid.GetMiddle(mouseWorldPos, tileWidth, tileHeight);
        if(currentTileOrigin == origin)
            return;
        Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility, Character.instance);
        currentTileNodes = NodeGrid.GetNodes(origin, tileWidth, tileHeight, NodeType.Terrain);
        HiglightInteractables(origin);
        currentTileOrigin = origin;
        Node.RevealNodes(currentTileNodes, color);
    }

    private void HiglightInteractables(Vector2 origin)
    {
        List<IInteractable> interactables = Node.GetNodesInteractable(currentTileNodes);
        if(interactables == currentInteractables)
            return;
        if(origin != currentTileOrigin)
            Obstacle.DehighlightInteractables(currentInteractables);
        currentInteractables = interactables;
        Obstacle.HighlightInteractables(currentInteractables);
    } 

    // private void HighlightObject()
    // {
    //     Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
    //     RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
    //     if (hit2D.collider == null || hit2D.collider.gameObject.tag != Obstacle.TAG)
    //     {
    //         if (currentInteractable != null)
    //         {
    //             currentInteractable.OnDehover();
    //             currentInteractable = null;
    //         }
    //         return;
    //     }
    //     if (currentInteractable == null)
    //         currentInteractable = hit2D.collider.gameObject.GetComponent<IInteractable>();
    //     currentInteractable.OnHover();
    // }

    private void StartCharacter(InputAction.CallbackContext context)
    {
        if (Character.instance.isMoving)
            return;
        Character.instance.GoHome();
    }

    private void UndoAction(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused)
            return;
        Undo();
    }

    private void RestartLevel(InputAction.CallbackContext context)
    {
        if (context.started && !GameEvent.isPaused)
            GameEvent.RestartGame();      
    }

    private void RevealPath(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused)
            return;
        Debug.LogWarning("Unimplemented!");
        // Character.instance.DisplayPath();
    }

    private static bool IsMouseOverUI(){
        // IsPointerOverGameobject is having a warning when used in new input system 
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void Initialize()
    {
        tools = new List<InputAction>(toolCount);
        playerInput = GetComponent<PlayerInput>();
        currentTool = Tool.Inspect;
        mouse = Mouse.current;
        mainCamera = Camera.main;
        Debug.Assert(playerInput != null, "playerInput GetComponent failed!");
        performAction  = playerInput.actions["PerformAction"];
        revealPath      = playerInput.actions["RevealPath"];
        undoAction      = playerInput.actions["Undo"];
        for(int i = 1; i <= toolCount; i++)
            tools.Add(playerInput.actions[$"Tool{i}"]);
        start           = playerInput.actions["Start"];
        reset           = playerInput.actions["Reset"];
        currentTileNodes = new List<Node>(4); 
        // actionList = new List<ActionData>();
        currentInteractables = new List<IInteractable>();
    }

    private void SubscribeFunctions()
    {
        // Debug.LogWarning("Subscribing Functions");
        performAction.started  += PerformAction;
        revealPath.started      += RevealPath;
        undoAction.started      += UndoAction;
        foreach(InputAction tool in tools)
            tool.started += SetCurrentTool;
        start.started           += StartCharacter;
        reset.started           += RestartLevel;
    }

    private void UnsubscribeFunctions()
    {
        // Debug.LogWarning("Unsubscribing Functions");
        performAction.started  -= PerformAction;
        revealPath.started      -= RevealPath;
        undoAction.started      -= UndoAction;
        foreach(InputAction tool in tools)
            tool.started -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
