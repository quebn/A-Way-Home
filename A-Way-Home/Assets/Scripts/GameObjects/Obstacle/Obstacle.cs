using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private List<Vector2> nodesToBlock;
    [SerializeField] private Tool toolType;
    public string ID;

    protected List<Node> nodes;
    protected SpriteRenderer spriteRenderer;
    protected bool incorrectTool => toolType.ToString() != PlayerActions.Instance.currentTool;
    protected NodeType nodesType { set { foreach (Node node in nodes) node.currentType = value; }}
    
    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        PlayerLevelData.gameObjectList.Add(this.ID, this.gameObject);
        nodes = new List<Node>(nodesToBlock.Count);
        foreach (Vector2 position in nodesToBlock)
            nodes.Add(NodeGrid.NodeWorldPointPos(position));
    }

    // protected void SetNodesUnwalkable()
    // {
    //     Debug.Assert(nodes.Count != 0, "Error: no nodes in list");
    //     foreach (Node node in nodes)
    //         node.currentType = NodeType.Obstacle;
    // }

    [ContextMenu("Generate Obstacle ID")]
    private void GenerateID() 
    {
        this.ID = System.Guid.NewGuid().ToString();
    }
}

public enum Tool { Tool1, Tool2, Tool3, Tool4, Tool5, Tool6, }

interface IObstacle 
{
    public void OnClick();
    public void OnHover();
    public void OnDehover();
}