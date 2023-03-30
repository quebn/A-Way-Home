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
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        ClearNodes();
        Debug.Log("Poison Miasma Cleared");
        GameObject.Destroy(this.gameObject);
    }

    private void DestroyNodeObstacle()
    {
        Node node = NodeGrid.NodeWorldPointPos(this.worldPos);
        if(node.hasObstacle)
            Destroy(node.GetObstacle());
    }
}
