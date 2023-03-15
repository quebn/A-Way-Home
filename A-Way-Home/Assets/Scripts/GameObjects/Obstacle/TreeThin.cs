using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeThin : Obstacle, IHoverable, ILightning
{
    [SerializeField] private SpriteRenderer upperRenderer;
    [SerializeField] private GameObject upper;
    [SerializeField] private Animator animatorUpper;
    // [SerializeField] private Animator animatorLower;
    [SerializeField] private List<GameObject> logs;
    private Dictionary<Vector2, List<Node>> placeableNodes;
    private Vector2 currentPlaceable;
    private bool isHovered;
    private int hp = 2;

    private bool isFullyDestroyed => hitpoints == 0;
    private bool isCutDown => hitpoints == 1;
    protected override int hitpoints{
        get => hp;
        set => hp = value;
    } 

    private void Update()
    {
        if(isHovered && !isCutDown)
            HighlightPlaceLocations();
    }

    protected override void Initialize()
    {
        base.Initialize();
        SetNodes(this.worldPos, NodeType.Obstacle, this);
        SetPlaceableLocations();
    }

    public void DestroyCompletely()
    {
        ClearNodes();
        this.hitpoints = 0;
        this.gameObject.SetActive(false);
    }

    public void OnLightningHit()
    {
        if(isFullyDestroyed)
            return;
        hitpoints -= 1;
        if(isCutDown)
            StartCoroutine(CutDown());
        else
            DestroyTrunk();
    }

    
    protected override void OnDehighlight(Tool tool)
    {
        if(tool != Tool.Lightning)
            return;
        spriteRenderer.color = Color.white;
        upperRenderer.color = Color.white;;
        isHovered = false;
        if(currentPlaceable != Vector2.zero)
            Node.ToggleNodes(placeableNodes[currentPlaceable], NodeGrid.nodesVisibility);
    }

    protected override void OnHighlight(Tool tool)
    {
        if(tool != Tool.Lightning)
            return;
        spriteRenderer.color = Color.green;
        upperRenderer.color = new Color32(255, 255, 255, 100);;
        isHovered = true;
        // TODO: Move highlight placeable function here
    }


    public override void LoadData(LevelData levelData)
    {
        base.LoadData(levelData);
        // Debug.LogError(hitpoints);
        if(isFullyDestroyed)
        {
            DestroyTrunk();
            return;
        }
        else if(isCutDown)
            upper.SetActive(false);
        // SetPlaceableLocations();
        // StartCoroutine(SetNodesOnLoad(this.worldPos, NodeType.Obstacle, this));
    }

    private IEnumerator CutDown()
    {
        yield return new WaitForSeconds(CutDownTreeAnimation()); 
        this.upper.SetActive(false);
        Node.ToggleNodes(placeableNodes[currentPlaceable], NodeGrid.nodesVisibility);
        for(int i = 0; i < placeableNodes[currentPlaceable].Count; i++){
            Debug.LogWarning(placeableNodes[currentPlaceable][i].worldPosition);
            TreeLog log = Instantiate(logs[i], placeableNodes[currentPlaceable][i].worldPosition, Quaternion.identity).GetComponent<TreeLog>();
            log.AddAsSpawned($"{GameData.levelData.spawnCount += 1}");
        }
    }

    private void HighlightPlaceLocations()
    {
        Vector2 pos = GetMouseDirection();
        if(currentPlaceable == pos || !placeableNodes.ContainsKey(pos))
            return;
        if(currentPlaceable != Vector2.zero)
            Node.ToggleNodes(placeableNodes[currentPlaceable], NodeGrid.nodesVisibility);
        currentPlaceable = pos;
        Node.RevealNodes(placeableNodes[currentPlaceable], Node.colorRed);
    }

    private void DestroyTrunk()
    {
        ClearNodes();
        this.gameObject.SetActive(false);
    }

    private void SetPlaceableLocations()
    {
        placeableNodes  = new Dictionary<Vector2, List<Node>>(2);
        for(float f = -1.5f; f < 3; f += 3){
            Vector2 pos = new Vector2(this.worldPos.x + f, this.worldPos.y);
            if(!NodeGrid.CheckTileIsTerrain(pos, 2, 1))
                continue;
            placeableNodes.Add(pos, NodeGrid.GetNodes(pos, 2, 1));
        }
        Debug.Log($"tree on {transform.position} placeables count-> {placeableNodes.Count}");
    }

    private Vector2 GetMouseDirection()
    {
        Vector2 mousePos = PlayerActions.Instance.mouseWorldPos;
        if(mousePos.x > this.worldPos.x)
            return new Vector2(worldPos.x + 1.5f, worldPos.y);
        else 
            return new Vector2(worldPos.x - 1.5f, worldPos.y);
    }

    private float CutDownTreeAnimation()
    {
        if (currentPlaceable.x > this.worldPos.x)
            animatorUpper.Play("TreeUpper_FallRight");
        else if (currentPlaceable.x < this.worldPos.x)
            animatorUpper.Play("TreeUpper_FallLeft");
        return animatorUpper.GetCurrentAnimatorStateInfo(0).length;
    }

    public void OnHover()
    {
        upperRenderer.color = new Color32(255, 255, 255, 50);

    }

    public void OnDehover()
    {
        upperRenderer.color = Color.white;
    }


}
