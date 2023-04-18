using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

[System.Serializable]
public enum Tool { Inspect, Lightning, Grow, Command, Tremor}//, PlaceMode }

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
    private InputAction cancelAction;
    private InputAction revealPath;
    private List<InputAction> tools;
    private InputAction start;
    private InputAction reset;
    // 
    // 
    private List<Node> hoveredNodes;
    private Obstacle selectedObstacle;

    private IHoverable hoverable;
    private bool obstaclesDone = true;
    private GameObject lilypad;

    public bool hasSelectedObs => selectedObstacle != null;
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

    private void Update()
    {
        Hover();
    }

    private void OnDisable()
    {
        UnsubscribeFunctions();
    }


    public void PerformAction(InputAction.CallbackContext context)
    {
        if (ActionsNotAllowed() || hoveredNodes.Count == 0)
            return;
        // selectedObstacle = Node.GetObstaclesByTool(hoveredNodes, currentTool);
        switch(currentTool)
        {
            case Tool.Inspect:
                Inspect();
                return;
            case Tool.Lightning:
                Lightning();
                break;
            case Tool.Tremor:
                Node.TremorNodes(hoveredNodes);
                break;
            case Tool.Grow:
                Grow();
                break;
            case Tool.Command:
                if (Command())
                    break;
                return;
        }
        obstaclesDone = false;
        GameData.IncrementPlayerMoves(-1);
        ProcessObstaclesAction();
        StartCoroutine(WaitForObstaclesAction());
        if(GameData.levelData.moves == 0)
            NodeGrid.DehighlightNodes(hoveredNodes);
    }


    private bool Command()
    {
        if(hasSelectedObs)
        {
            ICommand obstacle = selectedObstacle as ICommand;
            bool success = obstacle.OnCommand(hoveredNodes);
            if(success)
                selectedObstacle = null;
            return success;
        }
        Debug.Assert(currentTool == Tool.Command, $"ERROR: Unexpected tool selected :{currentTool.ToString()} -> expected: Tool.Command");
        selectedObstacle = hoveredNodes[0].GetObstacleByTool(currentTool);
        if(hasSelectedObs)
            selectedObstacle.OnSelect(currentTool);
        return false;
    }

    private void Lightning()
    {
        Node.ShockNode(hoveredNodes[0]);
        LightningAnimation(hoveredNodes[0].worldPosition);
    }

    public void CancelAction(InputAction.CallbackContext context)
    {
        selectedObstacle = null;
        SetCurrentTool(0);
        Debug.Log("Canceled!");
        // if(selectedObstacle != null)
        //     selectedObstacle = null;
    }    

    private void Inspect()
    {
        hoveredNodes[0].InspectObstacle();
        if(hoveredNodes.Contains(Character.instance.currentNode))
            Character.instance.Interact();
    }


    private void Grow()
    {
        GrowAnimation(this.hoveredNodes[0].worldPosition);
        if(hoveredNodes[0].currentType == NodeType.Water)
            GameObject.Instantiate(lilypad, hoveredNodes[0].worldPosition, Quaternion.identity);
        else
            hoveredNodes[0].GrowObstacle();
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
        if(currentTool == newTool)
            return;
        if(lilypadVisual.activeSelf)
            lilypadVisual.SetActive(false);
        NodeGrid.DehighlightNodes(hoveredNodes);
        // hoveredNodes = new List<Node>();
        if(hasSelectedObs)
            selectedObstacle = null;
        currentTool = newTool;
        hoveredNodes = NodeGrid.HighlightGetNodes(mouseWorldPos, hoveredNodes, currentTool);
        Cursor.SetCursor(mouseTextures[index], Vector2.zero, CursorMode.Auto);
    }

    private void Hover()
    {
        if(ActionsNotAllowed())
            return;
        // Debug.LogWarning("Returned");
        OnHoverUpper();
        hoveredNodes = NodeGrid.HighlightGetNodes(mouseWorldPos, hoveredNodes, currentTool);
        if(currentTool == Tool.Grow)
            OnWaterHover();
        WhileHoverObstacle();
        Character.instance.CanHighlight(hoveredNodes.Contains(Character.instance.currentNode));
    }

    public static Color GetToolColor(Tool tool)
    {
        switch(tool)
        {
            case Tool.Lightning:
                return Node.colorCyan;
            case Tool.Command:
                return Node.colorPurple;
            case Tool.Grow:
                return Node.colorGreen;
            case Tool.Tremor:
                return Node.colorYellow;
            default:
                return Node.colorClear;
        }
    }

    private void WhileHoverObstacle()
    {
        if(hoveredNodes == null || hoveredNodes.Count == 0)
            return;
        // for(int i = 0; i < hoveredNodes.Count; i++)
        //     hoveredNodes[i].WhileHighlight(currentTool);
    }

    private void OnWaterHover()
    {
        if(hoveredNodes.Count != 0 && hoveredNodes[0].currentType == NodeType.Water)
        {
            lilypadVisual.SetActive(true);
            lilypadVisual.transform.position = hoveredNodes[0].worldPosition;
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


    private void StartCharacter(InputAction.CallbackContext context)
    {
        if (Character.instance.isMoving || !obstaclesDone)
            return;
        Character.instance.GoHome();
        NodeGrid.DehighlightNodes(hoveredNodes);
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

    public static bool IsMouseOverUI(){
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
        cancelAction  = playerInput.actions["cancelAction"];
        revealPath      = playerInput.actions["RevealPath"];
        for(int i = 1; i <= toolCount; i++)
            tools.Add(playerInput.actions[$"Tool{i}"]);
        start           = playerInput.actions["Start"];
        reset           = playerInput.actions["Reset"];
        // actionList = new List<ActionData>();
        hoveredNodes = new List<Node>();
        lilypad = Resources.Load<GameObject>($"Spawnables/Lilypad");
        Debug.Assert(lilypad != null);
    }

    private void SubscribeFunctions()
    {
        // Debug.LogWarning("Subscribing Functions");
        performAction.started  += PerformAction;
        cancelAction.started += CancelAction;
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
        cancelAction.started -= CancelAction;
        revealPath.started      -= RevealPath;
        foreach(InputAction tool in tools)
            tool.started -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
