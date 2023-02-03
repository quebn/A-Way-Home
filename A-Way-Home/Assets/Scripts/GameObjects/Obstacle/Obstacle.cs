using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public enum Tool { Inspect, Lightning, Command, Grow, Tremor}//, PlaceMode }

public class Obstacle : MonoBehaviour
{
    public static string TAG = "Obstacle";

    [SerializeField] private Vector2Int tileSize;
    private string ID;

    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected List<Node> nodes;
    // protected bool isActive;

    protected Vector2 worldPos => this.transform.position; 
    protected int nodeCount => tileSize.x * tileSize.y; 
    protected Tool currentTool => PlayerActions.Instance.currentTool;

    public static Dictionary<string, Obstacle> list;

    private void Start()
    {
        Initialize();
    }

    public virtual void SetActionData(ActionData actionData)
    {
        actionData.ID = this.ID;
    } 

    public virtual void OnUndo(ActionData actionData)
    {
        // this.tag = Obstacle.TAG;
    }

    protected virtual void Initialize()
    {
        ID = $"{this.gameObject.name}{this.worldPos}";
        list.Add(this.ID, this);
    }

    protected void SetNodes(Vector3 worldPos, NodeType nodeType, IInteractable interactable = null)
    {
        // Initialize Node should:
        //      - Set what and how many nodes are assigned to the obstacle;
        //      - Set what are nodeType the node behaves as.
        //      - Set the what obstacle the nodes contains. 
        nodes = NodeGrid.GetNodes(worldPos, tileSize.x, tileSize.y);
        Node.SetNodesInteractable(nodes, nodeType, interactable);
        // Debug.Log($"{this.gameObject.name} -> Nodes count{nodes.Count}");
    }

    protected void ClearNodes()
    {
        if (nodes.Count == 0 || Node.GetNodesInteractable(nodes).Count == 0)
            return;
        Node.SetNodesInteractable(nodes, NodeType.Walkable);
        nodes = new List<Node>();
    }
    

    public static void HighlightInteractables(List<IInteractable> list)
    {
        if(list.Count == 0)
            return;
        foreach(IInteractable interactable in list)
            interactable.OnHighlight();
    }

    public static void DehighlightInteractables(List<IInteractable> list)
    {
        if(list.Count == 0)
            return;
        foreach(IInteractable interactable in list)
            interactable.OnDehighlight();

    }

}

public interface ITrap
{
    public void OnTrapTrigger(Character character);
}

public interface IInteractable
{
    public void OnInteract();
    
    public void OnHighlight();

    public void OnDehighlight();
}
