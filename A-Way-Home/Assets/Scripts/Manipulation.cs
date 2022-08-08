using UnityEngine;
using UnityEngine.InputSystem;

public class Manipulation : MonoBehaviour
{
    enum ToolType{ None, Pickaxe, WoodAxe, }
    public uint Moves = 0;
    private ToolType _CurrentType = ToolType.None;
    private void Update()
    {
        SetCurrentTool();
        switch(_CurrentType)
        {
            case ToolType.Pickaxe:
                ClearObstacle("RockObstacle");
                break;
            case ToolType.WoodAxe:
                ClearObstacle("WoodObstacle");
                break;
        }
    }

    private void SetCurrentTool()
    {
        if (Keyboard.current.zKey.wasPressedThisFrame)
        {
            Debug.Log("Current Tool: Pickaxe");
            _CurrentType = ToolType.Pickaxe;
        }
        else if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            Debug.Log("Current Tool: WoodAxe");
            _CurrentType = ToolType.WoodAxe;
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            Debug.Log("Current Tool: None");
            _CurrentType = ToolType.None;
        }
    }

    private void ClearObstacle(string obstacletag)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Moves == 0)
            {
                Debug.Log("No moves Left");
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit2D.collider != null && hit2D.collider.gameObject.tag == obstacletag)
            {
                Destroy(hit2D.collider.gameObject);
                Moves--;
                Debug.Log(hit2D.collider.gameObject + " was Destroyed");
                Debug.Log("Moves Left: " + Moves);
            }
        }
    }
}

