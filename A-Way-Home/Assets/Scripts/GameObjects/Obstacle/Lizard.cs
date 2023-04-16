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
    [SerializeField] private bool isBreathing; 
    
    private List<FireField> fireFields;
    private List<Node> fireNodes;

    public void OnCommand(List<Node> nodes)
    {
        ToggleFire();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        InitFireNodes();
        if(isBreathing)
            BreathFire();
        animator.SetBool("isBreathing", isBreathing);
    }

    private void ToggleFire()
    {
        isBreathing = !isBreathing;
        animator.SetBool("isBreathing", isBreathing);
        if(isBreathing)
            BreathFire();
        else
            DestroyFire();
    }

    private bool IfBurnable(Node node)
    {
        if(node.hasPlatform) 
            return node.GetObstacle(true).isBurnable; 
        if(node.IsWalkable() && !node.hasObstacle)
            return true;
        if(node.IsType(NodeType.Terrain) || node.IsType(NodeType.Water))
            return false;
        Debug.Log(node.GetObstacle().isBurnable);
        return node.GetObstacle().isBurnable;
    }

    private void BreathFire()
    {
        if(fireNodes == null || fireNodes.Count <= 0)
            return;
        Debug.Log("Breathing Fire....");
        foreach(Node node in fireNodes)
        {
            if(!IfBurnable(node))
                return;
            fireFields.Add(GameObject.Instantiate(fireField, node.worldPosition, Quaternion.identity, this.transform).GetComponent<FireField>());
        }
    }

    private void DestroyFire()
    {
        foreach(FireField fire in fireFields)
            fire.Remove();
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
            fireNodes.Add(node);
            gridPosIncrement += fireDirectionDiff;
        }
    }

}
