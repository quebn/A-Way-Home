using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public void PerformSkill(Vector3 position, Collider2D collider2D, string tag);
    public void OnClear(GameObject gameObject);
    public void OnDeselect();
    
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

    public virtual void DisplayPath(bool toggle)
    {
        
        Vector3[] nodePath = Pathfinding.FindPath(currentPos, homePosition);
        if (nodePath.Length == 0)
        {
            Debug.Log("no path to be shown.");
            return;
        }
        Debug.Log($"Showing Path | Path length = {nodePath.Length}");
        for (int i = 0; i < nodePath.Length; i++)
        {
            Node node = NodeGrid.NodeWorldPointPos(nodePath[i]);
            if (!toggle)
                node.tileSprite.color = new Color32(255, 255, 255, 150);
            else
                node.tileSprite.color = Color.green;
            node.tileObject.SetActive(toggle);
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

    protected Vector3 SetToMid(Vector3 vector3)
    {
        return new Vector3(SetToMid(vector3.x), SetToMid(vector3.y), 0);
    }
}

