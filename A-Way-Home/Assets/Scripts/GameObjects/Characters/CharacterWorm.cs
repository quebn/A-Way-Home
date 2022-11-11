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
    private List<Vector3> placeablePositions;//TODO: can be merge with paleable tiles
    private List<GameObject> placeableTiles; // <-----

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

    protected override void LoadPlatforms(GameObject spawnedObject)
    {
        List<Action> skillActions = new List<Action>();
        foreach(Action action in PlayerLevelData.Instance.levelData.actionList)
            if(action.type == ManipulationType.UniqueSkill)
                skillActions.Add(action);
        if (skillActions.Count == 0)
            return;
        foreach(Action action in PlayerLevelData.Instance.levelData.actionList){
            if (action.type == ManipulationType.UniqueSkill){
                entranceHole = GameObject.Instantiate(spawnedObject, action.GetCoordByIndex(0), Quaternion.identity);
                exitHole = GameObject.Instantiate(spawnedObject, action.GetCoordByIndex(1), Quaternion.identity);
            }
        }
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
            InGameUI.Instance.SetCharacterEnergy(-1);
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
            WorldCoords[] coords = new WorldCoords[2]{
                new WorldCoords(entranceHole.transform.position), 
                new WorldCoords(exitHole.transform.position)
            };
            PlayerLevelData.Instance.levelData.actionList.Add(new Action(ManipulationType.UniqueSkill, coords));
            foreach (GameObject tile in placeableTiles)
                GameObject.Destroy(tile);
            InGameUI.Instance.SetSkillCounter(-1);
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
        return Instantiate(portalObject, worldPosition, Quaternion.identity);
    }

    public void OnDeselect()
    {
        if (entranceHole  != null && exitHole == null)
        {
            NodeGrid.NodeWorldPointPos(entranceHole.transform.position).containsObject = false;
            GameObject.Destroy(entranceHole);
            entranceHole = null;
            foreach (GameObject tile in placeableTiles)//Destroys the tunnel range visual tiles
                GameObject.Destroy(tile);
        }
    }

    public void OnSkillUndo(ref Action action)
    {
        Debug.Assert(PlayerLevelData.Instance.levelData.actionList.Contains(action), "ERROR: ID should but is not present in action list");
        // PlayerLevelData.gameObjectList[action.obstacleID].SetActive(true);

        NodeGrid.NodeWorldPointPos(entranceHole.transform.position).RevertNode(); 
        NodeGrid.NodeWorldPointPos(exitHole.transform.position).RevertNode(); 
        
        GameObject.Destroy(entranceHole);
        GameObject.Destroy(exitHole);

        entranceHole = null;
        exitHole = null;

        bool check = PlayerLevelData.Instance.levelData.actionList.Remove(action);
        Debug.Log($"action removed from list: {check}");
        
        InGameUI.Instance.SetSkillCounter(1);
    }
}
