using UnityEngine;

public class WildFruitSpawn : SpawnablePlant, ITrap
{
    protected override void OnSpawn()
    {
        DestroyNodeObstacle();
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public override void OnLightningHit(int damage)
    {
        Damage(damage);
    }

    public override void OnGrow()
    {
        return;
    }

    public void OnTrapTrigger(Character character)
    {
        Debug.Log("Triggered!");
        character.IncrementEnergy(heal);
        Remove();
    }

    public override void Damage(int damage)
    {
        Remove();
    }
}