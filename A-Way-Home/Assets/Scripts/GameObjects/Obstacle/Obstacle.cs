using System.Collections.Generic;
using UnityEngine;

public enum Tool { Inspect, Lightning, Command, Grow, Tremor, None }

public class Obstacle : MonoBehaviour
{
    public const string TAG = "Interactable";

    [SerializeField] protected Texture2D mouseTexture;
    [SerializeField] private List<Tool> toolTypes;
    [SerializeField] private Vector2Int tileSize;
    [SerializeField] private string ID;

    protected List<Node> nodes;
    protected SpriteRenderer spriteRenderer;
    protected bool incorrectTool => !toolTypes.Contains(PlayerActions.Instance.currentTool);
    protected int size => tileSize.x * tileSize.y; 
    protected NodeType nodesType { set { foreach (Node node in nodes) node.currentType = value; }}

    public static Dictionary<string, Obstacle> list;

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        list.Add(this.ID, this);
        if (size > 0)
            nodes = new List<Node>(size);
    }

    protected void SetNodes(Vector3 worldPos)
    {
        if (nodes.Count == 1)
            nodes.Add(NodeGrid.NodeWorldPointPos(worldPos));
        else
            nodes = NodeGrid.NodesByTileSize(worldPos, tileSize.x, tileSize.y);
    }

    protected void SetMouseCursor(Texture2D mouseTexture)
    {
        Cursor.SetCursor(mouseTexture, Vector2.zero, CursorMode.Auto);
    }

    protected void ResetMouseCursor()
    {
        Cursor.SetCursor(UnityEditor.PlayerSettings.defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    [ContextMenu("Generate Obstacle ID")]
    private void GenerateID() 
    {
        this.ID = System.Guid.NewGuid().ToString();
    }

    protected void UpdateNodes()
    {
        nodesType = NodeType.Walkable;
        this.gameObject.SetActive(false);
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