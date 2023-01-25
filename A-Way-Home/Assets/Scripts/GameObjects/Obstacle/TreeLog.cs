using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeLog : Obstacle//, IInteractable, INodeInteractable
{
    // public bool notSpawned;
    // private Animator animator;

    // protected override void Initialize()
    // {
    //     base.Initialize();
    //     animator = GetComponent<Animator>();
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     InitializeNodes(this.transform.position);
    //     SetNodesType(NodeType.Obstacle, this);
    //     if(!notSpawned)
    //         SpawnAnimation();
    // }

    // public override void SetActionData(ActionData actionData)
    // {
    //     base.SetActionData(actionData);
    //     actionData.isActive = this.gameObject.activeSelf;
    // }

    // public override void OnUndo(ActionData actionData)
    // {
    //     base.OnUndo(actionData);
    //     this.gameObject.SetActive(actionData.GetLogData());
    //     SetNodesType(NodeType.Obstacle);
    //     Debug.Log("Undo Log");
    // }

    // private void RemoveLog()
    // {
    //     PlayerActions.Instance.LightningAnimation(this.transform.position);
    //     this.tag = "Interacted";
    //     PlayerLevelData.Instance.IncrementPlayerMoves(-1);
    //     SetNodesType(NodeType.Walkable);
    //     this.gameObject.SetActive(false);
    // }

    // private void SpawnAnimation()
    // {
    //     animator.Play("Log_Spawn");
    // }

    // private void ShowInfo()
    // {
    //     Debug.Log("Clicked on Log");
    // }

    // public static void SpawnTreeLog(Vector2 position, GameObject prefab, string id)
    // {

    // }

    // public void OnClick()
    // {
    //     if(incorrectTool)
    //         return;
    //     ShowInfo();
    // }

    // public void OnHover()
    // {
    //     if(incorrectTool)
    //         return;
    //     spriteRenderer.color = Color.green;
    //     // HighlightObstacle();
    // }

    // public void OnDehover()
    // {
    //     spriteRenderer.color = Color.white;
    // }

    // public void OnNodeInteract(Tool tool)
    // {
    //     RemoveLog();
    // }
}
