using System.Collections.Generic;

public class PlantPoison : Plant, ITrap
{
    private List<Node> tilesPoisoned;

    public override bool isCorrosive => false;

    protected override void Initialize()
    {
        base.Initialize();
        tilesPoisoned = NodeGrid.GetNeighborNodeList(this.nodes[0], NodeGrid.Instance.grid, 1);
    }

    public void OnTrapTrigger(Character character)
    {
        character.IncrementEnergy(heal);
        character.DamageAnimation();
        Damage(hitpoints);
    }

    public override void OnLightningHit(int damage)
    {
        base.OnLightningHit(damage);
        if(hitpoints < 4)
            RemoveMiasma();
    }

    public override void OnGrow()
    {
        base.OnGrow();
        GeneratePoisonTiles();
    }

    public override void Remove()
    {
        base.Remove();
        RemoveMiasma();
    }

    private void GeneratePoisonTiles()
    {
        for(int i = 0; i < tilesPoisoned.Count; i++)
        {
            if(!IsCorrosive(tilesPoisoned[i]))
                continue;
            tilesPoisoned[i].SetStatus(NodeStatus.Corrosive);
            Destroy(tilesPoisoned[i]);
        }
    }

    public override void Damage(int damage)
    {
        DamageAnimation();
        hitpoints -= damage;
        if(hitpoints < 2)
            hitpoints = 0;
        animator.Play(CurrentAnimationName());
        SetNodes(this.worldPos, isAdult ? NodeType.Obstacle: NodeType.Walkable, this);
        if(hitpoints <= 0)
            Remove();
    }

    private bool IsCorrosive(Node node)
    {
        if(node.hasPlatform) 
            return node.GetObstacle(true).isCorrosive; 
        if(node.IsWalkable() && !node.hasObstacle)
            return true;
        if(node.IsType(NodeType.Terrain) || node.IsType(NodeType.Water))
            return false;
        return node.GetObstacle().isCorrosive;
    }

    private void RemoveMiasma()
    {
        for(int i = 0; i < tilesPoisoned.Count; i++)
            if(tilesPoisoned[i].IsStatus(NodeStatus.Corrosive))
                tilesPoisoned[i].SetStatus(NodeStatus.None);
    }

}