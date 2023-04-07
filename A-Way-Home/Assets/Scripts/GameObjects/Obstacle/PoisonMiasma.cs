using UnityEngine;

public class PoisonMiasma : Spawnable, ITrap
{
    [SerializeField] private int damage;

    protected override void OnSpawn()
    {
        DestroyNodeObstacle();
        base.OnSpawn();
        SetNodes(this.worldPos, NodeType.Walkable, this);
        // SetNodes(this.worldPos, NodeType.Walkable, this);
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(damage);
    }

    public override void Remove()
    {
        ClearNodes();
        Debug.Log("Poison Miasma Cleared");
        GameObject.Destroy(this.gameObject);
    }
}
