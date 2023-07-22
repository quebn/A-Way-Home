public class WebSpawn : Spawnable, ILightning
{
    public override bool isBurnable => true;
    public override bool isMeltable => true;
    public override bool isFragile => true;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }
}
