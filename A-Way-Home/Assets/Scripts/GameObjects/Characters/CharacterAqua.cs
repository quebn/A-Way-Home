using UnityEngine;

public class CharacterAqua : Character, ICharacter
{
    public override void InitCharacter()
    {
        path = Pathfinding.FindPath(currentPos, homePosition, true);
        Debug.Log(path.Length);
        if (path.Length <=0)
            return;
        currentTargetPos = path[0];
        targetIndex = 0;
        isGoingHome = true;
    }

    public void PerformSkill(Vector3 position, Collider2D collider2D)
    {
        if(collider2D == null || collider2D.gameObject.tag != "Obstacle")
            return;
        ObstacleData obstacleData = collider2D.gameObject.GetComponent<ObstacleData>();
        if(obstacleData.toolType == ManipulationType.UniqueSkill)
        {
            collider2D.gameObject.SetActive(false);
            PlayerLevelData.AddRemovedToList(obstacleData.toolType, obstacleData.ID, true);
            InGameUI.Instance.SetPlayerMoves(-1);
            InGameUI.Instance.SetSkillCounter(1);
        }
    }

    protected override bool EndConditions()
    {
        Node node = NodeGrid.NodeWorldPointPos(currentPos);
        if (PlayerLevelData.Instance.levelData.skillCount < 1 && node.currentType == NodeType.Water)
        {
            Debug.Log("Not enough skill count to traverse water.");
            isGoingHome = false;
            animator.SetBool("isWalk", false);
            return true;
        }
        if (node.currentType == NodeType.Water)
            InGameUI.Instance.SetSkillCounter(-1);
        return base.EndConditions();
    }

    public void OnSkillUndo(ref Action action)
    {
        Debug.Assert(PlayerLevelData.Instance.levelData.removedObstacles.ContainsKey(action.obstacleID), "ERROR: ID should but is not present in removed list");
        PlayerLevelData.gameObjectList[action.obstacleID].SetActive(true);
        
        PlayerLevelData.Instance.levelData.removedObstacles.Remove(action.obstacleID);
        PlayerLevelData.Instance.levelData.actionList.Remove(action);
        
        InGameUI.Instance.SetPlayerMoves(1);
        InGameUI.Instance.SetSkillCounter(-1);
    }
}
