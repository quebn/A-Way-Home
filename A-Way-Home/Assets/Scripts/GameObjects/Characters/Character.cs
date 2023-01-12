using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [HideInInspector] public SpriteRenderer characterImage;
    [HideInInspector] public int energy;
    [HideInInspector] public float speed;
    [HideInInspector] public bool isGoingHome = false;

    protected int requiredEssence;
    protected Vector3[] path;
    protected Animator animator;
    protected Vector3 currentTargetPos;
    protected int targetIndex; 

    protected bool isFlipped {get => this.characterImage.flipX; set => this.characterImage.flipX = value;}
    protected int xPosDiff => (int)(currentTargetPos.x - currentPosition.x); 
    protected List<Vector3> currentDestinations => PlayerLevelData.Instance.currentDestinations; 
    public Essence currentEssence => Essence.list[currentPosition];
    public Vector3 currentPosition => transform.position;
    public bool destinationReached => currentDestinations.Contains(currentPosition);
    public bool isHome => requiredEssence == 0;
    
    private void Awake()
    {
        characterImage = GetComponent<SpriteRenderer>();
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
        this.requiredEssence += EssenceNeeded;
        SetMaxEnergy(energy);
        if (GameEvent.isSceneSandbox)
            this.speed = 5f;    
        else
            this.speed = GameData.Instance.gameSpeed;
    } 

    public void SetPath()
    {
        path = Pathfinding.FindPath(currentPosition, currentDestinations);
        Debug.LogWarning($"{currentDestinations.Count} Targets Remaining");
    }

    public void GoHome()
    {
        SetPath();
        if (path.Length <=0)
            return;
        currentTargetPos = path[0];
        targetIndex = 0;
        isGoingHome = true;
        animator.SetBool("isWalk", true);
    }

    private void Step()
    {
        // add Flip where the character is facing depending of whether its X position is decreasing or increasing.
        if (currentPosition == currentTargetPos)
        {
            targetIndex++;
            IncrementEnergy(-1);
            if (EndConditions())
                return;
            currentTargetPos = path[targetIndex];
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
        Essence.OnConsume(this);
        this.isGoingHome = false;
        if (isHome)
            TriggerLevelComplete();
        else
            NextTarget();
        Debug.Log($"Current Essence Needed: {this.requiredEssence}");
        return true;
    }

    private void NextTarget()
    {
        SetPath();
    }


    public void TriggerLevelComplete()
    {
        this.gameObject.SetActive(false);
        GameEvent.SetEndWindowActive(EndGameType.LevelClear);
    }

    // Character should have an TriggerDeath method
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
}