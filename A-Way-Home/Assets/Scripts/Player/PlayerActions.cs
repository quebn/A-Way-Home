using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[System.Serializable]
public enum ManipulationType{ None, UniqueSkill, Pickaxe, WoodAxe, }

public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance {get; private set;}

    [HideInInspector] public ManipulationType currentManipulationType = ManipulationType.None;
    [HideInInspector] public Mouse mouse;
    private PlayerInput playerInput;

    private InputAction removeObstacle; 
    private InputAction revealPath;
    private InputAction defaultTool; 
    private InputAction uniqueSkill;
    private InputAction undoAction;
    private InputAction tool1; 
    private InputAction tool2; 
    private InputAction tool3; 
    private InputAction tool4; 
    private InputAction start;
    private InputAction reset;

    private ICharacter character;

    private void Start()
    {
        InitPlayerActions();
        SubscribeFunctions();
        if (Instance == null)
            Instance = this;
    }
        
    private void OnDisable()
    {
        UnsubscribeFunctions();
    }

    private void ClearObstacle(ManipulationType toolType, GameObject gameObject)
    {
        int moves = PlayerLevelData.Instance.levelData.moves; 
        if (moves == 0 || IsMouseOverUI())
            return;
        ObstacleData obstacleData = gameObject.GetComponent<ObstacleData>(); 
        
        if (gameObject.tag == "Obstacle" && obstacleData.toolType == toolType)
        {
            gameObject.SetActive(false);
            // PlayerLevelData.Instance.levelData.actionList.Add(new Action(toolType, obstacleData.ID, true));
            PlayerLevelData.AddRemovedToList(toolType, obstacleData.ID, true);
            InGameUI.Instance.SetPlayerMoves(-1);
            
            character.OnClear(gameObject);
            
            Debug.Log(gameObject + " was Destroyed");
            Debug.Log("Moves Left: " + PlayerLevelData.Instance.levelData.moves);
            Debug.Log($"Added Obstacle with id of {obstacleData.ID} to Removed Obstacles Dictionary!");
        }
    }

    private void UniqueSkill(Collider2D collider2D)
    {
        if (IsMouseOverUI())
            return;
        string tag = "None"; 
        Vector3 position = mouse.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
        if (collider2D != null)
            tag = collider2D.gameObject.tag;
        character.PerformSkill(worldPos, collider2D, tag); 
    }

    private void RemoveObstacle(InputAction.CallbackContext context)
    {
        ManipulationType type = Instance.currentManipulationType;
        if (GameEvent.isPaused || PlayerLevelData.Instance.character.isHome || type == ManipulationType.None)
            return;

        Ray ray = Camera.main.ScreenPointToRay(mouse.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        
        if (type == ManipulationType.UniqueSkill)
        {
            UniqueSkill(hit2D.collider);
            return;
        }
        if (hit2D.collider != null)
            ClearObstacle(type, hit2D.collider.gameObject);
        NodeGrid.Instance.UpdateGrid();
    }

    private void UndoAction(InputAction.CallbackContext context)
    {
        Undo();
    }

    public void Undo()
    {
        PlayerLevelData data = PlayerLevelData.Instance;
        if (GameEvent.isPaused || data.levelData.actionList.Count == 0 || data.character.energy <= data.minimumEnergy)
            return;
        Action action = data.levelData.actionList[^1];
        Debug.Log($"{action.type.ToString()}");
        
        switch(action.type)
        {
            case ManipulationType.UniqueSkill:
                character.OnSkillUndo(ref action);
                break;
            default:
                OnToolUndo(ref action);
                break;
        }
        // undo should removed items from both ActionList and RemovedObstacles
        InGameUI.Instance.SetMaxEnergy(-1);
        NodeGrid.Instance.UpdateGrid();
    }

    private void OnToolUndo(ref Action action)
    {
        Debug.Assert(PlayerLevelData.Instance.levelData.removedObstacles.ContainsKey(action.obstacleID), "ERROR: ID should but is not present in removed list");
        PlayerLevelData.gameObjectList[action.obstacleID].SetActive(true);
        
        PlayerLevelData.Instance.levelData.removedObstacles.Remove(action.obstacleID);
        PlayerLevelData.Instance.levelData.actionList.Remove(action);
        
        character.OnToolUndo(action.type);
        InGameUI.Instance.SetPlayerMoves(1);
    }

    private void SetCurrentTool(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused || PlayerLevelData.Instance.character.isHome)
            return;
        switch(context.action.name)
        {
            case "Cancel":
                SetToolType(ManipulationType.None);
                break;
            case "Default":
                SetToolType(ManipulationType.None);
                break;
            case "Skill":
                currentManipulationType = ManipulationType.UniqueSkill;
                break;
            case "Tool1":
                SetToolType(ManipulationType.None);
                break;
            case "Tool2":
                SetToolType(ManipulationType.None);
                break;
            case "Tool3":
                SetToolType(ManipulationType.None);
                break;
            case "Tool4":
                SetToolType(ManipulationType.None);
                break;
            default:
                Debug.Log($"{context.action.name} is not recognize!");
                break;
        }
        Debug.Log($"Current Tool: {currentManipulationType}");
    }

    private void SetToolType(ManipulationType type)
    {
        currentManipulationType = type;
        character.OnDeselect();
    }

    private void StartCharacter(InputAction.CallbackContext context)
    {
        if (PlayerLevelData.Instance.character.isGoingHome)
            return;
        PlayerLevelData.Instance.character.InitCharacter();
    }

    private void RestartLevel(InputAction.CallbackContext context)
    {
        // GameEvent.instance.RestartGame();  
        if (context.started && !GameEvent.isPaused)
        {
            Debug.Log("Pressed R");
            GameEvent.RestartGame();      
        }
    }

    private void RevealPath(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused)
            return;
        // PlayerLevelData.Instance.character.DisplayPath();
    }

    private static bool IsMouseOverUI(){
        // IsPointerOverGameobject is having a warning when used in new input system 
        return EventSystem.current.IsPointerOverGameObject();
    }


    private void InitPlayerActions()
    {
        playerInput = GetComponent<PlayerInput>();
        mouse = Mouse.current;
        character = (ICharacter)PlayerLevelData.Instance.character;
        Debug.Assert(playerInput != null, "GetComponent failed!");
        removeObstacle  = playerInput.actions["Remove"];
        revealPath      = playerInput.actions["RevealPath"];
        defaultTool     = playerInput.actions["Default"];
        uniqueSkill     = playerInput.actions["Skill"];
        undoAction      = playerInput.actions["Undo"];
        tool1           = playerInput.actions["Tool1"];
        tool2           = playerInput.actions["Tool2"];
        tool3           = playerInput.actions["Tool3"];
        tool4           = playerInput.actions["Tool4"];
        start           = playerInput.actions["Start"];
        reset           = playerInput.actions["Reset"];
    }

    private void SubscribeFunctions()
    {
        removeObstacle.started  += RemoveObstacle;
        revealPath.started      += RevealPath;
        defaultTool.started     += SetCurrentTool;
        uniqueSkill.started     += SetCurrentTool;
        undoAction.started      += UndoAction;
        tool1.started           += SetCurrentTool;
        tool2.started           += SetCurrentTool;
        tool3.started           += SetCurrentTool;
        tool4.started           += SetCurrentTool;
        start.started           += StartCharacter;
        reset.started           += RestartLevel;
    }

    private void UnsubscribeFunctions()
    {
        removeObstacle.started  -= RemoveObstacle;
        revealPath.started      -= RevealPath;
        defaultTool.started     -= SetCurrentTool;
        uniqueSkill.started     -= SetCurrentTool;
        undoAction.started      -= UndoAction;
        tool1.started           -= SetCurrentTool;
        tool2.started           -= SetCurrentTool;
        tool3.started           -= SetCurrentTool;
        tool4.started           -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
