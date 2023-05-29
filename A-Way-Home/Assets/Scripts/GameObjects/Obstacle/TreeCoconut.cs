using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCoconut : TreeObstacle, ILightning, IActionWaitProcess
{
    [SerializeField] private List<GameObject> logs;
    [SerializeField] private bool hasNut;
    
    private bool isFalling = false;
    protected override Action<bool> AfterCutDown => OnCutDown;

    public void OnLightningHit(int damage)
    {
        Damage(damage);
        isFalling = (hitpoints == 1);
    }

    public void OnPlayerAction()
    {
        if(!isFalling)
            PlayerActions.FinishProcess(this);
    }

    private void OnCutDown(bool isRight)
    {
        int index = isRight ? 1 : 0;
        for(int i = 0 ; i < nodesPlaceable[index].Count; i++)
        {
            Node node = nodesPlaceable[index][i]; 
            if(LogNotPlaceable(node))
                continue;
            GameObject.Instantiate(
                hasNut && isFruitplaceable(node) 
                    ? logs[2] 
                    : node.currentType == NodeType.Water 
                        ? logs[1] 
                        : logs[0],
                node.worldPosition,
                Quaternion.identity
            );
        }
        isFalling = false;
        PlayerActions.FinishProcess(this);
    }
}
