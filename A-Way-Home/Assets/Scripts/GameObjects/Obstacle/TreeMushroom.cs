using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMushroom : TreeObstacle, ILightning
{
    [SerializeField] private List<GameObject> logs;

    public void OnLightningHit()
    {
        Damage(1);
    }

    protected override void OnCutDown()
    {
        for(int i = 0 ; i < placeableNodes[currentCursorLocation].Count; i++)
        {
            Node node = placeableNodes[currentCursorLocation][i]; 
            if(node.IsType(NodeType.Terrain) || (node.hasObstacle && !node.GetObstacle().isFragile))
                continue;
            GameObject.Instantiate(
                logs[0],
                // node.currentType == NodeType.Water ? logs[1] : logs[0],
                node.worldPosition,
                Quaternion.identity
            );
        }
    }
}
