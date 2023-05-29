using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeMushroom : TreeObstacle, ILightning, IActionWaitProcess
{
    [SerializeField] private List<GameObject> logs;
    private bool isFalling = false;
    
    protected override Action AfterCutDown => OnCutDown;

    public void OnLightningHit(int damage)
    {
        Debug.LogWarning(nodes[0].IsStatus(NodeStatus.Conductive));
        Damage(damage);
        isFalling = (hitpoints == 1);
    }

    public void OnPlayerAction()
    {
        if(!isFalling)
            PlayerActions.FinishProcess(this);
    }

    private void OnCutDown()
    {
        int index = IsCursorRight()? 1 : 0;
        for(int i = 0 ; i < nodesPlaceable[index].Count; i++)
        {
            Node node = nodesPlaceable[index][i]; 
            if(LogNotPlaceable(node) || node.hasPlatform)
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
