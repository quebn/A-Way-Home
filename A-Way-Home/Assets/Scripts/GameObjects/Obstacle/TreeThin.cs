using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeThin : Obstacle//, IInteractable, IPlaceable
{
    // [SerializeField] private GameObject treeUpper;
    // [SerializeField] private GameObject treeLower;
    // [SerializeField] private Animator animatorUpper;
    // [SerializeField] private Animator animatorLower;
    // [SerializeField] private List<GameObject> logs;

    // private SpriteRenderer lowerSpriteRenderer;
    // private Vector2 currentPlaceable;
    // private Dictionary<Vector2, List<Node>> placeableNodes;
    // private bool isClicked;
    // private bool isCutDown = false;

    // protected override void Initialize()
    // {
    //     base.Initialize();
    //     spriteRenderer = treeUpper.GetComponent<SpriteRenderer>();
    //     lowerSpriteRenderer = treeLower.GetComponent<SpriteRenderer>();
    //     InitializeNodes(this.transform.position);
    //     SetNodesType(NodeType.Obstacle);
    //     isClicked = false;
    //     SetCurrentPlaceables();
    // }

    // public override void SetActionData(ActionData actionData)
    // {
    //     base.SetActionData(actionData);
    //     // set world pos where the 2 log were spawnd.
    //     // actionData.isActive = true;
    // }

    // private void Update()
    // {
    //     HoverCurrentPlaceable();
    // }

    // public void OnClick()
    // {
    //     if(incorrectTool)
    //         return;
    //     if(!isCutDown){
    //         PlaceMode();
    //         return;
    //     }
    //     DestroyTrunk();
    // }

    // public void OnHover()
    // {
    //     if(incorrectTool)
    //         return;
    //     spriteRenderer.color = new Color32(255, 255, 255, 100);
    //     lowerSpriteRenderer.color = Color.green;

    // }

    // public void OnDehover()
    // {
    //     spriteRenderer.color = Color.white;
    //     lowerSpriteRenderer.color = Color.white;
    // }

    // public void OnPlace()
    // {
    //     if(placeableNodes[currentPlaceable].Count < 2 )
    //         return;
    //     isCutDown = !isCutDown;
    //     PlayerActions.Instance.LightningAnimation(new Vector2(this.worldPos.x, this.worldPos.y + .5f));
    //     Invoke("PlaceLogs", CutDownTreeAnimation());
    // }

    // private void DestroyTrunk()
    // {
    //     Debug.LogWarning("Trunk Destroyed");
    // }

    // private void HoverCurrentPlaceable()
    // {
    //     if(!this.isClicked && currentTool != Tool.PlaceMode)
    //         return;
    //     HighlightCurrentPlaceable(GetPlaceableOrigin(PlayerActions.Instance.mouseWorldPos));
    // }

    // private void HighlightCurrentPlaceable(Vector2 pos)
    // {
    //     if(currentPlaceable == pos || !placeableNodes.ContainsKey(pos))
    //         return;
    //     if(currentPlaceable != Vector2.zero)
    //         Node.RevealNodes(placeableNodes[currentPlaceable], Node.colorGreen);
    //     currentPlaceable = pos;
    //     Node.RevealNodes(placeableNodes[currentPlaceable], Node.colorRed);
    // }

    // private void SetCurrentPlaceables()
    // {
    //     placeableNodes  = new Dictionary<Vector2, List<Node>>(2);
    //     for(float f = -1.5f; f < 3; f += 3){
    //         Vector2 pos = new Vector2(this.worldPos.x + f, this.worldPos.y);
    //         if(!NodeGrid.CheckTileIsWalkable(pos, 2, 1))
    //             continue;
    //         placeableNodes.Add(pos, NodeGrid.NodesByTileSize(pos, 2, 1));
    //     }
    // }

    // private Vector2 GetPlaceableOrigin(Vector2 mousePos)
    // {
    //     if(mousePos.x > this.worldPos.x)
    //         return new Vector2(worldPos.x + 1.5f, worldPos.y);
    //     else 
    //         return new Vector2(worldPos.x - 1.5f, worldPos.y);
    // }

    // private void PlaceMode()
    // {
    //     PlayerActions.Instance.currentTool = Tool.PlaceMode;
    //     isClicked = true;
    //     foreach(KeyValuePair<Vector2, List<Node>> pair in placeableNodes)
    //         Node.RevealNodes(pair.Value, Node.colorGreen);
    // }

    // private void PlaceLogs()
    // {
    //     this.treeUpper.SetActive(false);
    //     isClicked = false;
    //     Node.ToggleNodes(placeableNodes[currentPlaceable], NodeGrid.nodesVisibility);
    //     for(int i = 0; i < placeableNodes[currentPlaceable].Count; i++)
    //         Instantiate(logs[i], placeableNodes[currentPlaceable][i].worldPosition, Quaternion.identity);
    //     PlayerActions.Instance.currentTool = Tool.Inspect;
    // }

    // private float CutDownTreeAnimation()
    // {
    //     if (currentPlaceable.x > this.worldPos.x)
    //         animatorUpper.Play("TreeUpper_FallRight");
    //     else if (currentPlaceable.x < this.worldPos.x)
    //         animatorUpper.Play("TreeUpper_FallLeft");
    //     return animatorUpper.GetCurrentAnimatorStateInfo(0).length;
    // }

}
