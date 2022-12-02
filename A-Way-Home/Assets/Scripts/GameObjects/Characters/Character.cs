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
    public bool isHome{ get { return currentPos == SetToMid(homePosition);}}
    
    private void Awake()
    {
        characterImage = jellyPrefab.GetComponent<SpriteRenderer>();
        animator = jellyPrefab.GetComponent<Animator>();
    }

    private void Update()
    {
        GoHome();
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

    public virtual void InitCharacter()
    {
        if (path.Length <=0)
            return;
        currentTargetPos = path[0];
        targetIndex = 0;
        isGoingHome = true;
        animator.SetBool("isWalk", true);
    }

    private void GoHome()
    {
        if (isGoingHome)
        {
            Debug.Log("Stepping....");
            Step();
        }
    }

    private void Step()
    {
        if (currentPos == currentTargetPos)
        {
            targetIndex++;
            InGameUI.Instance.SetCharacterEnergy(-1);
            if (EndConditions())
                return;
            currentTargetPos = path[targetIndex];
        }
        transform.position = Vector3.MoveTowards(currentPos, currentTargetPos, speed * Time. deltaTime);

    }

    protected virtual bool EndConditions()
    {
        if (isHome){
            this.gameObject.SetActive(false);
            PlayerLevelData.Instance.homeAnimator.SetBool("Reached", true);
            return true;
        }
        if (energy == 0){
            if (PlayerLevelData.Instance.levelData.lives == 1){
                GameEvent.SetEndWindowActive(EndGameType.GameOver);
                return true;
            }
            GameEvent.SetEndWindowActive(EndGameType.NoEnergy);
            return true;
        }
        return false;
    }

    protected float SetToMid(float f)
    {
        if (f < 0)
            return (float)(MathF.Truncate(f) - .5);
        return (float)(MathF.Truncate(f) + .5);
    }

    protected Vector3 SetToMid(Vector3 vector3)
    {
        return new Vector3(SetToMid(vector3.x), SetToMid(vector3.y), 0);
    }
}

public interface ICharacter
{
    public void PerformSkill(Vector3 position, Collider2D collider2D);
    public void OnClear(GameObject gameObject){}
    public void OnDeselect(){}
    public void OnSkillUndo(ref Action action);
    public void OnToolUndo(ManipulationType manipulationType){}
}
