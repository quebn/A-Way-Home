using UnityEngine;
using System.Collections.Generic;

public class Rock : Obstacle//, IInteractable
{
    // [SerializeField] private int travelRange;
    // private Animator animator;
    // private bool isWalking = false;
    // private Node nodeDestination;
    // private Vector3[] path;
    // private Vector3 currentTargetPos;
    // private int targetIndex;

    // private void Update()
    // {
    //     if(isWalking)
    //         GoToRandomNode();
    // }

    // protected override void Initialize()
    // {
    //     base.Initialize();
    //     animator = GetComponent<Animator>();
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     InitializeNodes(this.transform.position);
    //     SetRandomNodeDestPath();
    //     SetNodesType(NodeType.Obstacle);
    // }

    // public void OnClick()
    // {
    //     if (incorrectTool)
    //         return;
    //     RemoveRock();
    // }

    // public void OnHover()
    // {
    //     if (incorrectTool)
    //         return;
    //     spriteRenderer.color = Color.green;
    // }

    // public void OnDehover()
    // {
    //     spriteRenderer.color = Color.white;
    // }

    // private void RemoveRock()
    // {
    //     SetNodesType(NodeType.Obstacle);
    //     PlayerLevelData.Instance.IncrementPlayerMoves(-1);
    //     isWalking = true;
    //     animator.SetBool("isWalking", isWalking);
    // }

    // private void GoToRandomNode()
    // {
    //     if(this.transform.position == currentTargetPos)
    //     {
    //         targetIndex++;
    //         if (this.transform.position == nodeDestination.worldPosition)
    //             SettleDown();
    //         currentTargetPos = path[targetIndex];
    //     }
    //     this.transform.position = Vector3.MoveTowards(this.transform.position, currentTargetPos, 10f * Time.deltaTime);
    // }

    // private void SettleDown()
    // {
    //     nodes[0] = nodeDestination;
    //     Debug.Assert(nodes.Count == 1, "Node list isnt 1");
    //     nodes[0].currentType = NodeType.Obstacle;
    //     isWalking = false;
    //     animator.SetBool("isWalking", isWalking);
    //     SetRandomNodeDestPath();
    // }

    // // private int counter = 0;
    // private void SetRandomNodeDestPath()
    // {
    //     // counter++;
    //     // List<Node> nodeInRange = NodeGrid.GetNodesByRange(nodes[0], travelRange);
    //     // int randomIndex = Random.Range(0, nodeInRange.Count);
    //     // Debug.Log($"Random Number: {randomIndex} range:{nodeInRange.Count}");
    //     // nodeDestination = nodeInRange[randomIndex];
    //     // path = Pathfinding.FindPath(nodes[0].worldPosition, nodeDestination.worldPosition);
    //     // if (path == null || path.Length <= 0){
    //     //     SetRandomNodeDestPath();
    //     //     return;
    //     // }
    //     // currentTargetPos = path[0];
    //     // targetIndex = 0;
    // }
}
