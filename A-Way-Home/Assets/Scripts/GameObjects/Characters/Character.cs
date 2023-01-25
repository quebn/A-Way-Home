using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    protected int energy;
    protected int requiredEssence;
    protected int targetIndex; 
    protected float speed;
    protected bool isGoingHome = false;
    protected List<Node> path;
    protected Node currentTargetNode;


    public Sprite image => spriteRenderer.sprite;
    public Essence currentEssence => Essence.list[currentPosition];
    public Vector3 currentPosition => transform.position;
    public bool isHome => requiredEssence <= 0;
    public bool destinationReached => Essence.GetCurrentDestinations().Contains(currentPosition);
    public bool isMoving => isGoingHome;

    protected Vector3 currentTargetPos => currentTargetNode.worldPosition;

    protected int xPosDiff => (int)(currentTargetPos.x - currentPosition.x); 
    protected bool isFlipped {
        get => this.spriteRenderer.flipX; 
        set => this.spriteRenderer.flipX = value;
    }
    
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isGoingHome)
            Step();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        ITrap trap = collider.gameObject.GetComponent<ITrap>();
        trap.OnTrapTrigger(this);
    }


    public void Initialize(int energy, int EssenceNeeded)
    {
        IncrementEssence(EssenceNeeded);
        SetMaxEnergy(energy);
        GetPath();
        if (GameEvent.isSceneSandbox)
            this.speed = 5f;    
        else
            this.speed = GameData.Instance.gameSpeed;
    } 

    public List<Node> GetPath()
    {
        if(path != null)
            Node.ToggleNodes(path, NodeGrid.nodesVisibility);
        List<Vector3> destinations = Essence.GetCurrentDestinations();
        path = Pathfinding.FindPath(currentPosition, destinations);
        Debug.Log($"{requiredEssence} essence required! => {destinations.Count} Essence Found!");
        if(path.Count <= 0)
        {
            Debug.LogWarning("No Path Found");
            return path;
        } else
            Debug.Log("Path Found! Path nodes: " + path.Count);
        Node.ToggleNodes(path, Node.colorGreen, NodeGrid.nodesVisibility);
        return path;
    }

    public void GoHome()
    {
        if (path.Count <=0)
            return;
        currentTargetNode = path[0];
        targetIndex = 0;
        isGoingHome = true;
        animator.SetBool("isWalk", true);
    }

    private void Step()
    {
        if (currentPosition == currentTargetPos)
        {
            currentTargetNode.UpdateNode();
            targetIndex++;
            IncrementEnergy(-1);
            if (EndConditions())
                return;
            currentTargetNode = path[targetIndex];
        }
        Flip();
        transform.position = Vector3.MoveTowards(currentPosition, currentTargetPos, speed * Time.deltaTime);
    }
    
    private void Flip()
    {
        if (!isFlipped && xPosDiff < 0)
            isFlipped = !isFlipped;
        else if(isFlipped && xPosDiff > 0)
            isFlipped = !isFlipped;
    }

    protected bool EndConditions()
    {
        if (destinationReached)
            return Consume(currentEssence);
        if (energy == 0)
            return TriggerDeath();
        return false;
    }

    public bool Consume(Essence Essence)
    {
        animator.SetBool("isWalk", false);
        Essence.OnConsume(this);
        this.isGoingHome = false;
        if (isHome)
            TriggerLevelComplete();
        else
            GetPath();
        Debug.Log($"Current Essence Needed: {this.requiredEssence}");
        return true;
    }

    public void TriggerLevelComplete()
    {
        this.gameObject.SetActive(false);
        GameEvent.SetEndWindowActive(EndGameType.LevelClear);
    }

    public bool TriggerDeath()
    {
        this.isGoingHome = false;
        this.animator.SetBool("isWalk", isGoingHome);
        this.animator.Play("Character_Death");

        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("DisplayEndWindow", delay);
        return true;
    }

    private void DisplayEndWindow()
    {
        this.gameObject.SetActive(false);
        if (PlayerLevelData.Instance.levelData.lives == 1)
            GameEvent.SetEndWindowActive(EndGameType.GameOver);
        else
            GameEvent.SetEndWindowActive(EndGameType.TryAgain);
    }

    public int GetScore(int multiplier)
    {
        return this.energy * multiplier;  
    }

    public void SetMaxEnergy(int value)
    {
        this.energy = value;
        InGameUI.Instance.energyMaxValueUI = value;
    }

    public void IncrementEnergy(int increment)
    {
        this.energy += increment;
        InGameUI.Instance.energyValueUI = this.energy;
    }

    public void IncrementEssence(int increment)
    {
        this.requiredEssence += increment;
        InGameUI.Instance.essenceCounterUI = this.requiredEssence;
    }

    public bool NodeInPath(Node node)
    {
        return path.Contains(node);
    }
}