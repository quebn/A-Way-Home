using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Tool { Inspect, Lightning, Command, Grow, Tremor, PlaceMode }

public class Obstacle : MonoBehaviour
{
    public static string TAG = "Obstacle";

    [SerializeField] private List<Tool> toolTypes;
    [SerializeField] private Vector2Int tileSize;
    [SerializeField] private string ID;


    protected Vector2 worldPos => this.transform.position; 
    protected List<Node> nodes;
    protected SpriteRenderer spriteRenderer;
    
    protected int nodeCount => tileSize.x * tileSize.y; 
    protected bool isRemoved => !this.gameObject.activeSelf;
    protected bool incorrectTool => !toolTypes.Contains(PlayerActions.Instance.currentTool);
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
        if(ID == "")
            GenerateID();
        list.Add(this.ID, this);
    }

    protected void InitializeNodes(Vector3 worldPos, NodeType nodeType)
    {
        nodes = new List<Node>();
        nodes = NodeGrid.NodesByTileSize(worldPos, tileSize.x, tileSize.y);
        foreach(Node node in nodes)
            node.currentType = nodeType;
    }

    // protected void SetNodesType(NodeType nodeType)
    // {
    //     foreach(Node node in nodes)
    //         node.currentType = nodeType;
    // }

    // protected void SetNodesType(NodeType nodeType, INodeInteractable obstacle)
    // {
    //     foreach(Node node in nodes)
    //         node.SetObstacle(obstacle, nodeType);
    // }

    [ContextMenu("Generate Obstacle ID")]
    private void GenerateID() 
    {
        ID = System.Guid.NewGuid().ToString();
    }
}

interface IInteractable
{
    public void OnClick();
    public void OnHover();
    public void OnDehover();
}

interface ITrap
{
    public void OnTrapTrigger(Character character);
}

interface IPlaceable
{
    public void OnPlace();
}

public interface INodeInteractable
{
    public void OnNodeInteract(Tool tool);
}
