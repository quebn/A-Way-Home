using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[System.Serializable]
public enum Tool { Inspect, Lightning, Grow, Command, Tremor}//, PlaceMode }

public class Obstacle : MonoBehaviour, ISaveable
{
    [SerializeField] private Vector2Int tileSize;
    [SerializeField] protected string id;
    [SerializeField] protected GameObject outline;
    [SerializeField] protected bool isNotHoverable;

    [SerializeField] protected int hitpoints = 1;
    
    protected List<Node> nodes;

    protected Vector2 worldPos => this.transform.position; 
    protected int nodeCount => tileSize.x * tileSize.y; 

    public virtual bool isBurnable => false;
    public virtual bool isCorrosive => false;
    public virtual bool isMeltable => false;
    public virtual bool isTrampleable => false;
    public virtual bool isFragile => false;

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
        Debug.Assert(id != "", $"ERROR: {this.GetType().Name} ID is empty!");
    }

    protected void SetNodes(Vector3 worldPos, NodeType nodeType, Obstacle obstacle = null, bool isPlatform = false)
    {
        // Initialize Node should:
        //      - Set what and how many nodes are assigned to the obstacle;
        //      - Set what are nodeType the node behaves as.
        //      - Set the what obstacle the nodes contains. 
        nodes = new List<Node>();
        nodes = NodeGrid.GetNodes(worldPos, tileSize.x, tileSize.y);
        Node.SetNodesObstacle(nodes, nodeType, obstacle, isPlatform);
        // Debug.Log($"{this.gameObject.name} -> Nodes count{nodes.Count}");
    }

    protected void ClearNodes(NodeType nodeType = NodeType.Walkable, bool isPlatform = false)
    {
        if (nodes == null || nodes.Count == 0 || Node.GetNodesInteractable(nodes, isPlatform).Count == 0)
            return;
        Node.SetNodesObstacle(nodes, nodeType, isPlatform: isPlatform);
        nodes = new List<Node>();
    }


    protected virtual void OnDehighlight(Tool tool)
    {
        outline.SetActive(false);
    }

    protected virtual void OnHighlight(Tool tool)
    {
        outline.SetActive(true);
    } 

    public virtual void Destroy(Obstacle obstacle)
    {
        obstacle.Remove();
    }

    public void Highlight(Tool tool)
    {
        if(isNotHoverable && (outline == null  || outline.activeSelf))
            return;
        OnHighlight(tool);
    }

    public void Dehighlight(Tool tool)
    {
        if(outline == null  || !outline.activeSelf)
            return;
        OnDehighlight(tool);
    }

    public void WhileHighlight(Tool tool)
    {
        OnWhileHighlight(tool);
    }

    protected virtual void OnWhileHighlight(Tool tool)
    {
        Debug.Log("Hovering......");
    }

    protected void ForceDehighlight()
    {
        if(outline == null  || !outline.activeSelf)
            return;
        outline.SetActive(false);
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
        Debug.Assert(!levelData.obstacles.ContainsKey(id));
        levelData.obstacles.Add(this.id, new ObstacleData(this.GetType(), this.hitpoints, this.worldPos));
        Debug.Assert(levelData.obstacles.ContainsKey(id));
    }

    public virtual void LoadData(LevelData levelData)
    {
        Debug.Assert(id != "", $"ERROR: {this.GetType().Name} id is empty string");
        Debug.Assert(levelData.obstacles.ContainsKey(id), $"ERROR: {id} not found");
        if(levelData.obstacles.ContainsKey(id))
        {
            Debug.Log("Loading obstacle data");
            this.hitpoints = levelData.obstacles[id].GetValue("hp");
            this.gameObject.transform.position = levelData.obstacles[id].position;
        }
        if(hitpoints == 0)
            gameObject.SetActive(false);
        Debug.Assert(this.hitpoints == levelData.obstacles[id].GetValue("hp"), "ERROR: values doesnt match");
    }

    public virtual void Damage(int value)
    {
        hitpoints -= value;
        if(hitpoints <= 0)
            Remove();
    }

    public virtual void Remove() //Trigger death but for all
    {
        ForceDehighlight();
        hitpoints = 0;
        this.gameObject.SetActive(false);
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


public interface IHoverable
{
    public void OnHover();
    public void OnDehover();
}

public interface IActionWaitProcess
{
    public void OnPlayerAction();
}