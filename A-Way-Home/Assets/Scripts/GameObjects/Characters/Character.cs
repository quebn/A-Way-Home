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

    public void InitCharacter()
    {
        path = Pathfinding.FindPath(currentPos, homePosition);
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
        transform.position = Vector3.MoveTowards(currentPos, currentTargetPos, speed * Time.deltaTime);

    }

    protected bool EndConditions()
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

    private static float SetToMid(float f)
    {
        if (f < 0)
            return  ((float)(int)f) - .5f;
        return ((float)(int)f) + .5f;
    }

    private static Vector3 SetToMid(Vector3 vector3)
    {
        return new Vector3(SetToMid(vector3.x), SetToMid(vector3.y), 0);
    }
}