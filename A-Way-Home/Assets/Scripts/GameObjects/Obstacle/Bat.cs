using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Obstacle , IInteractable
{
    [SerializeField] private GameObject batGameObject;
    [SerializeField] private GameObject destination;
    [SerializeField] private bool isAwake;

    private Animator animator;

    private Color setNodes {
        set {
            foreach (Node node in nodes)
                node.tileRenderer.color = value;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        destination.SetActive(false);
        spriteRenderer = batGameObject.GetComponent<SpriteRenderer>();
        animator = batGameObject.GetComponent<Animator>();
        SetNodes(destination.transform.position);
        if (isAwake)
            GoToDestination();
    }

    public void OnClick()
    {
        // Should show node tile to place the bat 
        if (incorrectTool)
            return;
        Debug.Log("Interacted!");
        isAwake = true;
        GoToDestination();
        PlayerLevelData.Instance.SetPlayerMoves(-1);
    }

    public void OnHover()
    {
        // Should show the nodes the bat blocks when hovered
        SetMouseCursor(this.mouseTexture);
        HighlightNodes();
    }

    public void OnDehover()
    {
        ResetMouseCursor();
        if (isAwake)
            return;
        setNodes = Node.colorWhite;
    }

    private void HighlightNodes()
    {
        setNodes = Node.colorRed;
    }

    private void GoToDestination()
    {
        // Make the Bat wake up and go to its destination.
        animator.Play("Bat_Flying");
        batGameObject.transform.position = destination.transform.position;
        ClosePath();
    }

    private void ClosePath()
    {
        // Make the node that the bat overlaps unwalkable
        this.gameObject.tag = "Interacted";
        nodesType = NodeType.Obstacle;
    }
}
