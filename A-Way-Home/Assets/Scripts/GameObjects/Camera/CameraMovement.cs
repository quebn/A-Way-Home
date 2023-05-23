using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance {get; private set;}
    [SerializeField] private float zoomSpeed = 25f;
    [SerializeField] private float minZoom = 4.5f;
    [SerializeField] private float maxZoom = 8f;
    [SerializeField] private float panSpeed = 10f;
    [SerializeField] private float panBorderThickness = 10f;

    // Privates
    private Vector3 origin;
    private Vector3 difference;
    private Vector3 resetCamera;
    private Camera mainCamera;
    private Mouse currentMouse;
    private float zoom;
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;
    private Vector2 cameraBoundary => PlayerLevelData.Instance.levelBoundary;// replace bounds size.

    private void Update()
    {
        ZoomCamera();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;
        mainCamera.transform.position = new Vector3(
            PlayerLevelData.Instance.cameraCenterPos.x,
            PlayerLevelData.Instance.cameraCenterPos.y,
            -10
        );
        zoom = mainCamera.orthographicSize;
        Vector2 center = Vector2.zero;
        mapMinX = center.x - cameraBoundary.x * 0.5f;
        mapMaxX = center.x + cameraBoundary.x * 0.5f;
        mapMinY = center.y - cameraBoundary.y * 0.5f;
        mapMaxY = center.y + cameraBoundary.y * 0.5f;

        resetCamera = mainCamera.transform.position;

        if (Instance == null)
            Instance = this;
    }
    
    private void LateUpdate()
    {
        PanCamera();

    }

    private void PanCamera()
    {
        if (GameEvent.isPaused || Character.instance.isHome || PlayerActions.IsMouseOverUI())
            return;
        Vector3 pos = transform.position;
        Vector3 mousePos = currentMouse.position.ReadValue(); 
        if(mousePos.y >= Screen.height - panBorderThickness)
            pos.y += panSpeed * Time.deltaTime;
        if(mousePos.y <= panBorderThickness)
            pos.y -= panSpeed * Time.deltaTime;
        if(mousePos.x >= Screen.width - panBorderThickness)
            pos.x += panSpeed * Time.deltaTime;
        if(mousePos.x <= panBorderThickness)
            pos.x -= panSpeed * Time.deltaTime;
        transform.position = ClampCamera(pos);
    }

    private void ZoomCamera()
    {
        if (GameEvent.isPaused || Character.instance.isHome)
            return;
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
