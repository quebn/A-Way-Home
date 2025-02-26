using UnityEngine;

public class Lilypad : Obstacle, ILightning
{
    [SerializeField] private Animator animator;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this, true);
    }

    public override void Remove()
    {
        ClearNodes(NodeType.Water, true);
        base.Remove();
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }
}