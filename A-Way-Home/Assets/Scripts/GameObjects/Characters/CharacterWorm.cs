using System.Collections.Generic;
using UnityEngine;

public class CharacterWorm : Character, ICharacter
{
    [SerializeField] private GameObject portalObject;
    private GameObject entranceHole;
    private GameObject exitHole;
    private List<Vector3[]> paths;

    private void Start()
    {
        paths = new List<Vector3[]>();
        LoadPlatforms(portalObject);
    }

    private void Update()
    {
        if (isGoingHome)
        {
            animator.SetBool("isWalk", true);
            GoHome(paths[0]);
            InGameUI.Instance.SetCharacterEnergy(energy);
        }
    }
    public override void InitCharacter()
    {
        if (paths.Count == 0)
            SetPaths();
        if (paths[0].Length <= 0 )
        {
            paths = new List<Vector3[]>();
            return;
        }
        Debug.Log($"{paths[0][0].x}, {paths[0][0].y}");
        currentTargetPos = paths[0][0];
        targetIndex = 0;
        isGoingHome = true;
    }
    
    public void SetPaths()
    {
        if(entranceHole != null && exitHole != null){
            Debug.Log("Setting paths with holes set");
            paths.Add(Pathfinding.FindPath(transform.position, entranceHole.transform.position));
            paths.Add(Pathfinding.FindPath(exitHole.transform.position, homePosition));
        }else{
            if (entranceHole != null && entranceHole == null)
            {
                PlayerLevelData.Instance.levelData.skillCoords.RemoveAt(0);
                GameObject.Destroy(entranceHole);
                entranceHole = null;
            }
            paths.Add(Pathfinding.FindPath(currentPos, homePosition));
        }

    }

    private void GoHome(Vector3[] path)
    {
        if (currentPos == currentTargetPos)
        {
            energy--;
            targetIndex++;
            if (targetIndex >= path.Length)
            {
                if (currentPos == homePosition)
                {
                    Debug.Log("Reached");
                    isGoingHome = false;
                    PlayerLevelData.Instance.homeAnimator.SetBool("Reached", true);
                    this.gameObject.SetActive(false);
                    // TODO: Execute Window if animation of clodes is finished
                    // PlayerLevelData.Instance.homeAnimator.;
                    GameEvent.SetEndWindowActive(EndGameType.LevelClear);
                    return;
                }
                Debug.Log($"Not Reached character[{currentPos.x},{currentPos.y},{currentPos.z}] != home[{homePosition.x},{homePosition.x},{homePosition.x}]");
                paths.RemoveAt(0);
                transform.position = exitHole.transform.position;
                currentTargetPos = paths[0][0];
                targetIndex = 0;
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
    public void PerformSkill(Vector3 position)
    {
        if(PlayerLevelData.Instance.levelData.skillCount == 0)
            return;
        if (entranceHole == null)
            entranceHole = PlaceTunnel(position);
        else if(entranceHole != null && exitHole == null)
        {
            exitHole = PlaceTunnel(position);
            PlayerLevelData.Instance.levelData.skillCount--;
            SetSkillCounter();
        }
    }
    private GameObject PlaceTunnel(Vector3 worldPosition)
    {
        worldPosition.x = SetToMid(worldPosition.x);
        worldPosition.y = SetToMid(worldPosition.y);
        worldPosition.z = 0;
        Vector2 gridCoord = new Vector2();
        foreach (KeyValuePair<Vector2, Node> pair in NodeGrid.Instance.grid)
        {
            if (pair.Value.worldPosition != worldPosition)
                continue;
            gridCoord = pair.Key;
            if (pair.Value.containsObject || !pair.Value.isWalkable)
            {
                Debug.Log($"UNABLE TO PLACE: Node[{gridCoord.x},{gridCoord.y}] already has a portal or is unwalkable");
                return null;
            }
            break;
        }
        NodeGrid.Instance.grid[gridCoord].containsObject = true;
        PlayerLevelData.Instance.levelData.skillCoords.Add(new WorldCoords(worldPosition.x, worldPosition.y));
        return Instantiate(portalObject, worldPosition, Quaternion.identity);
    }
    public void OnCancel(){}
    public void OnClear(GameObject gameObject){}
}
