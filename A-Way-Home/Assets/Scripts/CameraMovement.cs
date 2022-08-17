using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{
    public SpriteRenderer Platform;
    public float PanSpeed = 20f;
    public float PanBorderWidth = 10f;
    public float ZoomSpeed = 5f;
    public float MinZoom = 4.5f;
    public float MaxZoom = 8f;

    // Privates
    private float _Zoom;
    private float _MapMinX, _MapMaxX, _MapMinY, _MapMaxY;

    private void Awake()
    {   
        _Zoom = Camera.main.orthographicSize;

        _MapMinX = Platform.transform.position.x - Platform.bounds.size.x * 0.5f;
        _MapMaxX = Platform.transform.position.x + Platform.bounds.size.x * 0.5f;

        _MapMinY = Platform.transform.position.y - Platform.bounds.size.y * 0.5f;
        _MapMaxY = Platform.transform.position.y + Platform.bounds.size.y * 0.5f;
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector3 CameraPos = transform.position;

        if (Keyboard.current.wKey.isPressed)
            CameraPos.y += PanSpeed * Time.deltaTime;

        if (Keyboard.current.sKey.isPressed)
            CameraPos.y -= PanSpeed * Time.deltaTime;

        if (Keyboard.current.dKey.isPressed)
            CameraPos.x += PanSpeed * Time.deltaTime;
            
        if (Keyboard.current.aKey.isPressed)
            CameraPos.x -= PanSpeed * Time.deltaTime;

        transform.position = ClampCamera(CameraPos);
    }
    
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
        _Zoom -= Scroll * ZoomSpeed * Time.deltaTime;
        _Zoom = Mathf.Clamp(_Zoom, MinZoom, MaxZoom);

        Camera.main.orthographicSize = _Zoom;
        Camera.main.transform.position = ClampCamera(Camera.main.transform.position);
    }

    private Vector3 ClampCamera(Vector3 TargetPos)
    {
        float CamHeight = Camera.main.orthographicSize;
        float CamWidth = Camera.main.orthographicSize * Camera.main.aspect;

        Vector2 Max = new Vector2(_MapMaxX - CamWidth, _MapMaxY - CamHeight);
        Vector2 Min = new Vector2(_MapMinX + CamWidth, _MapMinY + CamHeight);

        Vector2 ClampedPos = new Vector2(Mathf.Clamp(TargetPos.x, Min.x, Max.x), Mathf.Clamp(TargetPos.y, Min.y, Max.y));

        return new Vector3(ClampedPos.x, ClampedPos.y, TargetPos.z);
    }
}


    // Move Camera Code
    // Vector2 MousePos = Mouse.current.position.ReadValue();
    // if (Keyboard.current.wKey.isPressed || MousePos.y >= Screen.height - PanBorderWidth)
    //     CameraPos.y += PanSpeed * Time.deltaTime;

    // if (Keyboard.current.sKey.isPressed || MousePos.y <= PanBorderWidth)
    //     CameraPos.y -= PanSpeed * Time.deltaTime;

    // if (Keyboard.current.dKey.isPressed || MousePos.x >= Screen.width - PanBorderWidth)
    //     CameraPos.x += PanSpeed * Time.deltaTime;
        
    // if (Keyboard.current.aKey.isPressed || MousePos.x <= PanBorderWidth)
        // CameraPos.x -= PanSpeed * Time.deltaTime;