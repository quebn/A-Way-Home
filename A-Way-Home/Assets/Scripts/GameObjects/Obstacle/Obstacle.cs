using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public enum Tool { Inspect, Lightning, Grow, Command, Tremor}//, PlaceMode }

public class Obstacle : MonoBehaviour, ISaveable
{
    public const string TAG = "Obstacle";

    [SerializeField] private Vector2Int tileSize;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected string id;

    private int HP = 1;
    // private bool isHiglighted = false;
    protected List<Node> nodes;


    protected Vector2 worldPos => this.transform.position; 
    protected int nodeCount => tileSize.x * tileSize.y; 
    protected virtual int hitpoints {
        get => HP;
        set => HP = value;
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDisable()
    {
        ClearNodes();
    }

    protected virtual void Initialize()
    {
        if(!GameData.levelData.obstacles.ContainsKey(id))
        {
            GameData.levelData.obstacles.Add(id, new ObstacleData(this.GetType(), this.hitpoints, this.transform.position));
            // Debug.Log($"Added {id} in dictionary GameData.levelData.obstacles with hp of {hitpoints}");
        }
        Debug.Assert(GameData.levelData.obstacles.ContainsKey(id), "ERROR: Not in obstacle dictionary");
    }

    protected void SetNodes(Vector3 worldPos, NodeType nodeType, Obstacle obstacle = null)
    {
        // Initialize Node should:
        //      - Set what and how many nodes are assigned to the obstacle;
        //      - Set what are nodeType the node behaves as.
        //      - Set the what obstacle the nodes contains. 
        nodes = NodeGrid.GetNodes(worldPos, tileSize.x, tileSize.y);
        Node.SetNodesObstacle(nodes, nodeType, obstacle);
        // Debug.Log($"{this.gameObject.name} -> Nodes count{nodes.Count}");
    }

    protected void ClearNodes()
    {
        if (nodes == null || nodes.Count == 0 || Node.GetNodesInteractable(nodes).Count == 0)
            return;
        Node.SetNodesObstacle(nodes, NodeType.Walkable);
        nodes = new List<Node>();
    }

    protected virtual void OnDehighlight(Tool tool)
    {
        spriteRenderer.color = Color.white;
    }

    protected virtual void OnHighlight(Tool tool)
    {
        spriteRenderer.color = Color.green;
    } 

    public virtual void Destroy(Obstacle obstacle)
    {
        obstacle.Remove();
    }

    public void Highlight(Tool tool)
    {
        // isHiglighted = true;
        OnHighlight(tool);
    }

    public void Dehighlight(Tool tool)
    {
        // if(!isHiglighted)
            // return;
        // isHiglighted = false;
        OnDehighlight(tool);
    }

    public static void HighlightObstacles(List<Obstacle> list, Tool tool)
    {
        if(list.Count == 0)
            return;
        foreach(Obstacle obstacle in list)
            obstacle.Highlight(tool);
    }

    public static void DehighlightObstacles(List<Obstacle> list, Tool tool)
    {
        if(list.Count == 0)
            return;
        foreach(Obstacle obstacle in list)
            obstacle.Dehighlight(tool);

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

    public virtual void OnRevealNodeColor()
    {
        Node.RevealNodes(nodes, Node.colorWhite);
    }

    public virtual void Damage(int value)
    {
        hitpoints -= value;
        if(hitpoints <= 0)
            Remove();
    }

    public virtual void Remove() //Trigger death but for all
    {
        hitpoints = 0;
        this.gameObject.SetActive(false);
    }

    protected static void AddToOnPlayerActionList(IOnPlayerAction obstacle)
    {
        if(PlayerActions.onPlayerActions == null)
            PlayerActions.onPlayerActions = new List<IOnPlayerAction>();
        PlayerActions.onPlayerActions.Add(obstacle);
    }

    [ContextMenu("Generate Obstacle ID")]
    private void GenerateID() 
    {
        this.id = System.Guid.NewGuid().ToString();
    }
}

public interface IInspect
{
    public void OnInspect();
}

public interface ILightning
{
    public void OnLightningHit();
    public void OnAftershock(){}
}

public interface IGrow
{
    public void OnGrow();
}

public interface ICommand
{
    public void OnCommand();
}

public interface ITremor
{
    public void OnTremor();
}

public interface ITrap
{
    public void OnTrapTrigger(Character character);
}

public interface IOnPlayerAction
{
    public void OnPerformAction();
}

public interface IHoverable
{
    public void OnHover();
    public void OnDehover();
}