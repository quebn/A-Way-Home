using System.Collections.Generic;
using UnityEngine;

public class CharacterTerra : Character, ICharacter
{
    [SerializeField] private GameObject portalObject;
    [SerializeField] private GameObject placeableTile;
    [SerializeField] private int skillRange;
    private GameObject entranceHole;
    private GameObject exitHole;
    // private List<Vector3[]> paths;
    private List<Vector3> placeablePositions;//TODO:maybe can be merge with placeable tiles
    private List<GameObject> placeableTiles; // <-----

    private void Start()
    {
        LoadPlatforms(portalObject);
    }

    public override void InitCharacter()
    {
        SetPaths();
        base.InitCharacter();
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
            path = Pathfinding.FindPath(transform.position, entranceHole.transform.position);
        }else{
            if (entranceHole != null && entranceHole == null)
            {
                GameObject.Destroy(entranceHole);
                entranceHole = null;
            }
            path = Pathfinding.FindPath(currentPos, homePosition);
        }
    }

    protected override bool EndConditions()
    {
        if (isHome){
            this.gameObject.SetActive(false);
            PlayerLevelData.Instance.homeAnimator.SetBool("Reached", true);
            return true;
        }
        if (targetIndex >= path.Length){
            transform.position = exitHole.transform.position;
            path = Pathfinding.FindPath(currentPos, homePosition);
            currentTargetPos = path[0];
            targetIndex = 0;
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

    public void PerformSkill(Vector3 position, Collider2D collider2D)
    {
        Node node = NodeGrid.NodeWorldPointPos(position);
        // Checks and returns if theres is an obstacle when placing the skill 
        if(node.currentType != NodeType.Walkable || node.containsObject || PlayerLevelData.Instance.levelData.skillCount == 0)
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
        worldPosition = SetToMid(worldPosition);
        Node node = NodeGrid.NodeWorldPointPos(worldPosition);
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
    public void OnToolUndo(ManipulationType manipulationType)
    {
        OnDeselect();
    }

    public void OnSkillUndo(ref Action action)
    {
        Debug.Assert(PlayerLevelData.Instance.levelData.actionList.Contains(action), "ERROR: ID should but is not present in action list");

        NodeGrid.NodeWorldPointPos(entranceHole.transform.position).RevertNode(); 
        NodeGrid.NodeWorldPointPos(exitHole.transform.position).RevertNode(); 
        
        GameObject.Destroy(entranceHole);
        GameObject.Destroy(exitHole);

        entranceHole = null;
        exitHole = null;

        PlayerLevelData.Instance.levelData.actionList.Remove(action);
        
        InGameUI.Instance.SetSkillCounter(1);
    }
}
