using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Obstacle, ITremor, ICommand, ISelectable, IActionWaitProcess
{
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 fireStartPosDiff;
    [SerializeField] private Vector2Int fireDirectionDiff;
    // [SerializeField] private Vector2 mouthPosDiff;
    private List<Node> fireNodes;
    private Node fireNodeOrigin;
    private bool isBreathing => hitpoints % 2 == 0; 

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        InitFireNodes();
        Invoke("OnStartBreath", .5f);
        // Invoke("OnStartBreath", WaitforFinishInit());
    }


    public void OnPlayerAction()
    {
        ToggleFire();
        PlayerActions.FinishProcess(this);
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
        PlayerActions.FinishCommand(this);
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
        Damage(2);
    }

    public override void Remove()
    {
        if(isBreathing)
            DestroyFire();
        audioSources[0].Play();
        base.Remove();
    }

    private void OnStartBreath()
    {
        if(isBreathing)
            FireNode.StartFire(fireNodeOrigin, fireDirectionDiff, 3);
        animator.SetBool("isBreathing", isBreathing);
    }

    private void ToggleFire()
    {
        hitpoints = isBreathing ? 5 : 4;
        animator.SetBool("isBreathing", isBreathing);
        if(isBreathing)
            BreathFire();
        else
            DestroyFire();
    }

    private void BreathFire()
    {
        if(fireNodes == null || fireNodes.Count <= 0)
            return;
        audioSources[1].Play();
        FireNode.StartFire(fireNodeOrigin, fireDirectionDiff, 3);
    }

    private void DestroyFire()
    {
        FireNode.StopFire(fireNodeOrigin);
    }

    private void InitFireNodes()
    {
        fireNodes = new List<Node>();
        
        fireNodeOrigin = NodeGrid.NodeWorldPointPos(this.worldPos + fireStartPosDiff);
        Node currentFireNode = fireNodeOrigin;
        Vector2Int gridPosIncrement = new Vector2Int(currentFireNode.gridPos.x, currentFireNode.gridPos.y);
        for(int i = 0; i < 3; i++)
        {
            gridPosIncrement += fireDirectionDiff;
            fireNodes.Add(currentFireNode);
            if(!NodeGrid.Instance.grid.ContainsKey(gridPosIncrement))
                continue;
            currentFireNode.fireNode.childNode = NodeGrid.Instance.grid[gridPosIncrement];
            currentFireNode = currentFireNode.fireNode.childNode;
        }
    }

}
