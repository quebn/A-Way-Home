using UnityEngine;

public class Rock : Obstacle, IObstacle
{
    private Animator animator;
    private bool isWalking = false;

    private void Update()
    {
        while(isWalking)
            GoToRandomNode();
    }

    protected override void Initialize()
    {
        base.Initialize();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nodesType = NodeType.Obstacle;
    }

    public void OnClick()
    {
        if (incorrectTool)
            return;
        RemoveRock();
    }

    public void OnHover()
    {
        spriteRenderer.color = Color.green;
    }

    public void OnDehover()
    {
        spriteRenderer.color = Color.white;
    }

    private void RemoveRock()
    {
        InGameUI.Instance.SetPlayerMoves(-1);
        isWalking = true;
        animator.SetBool("isWalking", isWalking);
    }

    private void GoToRandomNode()
    {

    }
}
