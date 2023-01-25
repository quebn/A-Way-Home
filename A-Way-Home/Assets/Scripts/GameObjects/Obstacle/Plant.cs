using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Obstacle //, IInteractable
{
    // [System.Serializable] 
    // public enum Stage { Juvenile, Adult, Harvested}
    
    // [SerializeField] private Stage stage;
    // private Animator animator;

    // private bool juvenile => stage == Stage.Juvenile;
    // private bool adult => stage == Stage.Adult;
    // private bool harvested => stage == Stage.Harvested;

    // protected override void Initialize()
    // {
    //     base.Initialize();
    //     spriteRenderer = GetComponent<SpriteRenderer>();
    //     animator = GetComponent<Animator>();
    //     InitializeNodes(worldPos);
    //     if(adult)
    //         GrowUp();
    // }

    // public void OnClick()
    // {
    //     if(incorrectTool)
    //         return;
    //     switch(currentTool)
    //     {
    //         case Tool.Lightning:
    //             LightningStrike();
    //             break;
    //         case Tool.Grow:
    //             if(juvenile)
    //                 GrowUp();
    //             break;
    //     }
    // }

    // public void OnDehover()
    // {
    //     if(incorrectTool)
    //         return;
    //     spriteRenderer.color = Color.white;
    // }

    // public void OnHover()
    // {
    //     if(incorrectTool)
    //         return;
    //     spriteRenderer.color = Color.green;
    // }

    // private void LightningStrike()
    // {
    //     PlayerActions.Instance.LightningAnimation(worldPos);
    //     if(adult)
    //         SpawnFruit();
    //     else
    //         Invoke("Disintegrate", DisintegrateAnimation());
    // }

    // private void SpawnFruit()
    // {
    //     // SetNodesType(NodeType.Walkable);
    //     animator.Play("Plant_Harvested");
    // }

    // private void Disintegrate()
    // {
    //     gameObject.SetActive(false);
    // }

    // private float DisintegrateAnimation()
    // {
    //     animator.Play("Plant_Destroy");
    //     return animator.GetCurrentAnimatorStateInfo(0).length;
    // }


    // private void GrowUp()
    // {
    //     if (juvenile)
    //         stage = Stage.Adult;
    //     animator.Play("Plant_Grow");
    //     SetNodesType(NodeType.Obstacle);
    // }
}
