using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class Obstacle : MonoBehaviour, ISaveable, IInspect
{
    [SerializeField] private InspectInfo info;
    [SerializeField] private Vector2Int tileSize;
    [SerializeField] protected string id;
    [SerializeField] protected List<GameObject> outlines;
    [SerializeField] protected bool isNotHoverable;
    [SerializeField] protected List<Tool> toolsInteractable;
    [SerializeField] protected List<SpriteRenderer> spriteRenderers;
    [SerializeField] protected List<AudioSource> audioSources;

    protected List<Node> nodes;

    protected int hitpoints {
        get => info.hp; 
        set => info.hp = value;
    }
    protected int heal{ 
        get => info.heal; 
        set => info.heal = value;
    }
    protected SpriteRenderer mainSpriteRenderer => spriteRenderers[0];
    protected Vector2 worldPos => this.transform.position; 
    protected int nodeCount => tileSize.x * tileSize.y; 

    public virtual bool isBurnable => false;
    public virtual bool isCorrosive => false;
    public virtual bool isMeltable => false;
    public virtual bool isTrampleable => false;
    public virtual bool isFragile => false;
    public virtual bool isWalkableByGaia => false;
    public virtual bool isWalkableByTerra => false;

    public static int count = 0;

    private void Start()
    {
        Debug.Assert(spriteRenderers.Count != 0);
        Initialize();
        count++;
    }

    private void OnDisable()
    {
        ClearNodes();
    }

    public void OnInspect()
    {
        InspectUI.inspectInfo = this.info;
    }

    protected virtual void Initialize()
    {
        Debug.Assert(spriteRenderers != null || spriteRenderers.Count != 0, "ERROR: No sprite renderers found!");
        Debug.Assert(id != "", $"ERROR: {this.GetType().Name} ID is empty!");
    }

    protected void SetNodes(Vector3 worldPos, NodeType nodeType, Obstacle obstacle = null, bool isPlatform = false, bool retainType = false)
    {

        nodes = new List<Node>();
        nodes = NodeGrid.GetNodes(worldPos, tileSize.x, tileSize.y);
        Node.SetNodesObstacle(nodes, nodeType, obstacle, isPlatform, retainType);
    }

    protected void ClearNodes(NodeType nodeType = NodeType.Walkable, bool isPlatform = false, bool isRetained = false)
    {
        if (nodes == null || nodes.Count == 0 || Node.GetNodesObstacle(nodes, isPlatform).Count == 0)
            return;
        Node.SetNodesObstacle(nodes, nodeType, isPlatform: isPlatform, retainType:isRetained);
        nodes = new List<Node>();
    }


    protected virtual void OnDehighlight()
    {
        outlines[0].SetActive(false);
    }

    protected virtual void OnHighlight(Tool tool)
    {
        outlines[0].SetActive(true);
    } 

    protected bool AffectedByStatus(Node node)
    {
        switch(node.currentStatus)
        {
            case NodeStatus.Burning:
                return this.isBurnable;
            case NodeStatus.Corrosive:
                return this.isCorrosive;
            default:
                return false;
        }
    }

    protected void OnStatusInteract(Node node, Action<Node> action)
    {
        if(this.AffectedByStatus(node))
            this.Remove();
        else
            action(node);
    }

    protected void OnStatusInteract(Node node)
    {
        if(this.AffectedByStatus(node))
            this.Remove();
    }

    public virtual void Destroy(Obstacle obstacle)
    {
        obstacle.Remove();
    }

    public virtual void Destroy(Node node)
    {
        if(node.hasObstacle)
            Destroy(node.GetObstacle());
    }

    public void Highlight(Tool tool)
    {
        if((outlines == null ||outlines.Count == 0  || outlines[0].activeSelf) || !toolsInteractable.Contains(tool) || isNotHoverable)
            return;
        OnHighlight(tool);
    }

    public void Dehighlight()
    {
        if(outlines == null ||outlines.Count == 0  || !outlines[0].activeSelf || isNotHoverable)
            return;
        OnDehighlight();
    }

    public bool SelectableBy(Tool tool)
    {
        return toolsInteractable.Contains(tool);
    }
    
    public void WhileHovered(Tool tool)
    {
        if(!toolsInteractable.Contains(tool))
            return;
        OnWhileHovered(tool);
    }

    protected virtual void OnWhileHovered(Tool tool)
    {
        // Debug.Log("Hovering......");
    }

    protected void ForceDehighlight()
    {
        if(outlines[0] == null  || !outlines[0].activeSelf)
            return;
        outlines[0].SetActive(false);
    }

    public static void HighlightObstacles(List<Obstacle> list, Tool tool)
    {
        if(list.Count == 0)
            return;
        foreach(Obstacle obstacle in list)
            obstacle.Highlight(tool);
    }

    public static void DehighlightObstacles(List<Obstacle> list)
    {
        if(list.Count == 0)
            return;
        foreach(Obstacle obstacle in list)
            obstacle.Dehighlight();

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
        if(!levelData.obstacles.ContainsKey(id))
                return;
        this.hitpoints = levelData.obstacles[id].GetValue("hp");
        this.gameObject.transform.position = levelData.obstacles[id].position;
        Debug.Log($"Loaded Leveldata Obstacles :{levelData.obstacles[id].typeName} with hp: {levelData.obstacles[id].valuePairs["hp"]} -> {id}");
        if(hitpoints == 0)
            gameObject.SetActive(false);
        Debug.Assert(this.hitpoints == levelData.obstacles[id].GetValue("hp"), "ERROR: values doesnt match");
    }

    private IEnumerator DamageAnim()
    {
        for(int i = 0; i < spriteRenderers.Count; i++)
            spriteRenderers[i].color = Color.red;
        yield return new WaitForSeconds(.25f);
        for(int i = 0; i < spriteRenderers.Count; i++)
            spriteRenderers[i].color = Color.white;
    }

    public virtual void Damage(int value)
    {
        DamageAnimation();
        hitpoints -= value > hitpoints ? hitpoints : value;
        if(hitpoints <= 0)
            Remove();
    }

    protected void DamageAnimation()
    {
        if(spriteRenderers == null || spriteRenderers.Count == 0)
            return;
        StartCoroutine(DamageAnim());
    }

    public virtual void Remove()
    {
        ForceDehighlight();
        hitpoints = 0;
        List<Node> prevNodes = new List<Node>();
        for(int i = 0; i < nodes.Count; i++)
            prevNodes.Add(nodes[i]);
        ClearNodes();
        for(int i = 0; i < prevNodes.Count; i++)
            FireNode.ContinueFire(prevNodes[i]);
        this.gameObject.SetActive(false);
        
    }

    public bool WaitforFinishInit()
    {
        throw new NotImplementedException();
    }

    [ContextMenu("Generate Obstacle ID")]
    private void GenerateID() 
    {
        this.id = System.Guid.NewGuid().ToString();
    }

    [ContextMenu("Set Position Middle")]
    private void SetPosMid() 
    {
        this.transform.position = NodeGrid.GetMiddle(this.worldPos, tileSize.x, tileSize.y);
    }
}

public interface ISelectable
{
    public void OnSelect(Tool tool);
    public List<Node> OnSelectedHover(Vector3 mouseWorldPos, List<Node> currentNodes);
    public void OnDeselect();
    public List<Node> IgnoredToggledNodes();
}

public interface IInspect
{
    public void OnInspect();
}

public interface ILightning
{
    public void OnLightningHit(int damage);
    public void OnAftershock(Vector2 lightningOrigin){}
}

public interface IGrow
{
    public void OnGrow();
}

public interface ICommand
{
    public bool OnCommand(List<Node> nodes);
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