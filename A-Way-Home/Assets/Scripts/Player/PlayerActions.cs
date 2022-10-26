using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

public enum ManipulationType{ None, UniqueSkill, Pickaxe, WoodAxe, }

public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance {get; private set;}
    
    [HideInInspector] public ManipulationType currentManipulationType = ManipulationType.None;
    private PlayerInput playerInput;
    
    private InputAction removeObstacle; 
    private InputAction revealPath;
    private InputAction defaultTool; 
    private InputAction uniqueSkill;
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
        uint moves = PlayerLevelData.Instance.levelData.moves; 
        if (moves == 0 || IsMouseOverUI())
            return;

        ManipulationType type = gameObject.GetComponent<ObstacleData>().toolType;
        if (gameObject.tag == "Obstacle" && type == toolType)
        {
            gameObject.SetActive(false);
            string obstacleID = gameObject.GetComponent<ObstacleData>().ID;
            PlayerLevelData.Instance.levelData.removedObstacles.Add(obstacleID, true);
            PlayerLevelData.Instance.levelData.moves--;

            InGameUI.Instance.SetPlayerMoves();
            
            character.OnClear(gameObject);
            
            Debug.Log(gameObject + " was Destroyed");
            Debug.Log("Moves Left: " + PlayerLevelData.Instance.levelData.moves);
            Debug.Log($"Added Obstacle with id of {obstacleID} to Removed Obstacles Dictionary!");
        }
    }

    private void UniqueSkill()
    {
        if (IsMouseOverUI())
            return;
        Vector3 position = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);
        character.PerformSkill(worldPos); 
    }

    private void RemoveObstacle(InputAction.CallbackContext context)
    {
        ManipulationType type = Instance.currentManipulationType;
        if (GameEvent.isPaused || PlayerLevelData.Instance.character.isHome || type == ManipulationType.None)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
        
        if (type == ManipulationType.UniqueSkill)
        {
            // Checks and returns if theres is an obstacle when placing the skill 
            if (hit2D.collider == null || hit2D.collider.gameObject.tag != "Obstacle")
                UniqueSkill();
            return;
        }
        if (hit2D.collider != null)
            ClearObstacle(type, hit2D.collider.gameObject);
        NodeGrid.Instance.UpdateGrid();
    }

    private void UndoAction(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused)
            return;
        Debug.Assert(false, "To be implemented");
        //TODO: PlayerLevelData.Instance.character.DecrementEnergy();
        NodeGrid.Instance.UpdateGrid();
    }
    private void SetCurrentTool(InputAction.CallbackContext context)
    {
        if (GameEvent.isPaused || PlayerLevelData.Instance.character.isHome)
            return;
        switch(context.action.name)
        {
            case "Cancel":
                currentManipulationType = ManipulationType.None;
                break;
            case "Default":
                currentManipulationType = ManipulationType.None;
                break;
            case "Skill":
                currentManipulationType = ManipulationType.UniqueSkill;
                break;
            case "Tool1":
                currentManipulationType = ManipulationType.Pickaxe;
                break;
            case "Tool2":
                currentManipulationType = ManipulationType.WoodAxe;
                break;
            case "Tool3":
                currentManipulationType = ManipulationType.None;
                break;
            case "Tool4":
                currentManipulationType = ManipulationType.None;
                break;
            default:
                Debug.Log($"{context.action.name} is not recognize!");
                break;
        }
        Debug.Log($"Current Tool: {currentManipulationType}");
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
        PlayerLevelData.Instance.character.DisplayPath();
    }



    private static bool IsMouseOverUI(){
        // IsPointerOverGameobject is having a warning when used in new input system 
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void InitPlayerActions()
    {
        playerInput = GetComponent<PlayerInput>();
        character = (ICharacter)PlayerLevelData.Instance.character;
        Debug.Assert(playerInput != null, "GetComponent failed!");
        removeObstacle  = playerInput.actions["Remove"];
        revealPath      = playerInput.actions["RevealPath"];
        defaultTool     = playerInput.actions["Default"];
        uniqueSkill     = playerInput.actions["Skill"];
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
        tool1.started           -= SetCurrentTool;
        tool2.started           -= SetCurrentTool;
        tool3.started           -= SetCurrentTool;
        tool4.started           -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
