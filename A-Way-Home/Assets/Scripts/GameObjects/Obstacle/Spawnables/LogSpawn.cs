public class LogSpawn : Spawnable, ILightning
{
    public override bool isBurnable => true;
    public override bool isFragile => true;
    public override bool isMeltable => true;
    public override bool isCorrosive => true;

    protected override void OnSpawn()
    {
        DestroyNodeObstacle();
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public override void Remove()
    {
        audioSources[0].Play();
        base.Remove();
    }
}
