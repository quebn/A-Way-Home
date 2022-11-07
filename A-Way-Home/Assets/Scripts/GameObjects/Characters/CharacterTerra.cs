using System.Collections.Generic;
using UnityEngine;
public class CharacterTerra : Character, ICharacter
{
    [SerializeField] private GameObject platformObject;

    private void Start()
    {
        LoadPlatforms(platformObject);
    }


    public void PerformSkill(Vector3 position, Collider2D collider2D, string tag)
    {
        if(tag == "Obstacle" || PlayerLevelData.Instance.levelData.skillCount == 0)
            return;
        position.x = SetToMid(position.x);
        position.y = SetToMid(position.y);
        position.z = 0;
        Node node = NodeGrid.NodeWorldPointPos(position);
        if (node.containsObject)
            return;
        node.containsObject = true;
        node.type = NodeType.Walkable;
        Instantiate(platformObject, position, Quaternion.identity);
        PlayerLevelData.Instance.levelData.skillCount--;
        PlayerLevelData.Instance.levelData.skillCoords.Add(new WorldCoords(position.x, position.y));
        SetSkillCounter();
    }

    public void OnClear(GameObject gameObject)
    {
        ManipulationType type = gameObject.GetComponent<ObstacleData>().toolType;
        if (type == ManipulationType.Pickaxe)
        {
            PlayerLevelData.Instance.levelData.skillCount++;
            SetSkillCounter();
        }
    }

    public void OnDeselect(){}
}
