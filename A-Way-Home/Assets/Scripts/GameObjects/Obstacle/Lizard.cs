using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Obstacle, ITremor, ICommand, ISelectable
{
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 fireStartPosDiff;
    [SerializeField] private Vector2Int fireDirectionDiff;
    [SerializeField] private List<GameObject> fireFields;
    
    private bool isBreathing => hitpoints == 1; 
    private List<Node> fireNodes;

    protected override void Initialize()
    {
        base.Initialize();
        Debug.Log("Lizard");
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        InitFireNodes();
        Invoke("OnStartBreath", .5f);
        // Invoke("OnStartBreath", WaitforFinishInit());
    }

    public void OnSelect(Tool tool)
    {
        for(int i = 0; i < fireNodes.Count; i ++)
            fireNodes[i].RevealNode();
    }

    public List<Node> OnSelectedHover(Vector3 mouseWorldPos, List<Node> currentNodes)
    {
        return currentNodes;
    }

    public void OnDeselect()
    {
        for(int i = 0; i < fireNodes.Count; i ++)
            fireNodes[i].Dehighlight();
        PlayerActions.FinishCommand();
    }

    public List<Node> IgnoredToggledNodes()
    {
        return fireNodes;
    }

    public bool OnCommand(List<Node> nodes)
    {
        ToggleFire();
        return true;
    }

    public void OnTremor()
    {
        Remove();
    }

    private void OnStartBreath()
    {
        if(isBreathing)
            BreathFire();
        animator.SetBool("isBreathing", isBreathing);
    }

    private void ToggleFire()
    {
        hitpoints = isBreathing ? 2 : 1;
        animator.SetBool("isBreathing", isBreathing);
        if(isBreathing)
            BreathFire();
        else
            DestroyFire();
    }

    private bool IfBurnable(Node node)
    {
        if(node.IsType(NodeType.Terrain))
            return false;
        if(!node.hasObstacle)
            return true;
        Debug.Assert(node.hasObstacle);
        return node.GetObstacle().isBurnable;
    }

    private void BreathFire()
    {
        if(fireNodes == null || fireNodes.Count <= 0)
            return;
        Debug.Log("Breathing Fire....");
        for(int i = 0; i < fireNodes.Count; i++)
        {
            if(!IfBurnable(fireNodes[i]))
                return;
            fireFields[i].SetActive(true);
            fireNodes[i].isBurning = true;
        }
    }

    private void DestroyFire()
    {
        for(int i = 0; i < fireNodes.Count; i++)
        {
            fireFields[i].SetActive(false);
            fireNodes[i].isBurning = false;
        }
    }

    private void InitFireNodes()
    {
        fireNodes = new List<Node>();
        for(int i = 0; i < fireFields.Count; i++)
        {
            Node node = NodeGrid.NodeWorldPointPos(fireFields[i].transform.position);
            if(!NodeGrid.Instance.grid.ContainsValue(node))
                return;
            fireNodes.Add(node);
        }
    }


}
