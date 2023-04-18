using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : Obstacle, IActionWaitProcess, ILightning
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject webPrefab;

    private Node currentTargetNode;
    private Node lastNode;
    private List<Node> walkableNodes;
    private bool canWeb => hitpoints > 1;

    private bool isMoving {
        get => animator.GetBool("isMoving"); 
        set => animator.SetBool("isMoving", value); 
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        walkableNodes = NodeGrid.GetPathNeighborNodes(nodes[0], NodeGrid.Instance.grid);
        Debug.Assert(walkableNodes.Count > 0);
    }

    public void OnLightningHit()
    {
        Remove();
    }

    // public bool OnCommand(List<Node> nodes)
    // {
    //     hitpoints = canWeb? 1 : 2;
    // }


    public void OnPlayerAction()
    {
        if(TryGetPath())
            StartCoroutine(FollowPath());
    }

    public IEnumerator FollowPath()
    {
        while(isMoving)
        {
            if(this.transform.position == currentTargetNode.worldPosition)
            {
                Stop();
                yield break;
            }
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetNode.worldPosition, 5f * Time.deltaTime);
            yield return null;
        }
    }

    private bool TryGetPath()
    {
        if(walkableNodes == null || walkableNodes.Count == 0 || hitpoints <= 0 || !NodeGrid.IfNeigbhorsWalkable(nodes[0]))
        {
            PlayerActions.FinishProcess(this);
            return false;
        }
        SetCurrentTargetNode();
        Debug.LogWarning($"Targetnode pos => {currentTargetNode.worldPosition}");
        lastNode = nodes[0];
        ClearNodes();
        SpawnWeb();
        isMoving = true;
        return true;
    }

    private void SetCurrentTargetNode()
    {
        currentTargetNode = null;
        int randomIndex = UnityEngine.Random.Range(0, walkableNodes.Count);
        if(walkableNodes[randomIndex].IsWalkable())
            currentTargetNode = walkableNodes[randomIndex];
        else
            SetCurrentTargetNode();
    }

    private void Stop()
    {
        isMoving = false;
        PlayerActions.FinishProcess(this);
        if(currentTargetNode.hasObstacle)
            OnWalkableObtacles();
        currentTargetNode = null;
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        walkableNodes = NodeGrid.GetPathNeighborNodes(nodes[0], NodeGrid.Instance.grid);
    }

    private void OnWalkableObtacles()
    {
        if(currentTargetNode.IsObstacle(typeof(Plant)))
            Destroy(currentTargetNode.GetObstacle());
        else
            currentTargetNode.GetObstacle().Destroy(this);
    }
    private void SpawnWeb()
    {
        GameObject.Instantiate(webPrefab, lastNode.worldPosition, Quaternion.identity);
    }

}
