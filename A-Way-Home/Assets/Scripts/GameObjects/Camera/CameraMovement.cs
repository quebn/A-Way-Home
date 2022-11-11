using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance {get; private set;}
    public SpriteRenderer platform;
    public float zoomSpeed = 25f;
    public float minZoom = 4.5f;
    public float maxZoom = 8f;


    // Privates
    private Vector3 origin;
    private Vector3 difference;
    private Vector3 resetCamera;
    private Camera mainCamera;
    private float zoom;
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;
    private bool drag = false;

    private void Awake()
    {   
        mainCamera = Camera.main;
        zoom = mainCamera.orthographicSize;

        mapMinX = platform.transform.position.x - platform.bounds.size.x * 0.5f;
        mapMaxX = platform.transform.position.x + platform.bounds.size.x * 0.5f;

        mapMinY = platform.transform.position.y - platform.bounds.size.y * 0.5f;
        mapMaxY = platform.transform.position.y + platform.bounds.size.y * 0.5f;
    }

    private void Update()
    {
        ZoomCamera();
    }

    private void Start()
    {
        resetCamera = mainCamera.transform.position;
        if (Instance == null)
            Instance = this;
    }
    
    private void LateUpdate()
    {
        NewMoveCamera();
    }

    // TODO: rewrite this block of code someday
    public void NewMoveCamera()
    {
        // if (PlayerLevelData.Instance.character.isHome)
        //     return;
        Vector3 cameraPos = transform.position;
        if (Mouse.current.rightButton.isPressed)
        {
            difference = (mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue())) - mainCamera.transform.position;
            if (!drag)
            {
                drag = true;
                origin = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
        }
        else    
            drag = false;

        if (drag)
            cameraPos = origin - difference;
            transform.position = ClampCamera(cameraPos);
            
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            mainCamera.transform.position = resetCamera;    
        }
    }
    // End of Block

    private void ZoomCamera()
    {
        float Scroll = 0;

        Vector2 ScrollAxis = Mouse.current.scroll.ReadValue();
        Scroll = Mathf.Clamp(ScrollAxis.x + ScrollAxis.y, -2f, 2f);
        
        if (Keyboard.current.qKey.isPressed)
            Scroll = .1f;
        else if (Keyboard.current.eKey.isPressed)
            Scroll = -.1f;
        // ------------- to be remove if irrelevant
        if (Scroll == 0)
            return;
        // -------------
        zoom -= Scroll * zoomSpeed * Time.deltaTime;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

        mainCamera.orthographicSize = zoom;
        mainCamera.transform.position = ClampCamera(mainCamera.transform.position);
    }

    private Vector3 ClampCamera(Vector3 TargetPos)
    {
        float CamHeight = mainCamera.orthographicSize;
        float CamWidth = mainCamera.orthographicSize * mainCamera.aspect;

        Vector2 Max = new Vector2(mapMaxX - CamWidth, mapMaxY - CamHeight);
        Vector2 Min = new Vector2(mapMinX + CamWidth, mapMinY + CamHeight);

        Vector2 ClampedPos = new Vector2(Mathf.Clamp(TargetPos.x, Min.x, Max.x), Mathf.Clamp(TargetPos.y, Min.y, Max.y));

        return new Vector3(ClampedPos.x, ClampedPos.y, TargetPos.z);
    }
}
