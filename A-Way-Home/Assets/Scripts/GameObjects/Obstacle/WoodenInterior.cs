public class WoodenInterior : Obstacle, ILightning
{
    public override bool isBurnable => true;
    public override bool isMeltable => true;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
    }

    public void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public override void Damage(int value)
    {
        DamageAnimation();
        if(hitpoints > 0)
            hitpoints -= value;
        if(hitpoints > 0)
            return;
        Remove();
    }

    public override void Remove()
    {
        audioSources[0].Play();
        base.Remove();
    }
}