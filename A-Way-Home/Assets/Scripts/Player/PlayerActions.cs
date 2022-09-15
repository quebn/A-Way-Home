using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public enum ManipulationType{ None, Pickaxe, WoodAxe, }
public static class PlayerActions
{
    public static ManipulationType CurrentManipulationType;

    public static void ClearItem()
    {
        switch(CurrentManipulationType)
        {
            case ManipulationType.Pickaxe:
                ClearObstacle("RockObstacle");
                break;
            case ManipulationType.WoodAxe:
                ClearObstacle("WoodObstacle");
                break;
        }
    }
    public static void SetCurrentTool()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Debug.Log("Current Tool: Pickaxe");
            CurrentManipulationType = ManipulationType.Pickaxe;
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Debug.Log("Current Tool: WoodAxe");
            CurrentManipulationType = ManipulationType.WoodAxe;
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Current Tool: None");
            CurrentManipulationType = ManipulationType.None;
        }
    }
    private static void ClearObstacle(string obstacletag)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (PlayerLevelData.Instance.PlayerMoves == 0)
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
                PlayerLevelData.Instance.RemovedObstacles.Add(ObstacleID, false);
                PlayerLevelData.Instance.PlayerMoves--;
                Debug.Log(hit2D.collider.gameObject + " was Destroyed");
                Debug.Log("Moves Left: " + PlayerLevelData.Instance.PlayerMoves);
                Debug.Log($"Added Obstacle with id of {ObstacleID} to Removed Obstacles Dictionary!");
                
            }
        }
    }
    
    private static bool IsMouseOverUI(){
        return EventSystem.current.IsPointerOverGameObject();
    }
}
