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

    private bool IfBurnable(Node node)
    {
        if(node.IsWalkable() && !node.hasObstacle )
            return true;
        if(node.IsType(NodeType.Terrain) || node.IsType(NodeType.Water))
            return false;
        return node.GetObstacle().isBurnable;
    }

    private void BreathFire()
    {
        if(fireNodes == null || fireNodes.Count <= 0)
            return;
        foreach(Node node in fireNodes)
        {
            FireField fire = GameObject.Instantiate(fireField, node.worldPosition, Quaternion.identity, this.transform).GetComponent<FireField>();
            fireFields.Add(fire);
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
            if(IfBurnable(node))
                fireNodes.Add(node);
            gridPosIncrement += fireDirectionDiff;
        }
    }

}
