using UnityEngine;

public class PoisonMiasma : Obstacle, ITrap
{
    [SerializeField] private int damage;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Walkable, this);
        // SetNodes(this.worldPos, NodeType.Walkable, this);
    }



    public override void OnRevealNodeColor()
    {
        Node.RevealNodes(nodes, Node.colorPurple);
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(damage);
    }

    public void AddAsSpawned(string id)
    {
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        this.id = id;
        Debug.Assert(!GameData.levelData.obstacles.ContainsKey(id), "ERROR: obstacle with id of {id} should not exist!");
        base.Initialize();
    }

    public void Clear()
    {
        if(GameData.levelData.obstacles.ContainsKey(this.id))
            GameData.levelData.obstacles.Remove(id);
        ClearNodes();
        Debug.Log("Poison Miasma Cleared");
        GameObject.Destroy(this.gameObject);
    }

}
