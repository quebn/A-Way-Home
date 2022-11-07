using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAquatic : Character, ICharacter
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

    public override void DisplayPath(bool toggle)
    {
        Vector3[] nodePath = Pathfinding.FindPath(currentPos, homePosition, true);
        if (nodePath.Length == 0)
        {
            Debug.Log("no path to be shown.");
            return;
        }
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
    public void PerformSkill(Vector3 position, Collider2D collider2D, string tag)
    {
        if(collider2D == null && tag != "Obstacle")
            return;
        ObstacleData obstacleData = collider2D.gameObject.GetComponent<ObstacleData>();
        if(obstacleData.toolType == ManipulationType.UniqueSkill)
        {
            collider2D.gameObject.SetActive(false);
            PlayerLevelData.Instance.levelData.skillCount++;
            PlayerLevelData.Instance.levelData.removedObstacles.Add(obstacleData.ID, true);
            InGameUI.Instance.SetPlayerMoves();
            SetSkillCounter();
        }
    }
    protected override void GoHome()
    {
        Node node = NodeGrid.NodeWorldPointPos(currentPos);
        if (PlayerLevelData.Instance.levelData.skillCount < 1 && node.type == NodeType.Water)
        {
            Debug.Log("Not enough skill count to traverse water.");
            isGoingHome = false;
            animator.SetBool("isWalk", false);
            return;
        }
        if (node.type == NodeType.Water && currentPos == currentTargetPos)
        {
            PlayerLevelData.Instance.levelData.skillCount--;
            SetSkillCounter();
        }
        base.GoHome();
    }
    public void OnClear(GameObject gameObject){}
    public void OnDeselect(){}

}
