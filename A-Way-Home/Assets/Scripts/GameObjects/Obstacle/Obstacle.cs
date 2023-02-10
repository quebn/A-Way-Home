using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public enum Tool { Inspect, Lightning, Command, Grow, Tremor}//, PlaceMode }

public class Obstacle : MonoBehaviour, ISaveable
{
    public static string TAG = "Obstacle";
    [SerializeField] private Vector2Int tileSize;
    private int HP = 1;

    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected List<Node> nodes;
    // protected bool isActive;


    public string id;
    protected Vector2 worldPos => this.transform.position; 
    protected int nodeCount => tileSize.x * tileSize.y; 
    protected Tool currentTool => PlayerActions.Instance.currentTool;
    protected virtual int hitpoints {
        get => HP;
        set => HP = value;
    }


    private void Awake()
    {
        this.id = $"{this.gameObject.GetType().ToString()}{this.gameObject.name}{this.worldPos}";
    }

    private void Start()
    {
        Initialize();
    }


    protected virtual void Initialize()
    {
        // id = $"{this.gameObject.name}{this.worldPos}";
        if(GameData.levelData.spawneds.ContainsKey(id))
            return;
        if(!GameData.levelData.obstacles.ContainsKey(id))
        {
            GameData.levelData.obstacles.Add(id, hitpoints);
            Debug.Log($"Added {id} in dictionary GameData.levelData.obstacles with hp of {hitpoints}");
        }
        Debug.Assert(GameData.levelData.obstacles.ContainsKey(id), "ERROR: Not in obstacle dictionary");
        // if(!GameData.levelData.obstacles.ContainsKey(ID) || !GameData.levelData.spawneds.ContainsKey(ID))
        //     return;
        // if(GameData.levelData.obstacles.ContainsKey(ID))
        //     hitpoints = GameData.levelData.obstacles[ID];
        // else if(GameData.levelData.spawneds.ContainsKey(ID))
        //     hitpoints = GameData.levelData.spawneds[ID].hitpoints;
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
        Debug.Assert(levelData.obstacles.ContainsKey(id) || levelData.spawneds.ContainsKey(id));
        if(levelData.spawneds.ContainsKey(id)){
            Debug.Log("Saving spawned data");
            levelData.spawneds[id].hitpoints = this.hitpoints;
        }
        if(levelData.obstacles.ContainsKey(id))
        {
            Debug.Log("Saving obstacle data");
            levelData.obstacles[id] = this.hitpoints;
        }
    }

    public virtual void LoadData(LevelData levelData)
    {
        this.id = $"{this.gameObject.GetType().ToString()}{this.gameObject.name}{this.worldPos}";
        Debug.Assert(levelData.obstacles.ContainsKey(id) || levelData.spawneds.ContainsKey(id), $"ERROR: {id} not found");
        if(levelData.spawneds.ContainsKey(id))
        {
            Debug.Log("Loading spawned data");
            hitpoints = levelData.spawneds[id].hitpoints;
        }
        if(levelData.obstacles.ContainsKey(id))
        {
            Debug.Log("Loading obstacle data");
            hitpoints = levelData.obstacles[id];
        }
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
