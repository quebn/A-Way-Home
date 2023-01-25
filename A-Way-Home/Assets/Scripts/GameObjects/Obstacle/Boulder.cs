using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Boulder : Obstacle//, IInteractable, INodeInteractable
{

    // private Animator animator;
    // private int hitpoints;

    // protected override void Initialize()
    // {
    //     base.Initialize();
    //     animator = GetComponent<Animator>();
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     InitializeNodes(this.transform.position);
    //     SetNodesType(NodeType.Obstacle, this);
    //     hitpoints = 4;
    // }

    // public void OnClick()
    // {
    //     if (incorrectTool)
    //         return;
    //     Debug.Log("Click!");
    // }

    // public void OnHover()
    // {
    //     if(incorrectTool)
    //         return;
    //     spriteRenderer.color = Color.green;
    // }

    // public void OnDehover()
    // {
    //     spriteRenderer.color = Color.white;
    // }

    // public void OnNodeInteract(Tool tool)
    // {
    //     if(tool != Tool.Tremor)
    //         return;
    //     if(hitpoints > 0)
    //         hitpoints--;
    //     RemoveBoulder();
    // }

    // private void RemoveBoulder()
    // {
    //     if(hitpoints > 0)
    //         return;
    //     PlayerLevelData.Instance.IncrementPlayerMoves(-1);
    //     animator.Play("BigBoulder_Destroy");
    //     float delay = animator.GetCurrentAnimatorStateInfo(0).length;
    //     Invoke("OnDestroy", delay);
    // }


    // private void OnDestroy()
    // {
    //     SetNodesType(NodeType.Walkable);
    //     this.gameObject.SetActive(false);
    // }
}