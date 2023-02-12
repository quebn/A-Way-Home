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
        DestroyNodeObstacles();
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

    public void AddAsSpawned(string id)
    {
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        this.id = id;
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(id), "ERROR: obstacle with id of {id} should not exist!");
        base.Initialize();
        animator.Play("Log_Spawn");
    }

    private void DestroyNodeObstacles()
    {
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.IsObstacle(typeof(GroundSpike)))
        {
            GroundSpike groundSpike = (GroundSpike)node.GetObstacle();
            groundSpike.TriggerDeath();
        }
        else if(node.IsObstacle(typeof(RockCrab)))
        {
            RockCrab rockCrab = (RockCrab)node.GetObstacle();
            rockCrab.TriggerDeath();
        }
        else if(node.IsObstacle(typeof(Rock)))
        {
            Rock rock = (Rock)node.GetObstacle();
            rock.ClearRock();
        }
        else if(node.IsObstacle(typeof(Plant)))
        {
            Plant plant = (Plant)node.GetObstacle();
            plant.DamagePlant();
        }
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
