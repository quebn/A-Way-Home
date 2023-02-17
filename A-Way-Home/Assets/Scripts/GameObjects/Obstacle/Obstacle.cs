using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public enum Tool { Inspect, Lightning, Command, Grow, Tremor}//, PlaceMode }

public class Obstacle : MonoBehaviour, ISaveable
{
    public const string TAG = "Obstacle";

    [SerializeField] private Vector2Int tileSize;
    private int HP = 1;

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected string id;
    protected List<Node> nodes;
    // protected bool isActive;


    protected Vector2 worldPos => this.transform.position; 
    protected int nodeCount => tileSize.x * tileSize.y; 
    protected Tool currentTool => PlayerActions.Instance.currentTool;
    protected virtual int hitpoints {
        get => HP;
        set => HP = value;
    }

    private void Start()
    {
        Initialize();
    }


    protected virtual void Initialize()
    {
        // id = $"{this.gameObject.name}{this.worldPos}";
        // Debug.Assert(id != "", "ERROR: id is empty");
        // if(GameEvent.loadType != LevelLoadType.LoadGame)
            // Debug.Assert(!GameData.levelData.obstacles.ContainsKey(id), $"ERROR: {id} should not be in obstacle dictionary");
        if(!GameData.levelData.obstacles.ContainsKey(id))
        {
            GameData.levelData.obstacles.Add(id, new ObstacleData(this.GetType(), this.hitpoints, this.transform.position));
            // Debug.Log($"Added {id} in dictionary GameData.levelData.obstacles with hp of {hitpoints}");
        }
        Debug.Assert(GameData.levelData.obstacles.ContainsKey(id), "ERROR: Not in obstacle dictionary");
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

    // protected IEnumerator SetNodesOnLoad(Vector3 worldPos, NodeType nodeType, IInteractable interactable = null)
    // {
    //     while(NodeGrid.Instance.grid.Count != NodeGrid.Instance.maxSize)
    //         yield return null;
    //     SetNodes(worldPos, nodeType, interactable);
    // }

    protected void ClearNodes()
    {
        if (nodes == null || nodes.Count == 0 || Node.GetNodesInteractable(nodes).Count == 0)
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

    public virtual void SaveData(LevelData levelData)
    {
        Debug.Assert(levelData.obstacles.ContainsKey(id));
        if(levelData.obstacles.ContainsKey(id))
        {
            Debug.Log("Saving obstacle data");
            levelData.obstacles[id].hitpoints = this.hitpoints;
            levelData.obstacles[id].position = this.gameObject.transform.position;
        }
    }

    public virtual void LoadData(LevelData levelData)
    {
        Debug.Assert(levelData.obstacles.ContainsKey(id), $"ERROR: {id} not found");
        if(levelData.obstacles.ContainsKey(id))
        {
            Debug.Log("Loading obstacle data");
            this.hitpoints = levelData.obstacles[id].hitpoints;
            this.gameObject.transform.position = levelData.obstacles[id].position;
        }
    }

    [ContextMenu("Generate Essence ID")]
    private void GenerateID() 
    {
        this.id = System.Guid.NewGuid().ToString();
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
    public void OnAfterShock(){}
}
