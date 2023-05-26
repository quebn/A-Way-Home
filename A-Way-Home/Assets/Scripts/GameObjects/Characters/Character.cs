using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Character : MonoBehaviour, ISaveable
{
    public static Character instance;

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject higlightLight;
    protected int energy;
    protected int requiredEssence;
    protected int targetIndex; 
    protected float speed;
    protected bool isGoingHome = false;
    protected List<Node> path;
    protected Node currentTargetNode;
    protected bool isAlive = true;

    public Node currentNode => NodeGrid.NodeWorldPointPos(this.currentPosition);

    public Sprite image => spriteRenderer.sprite;
    public Essence currentEssence => Essence.GetEssence(currentPosition);
    public Vector3 currentPosition => transform.position;
    public bool isHome => noEssenceRequired && currentPosition == Home.instance.transform.position;
    public bool destinationReached => Essence.GetCurrentDestinations().Contains(currentPosition) || currentPosition == Home.instance.transform.position;
    public bool isMoving => isGoingHome;
    public bool hasPath => path.Count > 0;
    public bool isDead => !isAlive;
    protected Vector3 currentTargetPos => currentTargetNode.worldPosition;
    public bool noEssenceRequired => requiredEssence == 0;

    protected int xPosDiff => (int)(currentTargetPos.x - currentPosition.x); 
    protected bool isFlipped {
        get => this.spriteRenderer.flipX; 
        set => this.spriteRenderer.flipX = value;
    }
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update()
    {
        // Replace with Coroutine
        if (!isGoingHome && PlayerActions.finishedProcessing && PlayerActions.finishedCommand)
            InGameUI.Instance.TimeCountdown();
        if(isGoingHome)
            Step();
    }

    public void CanHighlight(bool condition)
    {
        if(condition == higlightLight.activeSelf)
            return;
        higlightLight.SetActive(condition);
    }

    public void Interact()
    {
        higlightLight.SetActive(false);
        GoHome();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.LogWarning($"Trap {collider.gameObject.name} collided");
        ITrap trap = collider.gameObject.GetComponent<ITrap>();
        if(trap == null)
            return;
        trap.OnTrapTrigger(this);
        if(energy <= 0)
            TriggerDeath();
    }

    public void DamageAnimation()
    {
        Debug.Log("Character Damaged");
        animator.Play("Damage");
    }

    public void Initialize(LevelData levelData)
    {
        // this.requiredEssence = levelData.characterRequiredEssence;
        IncrementEssence(levelData.characterRequiredEssence);
        this.energy = levelData.characterEnergy;
        InGameUI.Instance.energyValueUI = this.energy;
        // SetMaxEnergy(levelData.characterEnergy);
        Debug.Assert(this.requiredEssence == levelData.characterRequiredEssence, "ERROR: Character essence needed value mismatch");
        Debug.Assert(this.energy == levelData.characterEnergy, "ERROR: Character energy value mismatch");
        Debug.Log($"[{GameEvent.loadType.ToString()}]: Initialized Character with {levelData.characterEnergy} energy and {levelData.characterRequiredEssence} required Essence");
        Debug.Log($"[{GameEvent.loadType.ToString()}]: Initialized Character with {this.energy} energy and {this.requiredEssence} required Essence");
        Debug.Assert(currentNode != null);
        this.speed = 5f;
        StartCoroutine(GetPathOnInit());
    } 

    private IEnumerator GetPathOnInit()
    {
        int countEssence = GameObject.FindObjectsOfType<Essence>(false).Length;
        int countObstacle = GameObject.FindObjectsOfType<Obstacle>(false).Length;
        Debug.LogWarning($"Essence Count: {countEssence} -> {Essence.count}");
        Debug.LogWarning($"Obstacle Count: {countObstacle} -> {Obstacle.count}");
        yield return new WaitUntil(() => (Essence.count == countEssence && Obstacle.count == countObstacle && GameData.levelData.spawnCount == Spawnable.spawnCount));
        GetPath();
    }

    public List<Node> GetPath()
    {
        Debug.LogWarning("-------------------Getting Char Path-----------------");
        List<Node> oldPath = path; 
        path = new List<Node>();
        if(requiredEssence != 0)
        {
            List<Vector3> destinations = Essence.GetCurrentDestinations();
            path = Pathfinding.FindPath(currentPosition, destinations, isChar:true);
            Debug.Log($"{requiredEssence} essence required! => {destinations.Count} Essence Found!");
        }
        else{
            Home.instance.ActivateHome();
            List<Vector3> homes = new List<Vector3>(){Home.instance.transform.position};
            path = Pathfinding.FindPath(currentPosition, homes, isChar:true);
        }
        Node.ToggleNodes(oldPath, NodeGrid.nodesVisibility);
        if(path.Count <= 0)
        {
            Debug.LogWarning("No Path Found for Character");
            return path;
        } else
            Debug.Log("Path Found! Path nodes: " + path.Count);
        Node.ToggleNodes(path, Node.colorGreen, NodeGrid.nodesVisibility);
        return path;
    }

    public void GoHome()
    {
        if (path.Count <=0 || isMoving || energy <= 0)
            return;
        currentTargetNode = path[0];
        targetIndex = 0;
        isGoingHome = true;
        animator.SetBool("isWalk", true);
    }

    public void Relocate(Vector2 location)
    {
        this.transform.position = location;
        // currentNode = NodeGrid.NodeWorldPointPos(this.currentPosition); 
        GetPath();
        Debug.Log($"Relocated Character to {location}");
    }

    private void Step()
    {
        if (currentPosition == currentTargetPos)
        {
            AudioManager.instance.PlayAudio("Step");
            NodeStatusInteract(currentTargetNode);
            currentTargetNode.UpdateNodeColor();
            targetIndex++;
            if (EndConditions())
                return;
            currentTargetNode = path[targetIndex];
        }
        Flip();
        transform.position = Vector3.MoveTowards(currentPosition, currentTargetPos, speed * Time.deltaTime);
    }

    private void NodeStatusInteract(Node node)
    {
        switch(node.currentStatus)
        {
            case NodeStatus.None:
                IncrementEnergy(-1);
                break;
            case NodeStatus.Conductive:
                if(!IsName("Fulmen"))
                    IncrementEnergy(-1);
                break;
            case NodeStatus.Burning:
                TriggerDeath();
                break;
            case NodeStatus.Corrosive:
                TriggerDeath();
                break;
        }
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
        InGameUI.Instance.activatedCharacter = false;
        if (energy <= 0)
            return TriggerDeath();
        if (destinationReached)
        {
            Debug.LogWarning("Return True");
            return currentPosition == Home.instance.transform.position ? TriggerLevelComplete() : Consume(currentEssence);
        }
        Debug.LogWarning("Return false");
        return false;
    }

    public bool Consume(Essence essence)
    {
        animator.SetBool("isWalk", false);
        essence.OnConsume(this);
        this.isGoingHome = false;
        // this.currentNode = currentTargetNode;
        GetPath();
        Debug.Log($"Current Essence Needed: {this.requiredEssence}");
        return true;
    }

    public bool TriggerLevelComplete()
    {
        ScoreSystem.CalculateScore();
        AudioManager.instance.PlayAudio("LevelComplete");
        this.gameObject.SetActive(false);
        if(Home.instance.portalActivated){
            GameEvent.GoToEndStoryScene();
            return true;
        }
        GameEvent.UnlockNextStageLevel();
        GameEvent.SetEndWindowActive(EndGameType.LevelClear);
        return true;
    }

    public bool TriggerDeath(float animDelay = 0)
    {
        this.isGoingHome = false;
        this.isAlive = false;
        this.animator.SetBool("isWalk", isGoingHome);
        StartCoroutine(PlayDeathAnim(animDelay));
        AudioManager.instance.PlayAudio("Death");
        return true;
    }

    private IEnumerator PlayDeathAnim(float delayDeath = 0)
    {
        yield return new WaitForSeconds(delayDeath);
        this.animator.Play("Character_Death");
        StartCoroutine(DisplayEndWindow(animator.GetCurrentAnimatorClipInfo(0).Length));

    }

    private IEnumerator DisplayEndWindow(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.gameObject.SetActive(false);
        if (GameData.levelData.lives == 1)
            GameEvent.SetEndWindowActive(EndGameType.GameOver);
        else
            GameEvent.SetEndWindowActive(EndGameType.TryAgain);
    }

    public float GetScore(float multiplier)
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
        return path != null ? path.Contains(node) : false;
    }

    public void SaveData(LevelData levelData)
    {
        levelData.characterEnergy = this.energy;
        levelData.characterPosition = this.currentPosition;
        levelData.characterRequiredEssence = this.requiredEssence;
        Debug.Log($"Saved ff. Character Data -> Energy:{this.energy} \n Essence Required:{this.requiredEssence} \n Position{this.currentPosition}");
    }

    public void LoadData(LevelData levelData)
    {
        this.transform.position = levelData.characterPosition;
    }

    public static bool IsName(string name)
    {
        return name == GameData.levelData.characterName;
    }
}