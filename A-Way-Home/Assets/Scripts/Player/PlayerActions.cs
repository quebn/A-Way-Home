using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance { get; private set; }

    [SerializeField] private Animator animatorTool;
    [SerializeField] private Animator animatorExplosion;
    [SerializeField] private int toolCount;
    [SerializeField] private List<Texture2D> mouseTextures;

    private Tool currentTool;
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
    private List<Obstacle> currentObstacles;
    private IHoverable hoveredObstacle;

    public Vector3 mouseWorldPos => mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
    public static List<IOnPlayerAction> onPlayerActions;

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
    }

    public void PerformAction(InputAction.CallbackContext context)
    {
        if (ActionsNotAllowed() || currentTileNodes.Count == 0)
            return;
        switch(currentTool)
        {
            case Tool.Inspect:
                currentTileNodes[0].InspectObstacle();
                return;
            case Tool.Lightning:
                Node.ShockNode(currentTileNodes[0]);
                LightningAnimation(this.currentTileOrigin);
                break;
            case Tool.Tremor:
                Node.TremorNodes(currentTileNodes);
                break;
            case Tool.Grow:
                GrowAnimation(this.currentTileOrigin);
                currentTileNodes[0].GrowObstacle();
                break;
            case Tool.Command:
                currentTileNodes[0].CommandObstacle();
                break;
        }
        GameData.IncrementPlayerMoves(-1);
        UpdateOnActionObstacles();
        Character.instance.GetPath();
    }

    private void UpdateOnActionObstacles()
    {
        // List<IOnPlayerAction> onPlayerActions = new List<IOnPlayerAction>(FindObjectsOfType<MonoBehaviour>(true).OfType<IOnPlayerAction>());
        if(onPlayerActions == null||onPlayerActions.Count == 0)
            return;
        foreach(IOnPlayerAction obstacle in onPlayerActions)
            obstacle.OnPerformAction();
    }

    private bool ActionsNotAllowed()
    {
        return  IsMouseOverUI() || 
        GameData.levelData.moves == 0 ||
        Character.instance.isMoving;
    }

    // private void Place()
    // {

    // }

    private float LightningAnimation(Vector2 location)
    {
        animatorTool.transform.position = location;
        animatorTool.Play("Lightning_Strike");
        animatorExplosion.Play("SmallExplosion_Destroy");
        return animatorTool.GetNextAnimatorStateInfo(0).length;
    }

    private void GrowAnimation(Vector2 location)
    {
        animatorTool.transform.position = location;
        animatorTool.Play("Grow");
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
        if (index > 5 || index < 0 || index > PlayerLevelData.Instance.unlockedTools)
            return;
        Tool newTool = (Tool)index;
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
            case Tool.Grow:
            case Tool.Command:
            case Tool.Tremor:
                currentTileOrigin = new Vector2();
                Obstacle.DehighlightObstacles(currentObstacles, currentTool);
                Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility, Character.instance);
                currentObstacles = new List<Obstacle>();
                currentTileNodes = new List<Node>();
                break;
        }
        Debug.Log("Change");
    }



    private void Hover()
    {
        if(ActionsNotAllowed())
            return;
        OnHoverObstacle();
        switch(currentTool)
        {
            case Tool.Lightning:
                HighlightTile(1, 1, Node.colorCyan);
                break;
            case Tool.Command:
                HighlightTile(1, 1, Node.colorPurple);
                break;
            case Tool.Grow:
                HighlightTile(1, 1, Node.colorGreen);
                break;
            case Tool.Tremor:
                HighlightTile(2, 2, Node.colorYellow);
                break;
        }
    }

    private void OnHoverObstacle()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if(hit2D.collider == null) //|| hit2D.collider.gameObject.tag != Obstacle.TAG)
        {
            if(hoveredObstacle != null)
            {
                hoveredObstacle.OnDehover();
                hoveredObstacle = null;
            }
            return;
        }
        hoveredObstacle = hit2D.collider.gameObject.GetComponent<Obstacle>() as IHoverable;
        if(hoveredObstacle == null)
            return;
        hoveredObstacle.OnHover();
    }

    private void HighlightObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit2D.collider == null || hit2D.collider.gameObject.tag != Obstacle.TAG)
        {
            if (currentObstacles.Count > 0)
            {
                currentObstacles[0].Dehighlight(currentTool);
                currentObstacles = new List<Obstacle>();
            }
            return;
        }
        if(currentObstacles.Count < 1)
            currentObstacles.Add(hit2D.collider.gameObject.GetComponent<Obstacle>());
        currentObstacles[0].Highlight(currentTool);
        // Debug.Log("Hovering on Object");
    }

    private void HighlightTile(int tileWidth, int tileHeight, Color color)
    {
        Vector2 origin = NodeGrid.GetMiddle(mouseWorldPos, tileWidth, tileHeight);
        if(currentTileOrigin == origin)
            return;
        Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility, Character.instance);
        currentTileNodes = NodeGrid.GetNodes(origin, tileWidth, tileHeight, NodeType.Terrain);
        HiglightObstacles(origin);
        currentTileOrigin = origin;
        Node.RevealNodes(currentTileNodes, color);
    }

    private void HiglightObstacles(Vector2 origin)
    {
        List<Obstacle> interactables = Node.GetNodesInteractable(currentTileNodes);
        if(interactables == currentObstacles)
            return;
        if(origin != currentTileOrigin)
            Obstacle.DehighlightObstacles(currentObstacles, currentTool);
        currentObstacles = interactables;
        Obstacle.HighlightObstacles(currentObstacles, currentTool);
    } 

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
        currentObstacles = new List<Obstacle>();
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
