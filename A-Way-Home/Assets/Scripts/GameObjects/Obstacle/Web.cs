using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : Obstacle, IInteractable
{

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnDehighlight()
    {
        throw new System.NotImplementedException();
    }

    public void OnHighlight()
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract()
    {
        throw new System.NotImplementedException();
    }

    public void AddAsSpawned(string id)
    {
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        this.id = id;
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(id), "ERROR: obstacle with id of {id} should not exist!");
        base.Initialize();
    }

    public void Clear()
    {
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        ClearNodes();
        Debug.Log("Poison Miasma Cleared");
        GameObject.Destroy(this.gameObject);
    }
}
