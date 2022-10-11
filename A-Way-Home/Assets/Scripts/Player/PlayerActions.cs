using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public enum ManipulationType{ None, Pickaxe, WoodAxe, }

// TODO: Rewrite this file to be a monobehaviour and have callback functions
public class PlayerActions : MonoBehaviour
{
    public static PlayerActions Instance {get; private set;}
    
    [HideInInspector] public ManipulationType currentManipulationType = ManipulationType.None;
    private PlayerInput playerInput;
    
    private InputAction removeObstacle; 
    private InputAction revealPath;
    private InputAction tool1; 
    private InputAction tool2; 
    private InputAction tool3; 
    private InputAction tool4; 
    private InputAction tool5; 
    private InputAction start;
    private InputAction reset;
    
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
    
    private void RemoveObstacle(InputAction.CallbackContext context)
    {
        if (InGameUI.Instance.isPaused || PlayerLevelData.Instance.character.isHome)
            return;
        switch(Instance.currentManipulationType)
        {
            case ManipulationType.Pickaxe:
                ClearObstacle("RockObstacle");
                break;
            case ManipulationType.WoodAxe:
                ClearObstacle("WoodObstacle");
                break;
        }
    }

    private void SetCurrentTool(InputAction.CallbackContext context)
    {
        if (InGameUI.Instance.isPaused || PlayerLevelData.Instance.character.isHome)
            return;
        switch(context.action.name)
        {
            case "Cancel":
                currentManipulationType = ManipulationType.None;
                break;
            case "Tool1":
                currentManipulationType = ManipulationType.None;
                break;
            case "Tool2":
                currentManipulationType = ManipulationType.Pickaxe;
                break;
            case "Tool3":
                currentManipulationType = ManipulationType.WoodAxe;
                break;
            case "Tool4":
                currentManipulationType = ManipulationType.None;
                break;
            case "Tool5":
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
        InGameUI.Instance.PlayAction();
    }

    private void RestartLevel(InputAction.CallbackContext context)
    {
        // GameEvent.instance.RestartGame();  
        if (context.started)
        {
            Debug.Log("Pressed R");
            GameEvent.RestartGame();      
        }
    }

    private void ClearObstacle(string obstacletag)
    {
        if (PlayerLevelData.Instance.levelData.moves == 0)
        {
            Debug.Log("No moves Left");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit2D.collider != null && hit2D.collider.gameObject.tag == obstacletag && !IsMouseOverUI())
        {
            string ObstacleID = hit2D.collider.gameObject.GetComponent<ObstacleData>().ID;
            hit2D.collider.gameObject.SetActive(false);
            PlayerLevelData.Instance.levelData.removedObstacles.Add(ObstacleID, false);//should be true not false
            PlayerLevelData.Instance.levelData.moves--;
            InGameUI.Instance.SetPlayerMoves();
            Debug.Log(hit2D.collider.gameObject + " was Destroyed");
            Debug.Log("Moves Left: " + PlayerLevelData.Instance.levelData.moves);
            Debug.Log($"Added Obstacle with id of {ObstacleID} to Removed Obstacles Dictionary!");
            
        }
    }
    
    private static bool IsMouseOverUI(){
        // IsPointerOverGameobject is having a warning when used in new input system 
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void RevealPath(InputAction.CallbackContext context)
    {
        PlayerLevelData.Instance.character.DisplayPath();
    }
    private void InitPlayerActions()
    {
        playerInput = GetComponent<PlayerInput>();
        Debug.Assert(playerInput != null, "GetComponent failed!");
        removeObstacle = playerInput.actions["Remove"];
        revealPath = playerInput.actions["RevealPath"];
        tool1 = playerInput.actions["Tool1"];
        tool2 = playerInput.actions["Tool2"];
        tool3 = playerInput.actions["Tool3"];
        tool4 = playerInput.actions["Tool4"];
        tool5 = playerInput.actions["Tool5"];
        start = playerInput.actions["Start"];
        reset = playerInput.actions["Reset"];
    }

    private void SubscribeFunctions()
    {
        removeObstacle.started += RemoveObstacle;
        revealPath.started += RevealPath;
        tool1.started += SetCurrentTool;
        tool2.started += SetCurrentTool;
        tool3.started += SetCurrentTool;
        tool4.started += SetCurrentTool;
        tool5.started += SetCurrentTool;
        start.started += StartCharacter;
        reset.started += RestartLevel;
    }

    private void UnsubscribeFunctions()
    {
        removeObstacle.started  -= RemoveObstacle;
        revealPath.started      -= RevealPath;
        tool1.started           -= SetCurrentTool;
        tool2.started           -= SetCurrentTool;
        tool3.started           -= SetCurrentTool;
        tool4.started           -= SetCurrentTool;
        tool5.started           -= SetCurrentTool;
        start.started           -= StartCharacter;
        reset.started           -= RestartLevel;
    }
}
