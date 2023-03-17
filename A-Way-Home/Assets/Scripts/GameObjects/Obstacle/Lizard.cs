using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Obstacle, ICommand
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fireField;
    [SerializeField] private int fireRange;

    [SerializeField] private Vector2 fireStartPosDiff;
    [SerializeField] private Vector2Int fireDirectionDiff;
    
    private List<FireField> fireFields;
    private List<Node> fireNodes;
    private bool isBreathing { get => animator.GetBool("isBreathing"); set => animator.SetBool("isBreathing", value);}

    public void OnCommand()
    {
        ToggleFire();
        Debug.Log("Breathing Fire....");
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        InitFireNodes();
    }

    private void ToggleFire()
    {
        isBreathing = !isBreathing;
        if(isBreathing)
            BreathFire();
        else
            DestroyFire();
    }

    private void BreathFire()
    {
        if(fireNodes == null || fireNodes.Count <= 0)
            return;
        foreach(Node node in fireNodes)
        {
            if(node.IsType(NodeType.Walkable))
            {
                if(node.hasObstacle)
                {
                    if(node.IsObstacle(typeof(GroundSpike)))
                    {
                        GroundSpike spike = (GroundSpike)node.GetObstacle();
                        spike.Remove();
                        Debug.LogWarning("Cleared Spike");
                    }
                    else if(node.IsObstacle(typeof(RockCrab)))
                    {
                        RockCrab crab = (RockCrab)node.GetObstacle();
                        crab.Remove();
                    }
                    else if(node.IsObstacle(typeof(PlantEnergy)))
                    {
                        PlantEnergy plant = (PlantEnergy)node.GetObstacle();
                        plant.Remove();
                        Debug.LogWarning("Cleared Plant");
                    }
                    else if(node.IsObstacle(typeof(PlantPoison)))
                    {
                        PlantPoison plant = (PlantPoison)node.GetObstacle();
                        plant.Remove();
                        Debug.LogWarning("Cleared Plant");
                    }
                    else if(node.IsObstacle(typeof(Undead)))
                    {
                        Undead undead = (Undead)node.GetObstacle();
                        undead.TriggerDeath(true);
                    }
                }
            }
            else if(node.IsType(NodeType.Obstacle))
            {
                if(node.IsObstacle(typeof(TreeThin)))
                {
                    TreeThin tree = (TreeThin)node.GetObstacle();
                    tree.DestroyCompletely();
                }
                else if(node.IsObstacle(typeof(TreeLog)))
                {
                    TreeLog log = (TreeLog)node.GetObstacle();
                    log.Clear();
                }
                else if(node.IsObstacle(typeof(Undead)))
                {
                    Undead undead = (Undead)node.GetObstacle();
                    undead.TriggerDeath(true);
                }
                else if(node.IsObstacle(typeof(PlantEnergy)))
                {
                    PlantEnergy plant = (PlantEnergy)node.GetObstacle();
                    plant.Remove();
                    Debug.LogWarning("Cleared Plant");
                }
                else if(node.IsObstacle(typeof(PlantPoison)))
                {
                    PlantPoison plant = (PlantPoison)node.GetObstacle();
                    plant.Remove();
                    Debug.LogWarning("Cleared Plant");
                }
                else
                    return;
            }
            else return;
            FireField fire = GameObject.Instantiate(fireField, node.worldPosition, Quaternion.identity, this.transform).GetComponent<FireField>();
            fire.AddAsSpawned($"{GameData.levelData.spawnCount += 1}");
            fireFields.Add(fire);
        }
    }

    private void DestroyFire()
    {
        foreach(FireField fire in fireFields)
            fire.Clear();
        fireFields = new List<FireField>();
    }

    private void InitFireNodes()
    {
        fireNodes = new List<Node>();
        fireFields = new List<FireField>();
        Node fireStartNode = NodeGrid.NodeWorldPointPos(this.worldPos + fireStartPosDiff);
        Vector2Int gridPosIncrement = new Vector2Int(fireStartNode.gridPos.x, fireStartNode.gridPos.y);
        for(int i = 0; i < fireRange; i++)
        {
            if(!NodeGrid.Instance.grid.ContainsKey(gridPosIncrement))
                return;
            Node node = NodeGrid.Instance.grid[gridPosIncrement];
            // if(node.IsType(NodeType.Walkable))// && !node.hasObstacle)
            fireNodes.Add(node);
            // else 
                // return;
            gridPosIncrement += fireDirectionDiff;
        }
    }

}
