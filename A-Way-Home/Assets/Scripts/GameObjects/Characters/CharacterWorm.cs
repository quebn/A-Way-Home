using System.Collections.Generic;
using UnityEngine;

public class CharacterWorm : Character, ICharacter
{
    [SerializeField] private GameObject portalObject;
    [SerializeField] private GameObject placeableTile;
    [SerializeField] private int skillRange;
    private GameObject entranceHole;
    private GameObject exitHole;
    private List<Vector3[]> paths;
    private List<Vector3> placeablePositions;
    private List<GameObject> placeableTiles;
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
    public void PerformSkill(Vector3 position, Collider2D collider2D, string tag)
    {
        // Checks and returns if theres is an obstacle when placing the skill 
        if(collider2D != null || tag == "Obstacle" || PlayerLevelData.Instance.levelData.skillCount == 0)
            return; 
        if (entranceHole == null)
        {
            entranceHole = PlaceTunnel(position);
            Debug.Log("Performed setting entranceHole ");
            placeableTiles = new List<GameObject>();
            placeablePositions = GetPlaceablePos(skillRange);
            HighlightPlaceables();
        }
        else if(entranceHole != null && exitHole == null)
        {
            //check if position is within the number tiles given by skillRange as the distance of entrance.
            if(placeablePositions.Contains(SetToMid(position)))
                exitHole = PlaceTunnel(position);
            else    
                Debug.Log($"Not Placeable in {SetToMid(position).ToString()}");
            if (exitHole == null)
                return;
            foreach (GameObject tile in placeableTiles)
                GameObject.Destroy(tile);
            PlayerLevelData.Instance.levelData.skillCount--;
            SetSkillCounter();
        }
    }
    
    private void HighlightPlaceables()
    {
        foreach(Vector3 position in placeablePositions)
        {
            placeableTiles.Add(GameObject.Instantiate(placeableTile, position, Quaternion.identity));
        }
    }
    private List<Vector3> GetPlaceablePos(int tileRange)
    {
        Node entranceNode = NodeGrid.NodeWorldPointPos(entranceHole.transform.position);
        List<Vector3> placeablePos = new List<Vector3>();
        Dictionary<Vector2, Node> grid = NodeGrid.Instance.grid;
        for (int x = -tileRange; x <= tileRange; x++){
            for (int y = -tileRange; y <= tileRange; y++){
                if (x == 0 && y == 0)
                    continue;
                Vector2Int check = new Vector2Int(entranceNode.gridPos.x + x, entranceNode.gridPos.y + y);
                if (grid.ContainsKey(check) && grid[check].IsWalkable())
                    placeablePos.Add(grid[check].worldPosition);
            }
        }
        return placeablePos;
    }
    private GameObject PlaceTunnel(Vector3 worldPosition)
    {
        worldPosition.x = SetToMid(worldPosition.x);
        worldPosition.y = SetToMid(worldPosition.y);
        worldPosition.z = 0;
        Node node = NodeGrid.NodeWorldPointPos(worldPosition);
        if (node.containsObject || !node.IsWalkable())
            return null;
        node.containsObject = true;
        PlayerLevelData.Instance.levelData.skillCoords.Add(new WorldCoords(worldPosition.x, worldPosition.y));
        return Instantiate(portalObject, worldPosition, Quaternion.identity);
    }
    public void OnDeselect()
    {
        if (entranceHole  != null && exitHole == null)
        {
            NodeGrid.NodeWorldPointPos(entranceHole.transform.position).containsObject = false;
            GameObject.Destroy(entranceHole);
            entranceHole = null;
            foreach (GameObject tile in placeableTiles)
                GameObject.Destroy(tile);
            PlayerLevelData.Instance.levelData.skillCoords.RemoveAt(0);
        }
    }
    public void OnClear(GameObject gameObject){}
}
