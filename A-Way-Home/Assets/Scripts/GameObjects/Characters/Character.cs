using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] protected GameObject jellyPrefab;
    [HideInInspector] public SpriteRenderer characterImage;
    [HideInInspector] public Vector3 homePosition;
    [HideInInspector] public int energy;
    [HideInInspector] public float speed;
    [HideInInspector] public bool isSkillActive;
    [HideInInspector] public bool isGoingHome = false;

    protected Vector3[] path;
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
    protected virtual void LoadPlatforms(GameObject spawnedObject)
    {
        List<Action> skillActions = new List<Action>();
        foreach(Action action in PlayerLevelData.Instance.levelData.actionList)
            if(action.type == ManipulationType.UniqueSkill)
                skillActions.Add(action);
        if (skillActions.Count == 0)
            return;
        foreach(Action action in skillActions)
        {
            GameObject gameObject = GameObject.Instantiate(spawnedObject, action.skillCoord, Quaternion.identity);
            PlayerLevelData.gameObjectList.Add($"{gameObject.transform.position.ToString()}", gameObject);
        }
    }
    private void Update()
    {
        if (isGoingHome)
        {
            animator.SetBool("isWalk", true);
            GoHome();
        }
    }

    public virtual void InitCharacter()
    {
        path = Pathfinding.FindPath(currentPos, homePosition);
        if (path.Length <=0)
            return;
        currentTargetPos = path[0];
        targetIndex = 0;
        isGoingHome = true;
    }


    protected virtual void GoHome()
    {
        if (currentPos ==  currentTargetPos)
        {
            InGameUI.Instance.SetCharacterEnergy(-1);
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

    protected Vector3 SetToMid(Vector3 vector3)
    {
        return new Vector3(SetToMid(vector3.x), SetToMid(vector3.y), 0);
    }
}

public interface ICharacter
{
    public void PerformSkill(Vector3 position, Collider2D collider2D, string tag);
    public void OnClear(GameObject gameObject){}
    public void OnDeselect(){}
    public void OnSkillUndo(ref Action action);
    public void OnToolUndo(ManipulationType manipulationType){}
}
