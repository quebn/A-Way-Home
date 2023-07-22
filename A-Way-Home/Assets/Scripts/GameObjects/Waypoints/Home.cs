using UnityEngine;

public class Home : MonoBehaviour
{
    public static Home instance {get; private set;}

    [SerializeField] private GameObject lightObject;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isPortal = false;
    [SerializeField] private GameObject portal;

    public bool portalActivated => isPortal;

    void Awake()
    {
        if(instance != this)
            instance = this;
        this.transform.position = NodeGrid.GetMiddle(this.transform.position);
    }

    public void ActivateHome()
    {
        if(isPortal){
            ActivatePortal();
            return;
        }
        lightObject.SetActive(true);
        spriteRenderer.color = Color.green  ;
    }

    private void ActivatePortal()
    {
        spriteRenderer.enabled = false;
        portal.SetActive(true);
    }
}