using System.Collections.Generic;
using UnityEngine;
public class CharacterTerra : Character, ICharacter
{
    [SerializeField] private GameObject platformObject;

    private void Start()
    {
        LoadPlatforms(platformObject);
    }


    public void PerformSkill(Vector3 position)
    {
        
        if(PlayerLevelData.Instance.levelData.skillCount == 0)
        {        
            Debug.Log("You have 0 rocks to use!");
            return;
        }
        position.x = SetToMid(position.x);
        position.y = SetToMid(position.y);
        position.z = 0;
        Vector2 gridCoord = new Vector2();
        foreach (KeyValuePair<Vector2, Node> pair in NodeGrid.Instance.grid)
        {
            if (pair.Value.worldPosition == position)
            {
                gridCoord = pair.Key;
                if (pair.Value.containsObject)
                {
                    Debug.Log($"UNABLE TO PLACE: Node[{gridCoord.x},{gridCoord.y}] already has a platform");
                    return;
                }
                break;
            }
        }
        NodeGrid.Instance.grid[gridCoord].containsObject = true;
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
}
