using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockEssence : Obstacle, ITremor
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject essence;

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        if(hitpoints <= 0)
            Remove();
    }

    public void OnTremor()
    {
        Damage(2);
    }

    public override void Remove()
    {
        ForceDehighlight();
        if(hitpoints != 0)
            hitpoints = 0;
        ClearNodes();
        animator.Play("Destroy");
        float delay = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("OnRemove", delay);
        essence.SetActive(true);
    }

    private void OnRemove()
    {
        audioSources[0].Play();
        this.spriteRenderers[0].enabled = false;
    }

    public override void LoadData(LevelData levelData)
    {
        Debug.Assert(id != "", $"ERROR: {this.GetType().Name} id is empty string");
        Debug.Assert(levelData.obstacles.ContainsKey(id), $"ERROR: {id} not found");
        if(!levelData.obstacles.ContainsKey(id))
                return;
        // Debug.Log("Loading obstacle data");
        this.hitpoints = levelData.obstacles[id].GetValue("hp");
        this.gameObject.transform.position = levelData.obstacles[id].position;
        Debug.Log($"Loaded Leveldata Obstacles :{levelData.obstacles[id].typeName} with hp: {levelData.obstacles[id].valuePairs["hp"]} -> {id}");
        // if(hitpoints == 0)
        //     gameObject.SetActive(false);
        Debug.Assert(this.hitpoints == levelData.obstacles[id].GetValue("hp"), "ERROR: values doesnt match");
    }
}
