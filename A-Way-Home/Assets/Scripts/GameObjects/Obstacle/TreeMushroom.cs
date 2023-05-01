using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeMushroom : TreeObstacle, ILightning, IActionWaitProcess
{
    [SerializeField] private List<GameObject> logs;
    private bool isFalling = false;
    
    public void OnLightningHit(int damage)
    {
        Debug.LogWarning(nodes[0].isConductive);
        Damage(damage);
        isFalling = (hitpoints == 1);
    }

    public void OnPlayerAction()
    {
        if(!isFalling)
            PlayerActions.FinishProcess(this);
    }

    protected override void OnCutDown()
    {
        for(int i = 0 ; i < placeableNodes[currentCursorLocation].Count; i++)
        {
            Node node = placeableNodes[currentCursorLocation][i]; 
            if(node.IsType(NodeType.Terrain) || (node.hasObstacle && !node.GetObstacle().isFragile) || node.hasPlatform || node == Character.instance.currentNode)
                continue;
            GameObject.Instantiate(
                node.currentType == NodeType.Water ? logs[1] : logs[0],
                node.worldPosition,
                Quaternion.identity
            );
        }
        isFalling = false;
        PlayerActions.FinishProcess(this);
    }
}
