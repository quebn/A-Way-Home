using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

[System.Serializable]
public enum Tool { Inspect, Lightning, Grow, Command, Tremor, Select}//, PlaceMode }

public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance { get; private set; }

    [SerializeField] private Animator animatorTool;
    [SerializeField] private Animator animatorExplosion;
    [SerializeField] private int toolCount;
    [SerializeField] private List<Texture2D> mouseTextures;
    [SerializeField] private GameObject lilypadVisual;

    private Tool currentTool;
    private Mouse mouse;
    private Camera mainCamera;
    private PlayerInput playerInput;
    private InputAction performAction; 
    private InputAction revealPath;
    private List<InputAction> tools;
    private InputAction start;
    private InputAction reset;
    private Vector2 currentTileOrigin;
    private List<Node> currentTileNodes;
    private List<Obstacle> currentObstacles;
    private Obstacle selectedObstacle;
    private IHoverable hoverable;
    private bool obstaclesDone = true;
    private GameObject lilypad;

    public static bool finishedProcessing => Instance.obstaclesDone;
    public Vector3 mouseWorldPos => mainCamera.ScreenToWorldPoint(mouse.position.ReadValue());
    private static HashSet<IActionWaitProcess> actionWaitProcesses;
    private static Queue<IActionWaitProcess> finishedProcesses;

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


    public void PerformAction(InputAction.CallbackContext context)
    {
        if (ActionsNotAllowed() || currentTileNodes.Count == 0)
            return;
        obstaclesDone = false;
        switch(currentTool)
        {
            case Tool.Inspect:
                currentTileNodes[0].InspectObstacle();
                if(currentTileNodes.Contains(Character.instance.currentNode))
                    Character.instance.Interact();
                obstaclesDone = true;
                return;
            case Tool.Lightning:
                Node.ShockNode(currentTileNodes[0]);
                LightningAnimation(this.currentTileOrigin);
                break;
            case Tool.Tremor:
                Node.TremorNodes(currentTileNodes);
                break;
            case Tool.Grow:
                Grow();
                break;
            case Tool.Command:
                Command();
                return;
        }
        GameData.IncrementPlayerMoves(-1);
        ProcessObstaclesAction();
        StartCoroutine(WaitForObstaclesAction());
        if(GameData.levelData.moves == 0)
            Dehighlight();
    }

    private void Command()
    {
        bool hasCommand = currentTileNodes[0].CommandObstacle();
        Debug.Assert(currentObstacles.Count == 1);
        selectedObstacle = currentObstacles[0];
        obstaclesDone = true;
    }

    private void Grow()
    {
        GrowAnimation(this.currentTileOrigin);
        if(currentTileNodes[0].currentType == NodeType.Water)
            GameObject.Instantiate(lilypad, currentTileNodes[0].worldPosition, Quaternion.identity);
        else
            currentTileNodes[0].GrowObstacle();
    }

    private IEnumerator WaitForObstaclesAction()
    {
        // Pause Timer
        while(!obstaclesDone)
        {
            if(actionWaitProcesses.Count == 0)
            {
                actionWaitProcesses.Clear();
                obstaclesDone = true;
                Character.instance.GetPath();
                // Resume Timer
                yield break;
            }
            Debug.Log($"Unfinished processes: {actionWaitProcesses.Count}");
            Debug.Log($"Waiting to be finished: {finishedProcesses.Count}");
            while(finishedProcesses.Count > 0)
                actionWaitProcesses.Remove(finishedProcesses.Dequeue());
            yield return null;
        }
    }

    private HashSet<IActionWaitProcess> FetchAllProcess()
    {
        IEnumerable<IActionWaitProcess> saveables = FindObjectsOfType<MonoBehaviour>(false).OfType<IActionWaitProcess>();
        return new HashSet<IActionWaitProcess>(saveables);
    }

    private void ProcessObstaclesAction()
    {
        finishedProcesses = new Queue<IActionWaitProcess>();
        actionWaitProcesses = FetchAllProcess();
        if(actionWaitProcesses == null||actionWaitProcesses.Count == 0)
        {
            obstaclesDone = true;
            return;
        }
        foreach(IActionWaitProcess process in actionWaitProcesses)
            process.OnPlayerAction();
    }

    public static void FinishProcess(IActionWaitProcess process)
    {
        if(actionWaitProcesses == null || !actionWaitProcesses.Contains(process))
            return;
        finishedProcesses.Enqueue(process);
        Debug.Log($"Queueing process as finished: {(process as Obstacle).gameObject.name}");
    }

    private bool ActionsNotAllowed()
    {
        return  IsMouseOverUI() || 
        GameData.levelData.moves == 0 ||
        Character.instance.isMoving || 
        !obstaclesDone;
    }

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
            Dehighlight();
        currentTool = newTool;
        Cursor.SetCursor(mouseTextures[index], Vector2.zero, CursorMode.Auto);
    }

    private void Dehighlight()
    {
        if(lilypadVisual.activeSelf)
            lilypadVisual.SetActive(false);
        currentTileOrigin = new Vector2();
        Obstacle.DehighlightObstacles(currentObstacles, currentTool);
        Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility);
        currentObstacles = new List<Obstacle>();
        currentTileNodes = new List<Node>();
    }

    private void Hover()
    {
        if(ActionsNotAllowed())
            return;
        OnHoverUpper();
        switch(currentTool)
        {
            case Tool.Inspect:
                InspectTile(1, 1);
                break;
            case Tool.Lightning:
                HighlightTile(1, 1, Node.colorCyan, true);
                break;
            case Tool.Command:
                HighlightTile(1, 1, Node.colorPurple);
                break;
            case Tool.Grow:
                HighlightTile(1, 1, Node.colorGreen);
                OnWaterHover();
                break;
            case Tool.Tremor:
                HighlightTile(2, 2, Node.colorYellow);
                break;
        }
        HighlightObtacleOthers();
        Character.instance.CanHighlight(currentTileNodes.Contains(Character.instance.currentNode));
    }

    private void HighlightObtacleOthers()
    {
        if(currentObstacles == null || currentObstacles.Count == 0)
            return;
        for(int i = 0; i < currentObstacles.Count; i++)
            currentObstacles[i].WhileHighlight(currentTool);
    }

    private void OnWaterHover()
    {
        if(currentTileNodes.Count != 0 &&currentTileNodes[0].currentType == NodeType.Water)
        {
            lilypadVisual.SetActive(true);
            lilypadVisual.transform.position = currentTileOrigin;
            Debug.Log("Hovering On water");
        }
        else lilypadVisual.SetActive(false);

    }

    private void OnHoverUpper()
    {
        Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        if(hit2D.collider == null || hit2D.collider.gameObject.tag != "Hoverable")
        {
            if(hoverable != null)
            {
                hoverable.OnDehover();
                hoverable = null;
            }
            return;
        }
        IHoverable newHovered = hit2D.collider.gameObject.GetComponent<IHoverable>(); 
        if(hoverable != null && newHovered != hoverable)
            hoverable.OnDehover();
        hoverable = hit2D.collider.gameObject.GetComponent<IHoverable>();
        hoverable.OnHover();
    }

    private void HighlightTile(int tileWidth, int tileHeight, Color color, bool isOpenOnly = false)
    {
        if(selectedObstacle != null)
            return;
        Vector2 origin = NodeGrid.GetMiddle(mouseWorldPos, tileWidth, tileHeight);
        if(currentTileOrigin == origin)
            return;
        Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility);
        currentTileNodes = NodeGrid.GetNodes(origin, tileWidth, tileHeight, NodeType.Terrain, isOpenOnly);
        HiglightObstacles(origin);
        currentTileOrigin = origin;
        Node.RevealNodes(currentTileNodes, color);
    }

    private void InspectTile(int tileWidth, int tileHeight)
    {
        Vector2 origin = NodeGrid.GetMiddle(mouseWorldPos, tileWidth, tileHeight);
        if(currentTileOrigin == origin)
            return;
        Node.ToggleNodes(currentTileNodes, NodeGrid.nodesVisibility);
        currentTileNodes = NodeGrid.GetNodes(origin, tileWidth, tileHeight, NodeType.Terrain);
        HiglightObstacles(origin);
        currentTileOrigin = origin;
        Node.ToggleNodes(currentTileNodes, NodeGrid.Instance.isActiveAndEnabled);
    }

    private void HiglightObstacles(Vector2 origin)
    {
        List<Obstacle> interactables = Node.GetNodesInteractable(currentTileNodes);
        if(interactables.Count == 0)
            interactables = Node.GetNodesInteractable(currentTileNodes, true);
        if(interactables == currentObstacles)
            return;
        if(origin != currentTileOrigin)
            Obstacle.DehighlightObstacles(currentObstacles, currentTool);
        currentObstacles = interactables;
        Obstacle.HighlightObstacles(currentObstacles, currentTool);
    } 

    private void StartCharacter(InputAction.CallbackContext context)
    {
        if (Character.instance.isMoving || !obstaclesDone)
            return;
        Character.instance.GoHome();
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
        for(int i = 1; i <= toolCount; i++)
            tools.Add(playerInput.actions[$"Tool{i}"]);
        start           = playerInput.actions["Start"];
        reset           = playerInput.actions["Reset"];
        currentTileNodes = new List<Node>(4); 
        // actionList = new List<ActionData>();
        currentObstacles = new List<Obstacle>();
        lilypad = Resources.Load<GameObject>($"Spawnables/Lilypad");
        Debug.Assert(lilypad != null);
    }

    private void SubscribeFunctions()
    {
        // Debug.LogWarning("Subscribing Functions");
        performAction.started  += PerformAction;
        revealPath.started      += RevealPath;
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
        foreach(InputAction tool in tools)
            tool.started -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
