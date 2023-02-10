using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle, IInteractable//, IInteractable, INodeInteractable
{

    // public bool notSpawned;
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.transform.position, NodeType.Obstacle, this);
    }

    public void OnInteract()
    {
        if(currentTool == Tool.Lightning)
            RemoveLog();
    }

    public void OnHighlight()
    {
        if(currentTool != Tool.Lightning)
            return;
        this.spriteRenderer.color = Color.green;
    }

    public void OnDehighlight()
    {
        if(currentTool != Tool.Lightning)
            return;
        this.spriteRenderer.color = Color.white;
    }

    public void AddAsSpawned()
    {
        if(!GameData.levelData.spawneds.ContainsKey(id))
            GameData.levelData.spawneds.Add(id, new SpawnedData(this.GetType().ToString(), hitpoints, this.worldPos));
        if(GameData.levelData.obstacles.ContainsKey(id))
        {
            Debug.LogWarning($"{id} Was in obstacles and is being removed.");
            GameData.levelData.obstacles.Remove(id);
        }
        // animation spawn here.
        animator.Play("Log_Spawn");
    }

    private void RemoveLog()
    {
        hitpoints -= 1;
        ClearNodes();
        Debug.Assert(hitpoints == 0, "ERROR: Hitpoints should be 0!");
        this.gameObject.SetActive(false);
        // if(GameData.levelData.spawneds.ContainsKey(id))
        //     GameData.levelData.spawneds.Remove(id);
        // else if(GameData.levelData.obstacles.ContainsKey(id))
        //     GameData.levelData.obstacles[id] = hitpoints;
    }

    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        // Debug.LogWarning($"loaded hp: {hitpoints}"/);
        if(hitpoints != 0)
            return;
        ClearNodes();
        Debug.Assert(hitpoints == 0, "ERROR: Hitpoints should be 0!");
        this.gameObject.SetActive(false);
    }
}
