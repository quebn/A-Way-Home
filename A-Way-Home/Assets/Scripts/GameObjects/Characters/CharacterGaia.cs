using System.Collections.Generic;
using UnityEngine;

public class CharacterGaia : Character, ICharacter
{
    [SerializeField] private GameObject platformObject;

    private void Start()
    {
        LoadPlatforms(platformObject);
    }


    public void PerformSkill(Vector3 position, Collider2D collider2D)
    {
        position = SetToMid(position);
        Node node = NodeGrid.NodeWorldPointPos(position);
        if(node.currentType == NodeType.Obstacle || node.IsWalkable() || node.containsObject|| PlayerLevelData.Instance.levelData.skillCount == 0)
            return;
        node.containsObject = true;
        node.currentType = NodeType.Walkable;
        node.SetColor();
        GameObject platform = Instantiate(platformObject, position, Quaternion.identity);
        PlayerLevelData.gameObjectList.Add($"{platform.transform.position.ToString()}", platform);
        PlayerLevelData.Instance.levelData.actionList.Add(new Action(ManipulationType.UniqueSkill ,position, $"{platform.transform.position.ToString()}"));
        InGameUI.Instance.SetSkillCounter(-1);
    }

    public void OnClear(GameObject gameObject)
    {
        ManipulationType type = gameObject.GetComponent<ObstacleData>().toolType;
        if (type == ManipulationType.Pickaxe)
            InGameUI.Instance.SetSkillCounter(1);
    }
    public void OnToolUndo(ManipulationType manipulationType)
    {
        if (manipulationType == ManipulationType.Pickaxe)
            InGameUI.Instance.SetSkillCounter(-1);
    }
    public void OnSkillUndo(ref Action action)
    {
        Debug.Assert(PlayerLevelData.gameObjectList.ContainsKey(action.obstacleID), "ERROR: ID should but is not present in GameObject list");
        PlayerLevelData.gameObjectList[action.obstacleID].SetActive(true);
        
        GameObject.Destroy(PlayerLevelData.gameObjectList[action.obstacleID]);
        PlayerLevelData.gameObjectList.Remove(action.obstacleID);
        
        bool check = PlayerLevelData.Instance.levelData.actionList.Remove(action);
        Debug.Log($"action removed from list: {check}");
        
        InGameUI.Instance.SetSkillCounter(1);
        NodeGrid.NodeWorldPointPos(action.skillCoord).RevertNode(); 
    }
}
