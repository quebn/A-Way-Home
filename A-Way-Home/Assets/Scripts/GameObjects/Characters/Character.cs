using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public void PerformSkill(Vector3 position);
    public void OnClear(GameObject gameObject);
    
}
public class Character : MonoBehaviour
{
    [SerializeField] protected GameObject jellyPrefab;
    [HideInInspector] public SpriteRenderer characterImage;
    [HideInInspector] public Vector3 homePosition;
    [HideInInspector] public uint energy;
    [HideInInspector] public float speed;
    [HideInInspector] public bool isSkillActive;
    [HideInInspector] public bool isGoingHome = false;

    private Vector3[] path;

    protected Animator animator;
    protected Vector3 currentTargetPos;   
    protected int targetIndex;

    public Vector3 currentPos{ get { return transform.position; } }
    public bool isHome{ get { return this.transform.position == homePosition;}}
    
    private void Awake()
    {
        characterImage = jellyPrefab.GetComponent<SpriteRenderer>();
        animator = jellyPrefab.GetComponent<Animator>();
    }

    protected void SetSkillCounter()
    {
        InGameUI.Instance.SkillCounter = PlayerLevelData.Instance.levelData.skillCount;
    }

    protected void LoadPlatforms(GameObject spawnedObject)
    {
        List<WorldCoords> coords = PlayerLevelData.Instance.levelData.skillCoords;
        if (coords.Count == 0)
            return;
        foreach(WorldCoords coord in coords)
            Instantiate(spawnedObject,new Vector2(coord.x, coord.y), Quaternion.identity);
    }
    private void Update()
    {
        if (isGoingHome)
        {
            animator.SetBool("isWalk", true);
            GoHome();
            InGameUI.Instance.SetCharacterEnergy(energy);
        }
    }

    public virtual void InitCharacter()
    {
        path = Pathfinding.FindPath(currentPos, homePosition);
        Debug.Log(path.Length);
        if (path.Length <=0)
            return;
        currentTargetPos = path[0];
        targetIndex = 0;
        isGoingHome = true;
    }

    public void DisplayPath()
    {
        GameObject tile = NodeGrid.Instance.revealedTile;
        Vector3[] nodePath = Pathfinding.FindPath(currentPos, homePosition);
        foreach (Vector3 position in nodePath)
        {
            Instantiate(tile, position, Quaternion.identity);
        }
    }

    protected virtual void GoHome()
    {
        if (currentPos ==  currentTargetPos)
        {
            energy--;
            targetIndex++;
            if (targetIndex >= path.Length)
            {
                isGoingHome = false;
                PlayerLevelData.Instance.homeAnimator.SetBool("Reached", true);
                this.gameObject.SetActive(false);
                // TODO: Execute Window if animation of clodes is finished
                // PlayerLevelData.Instance.homeAnimator.;
                GameEvent.SetEndWindowActive(EndGameType.LevelClear);
                return;
            }
            if (energy == 0)
            {
                if (PlayerLevelData.Instance.levelData.lives == 1)
                {
                    isGoingHome = false;
                    GameEvent.SetEndWindowActive(EndGameType.GameOver);
                    return;
                }
                isGoingHome = false;
                GameEvent.SetEndWindowActive(EndGameType.NoEnergy);
                return;   
            }
            currentTargetPos = path[targetIndex];
        }
        transform.position = Vector3.MoveTowards(currentPos, currentTargetPos, speed * Time. deltaTime);
    }

    protected float SetToMid(float f)
    {
        if (f < 0)            
            return (float)(MathF.Truncate((float)f) - .5);
        return (float)(MathF.Truncate((float)f) + .5);
    }
}

