using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{
    public SpriteRenderer platform;
    public float panSpeed = 20f;
    public float panBorderWidth = 10f;
    public float zoomSpeed = 5f;
    public float minZoom = 4.5f;
    public float maxZoom = 8f;
    public bool keyboard  = true; 
    public bool mouse  = false; 
    // Privates
    private Camera _Camera;
    private float _Zoom;
    private float _MapMinX, _MapMaxX, _MapMinY, _MapMaxY;

    private void Awake()
    {   
        _Camera = Camera.main;
        _Zoom = _Camera.orthographicSize;

        _MapMinX = platform.transform.position.x - platform.bounds.size.x * 0.5f;
        _MapMaxX = platform.transform.position.x + platform.bounds.size.x * 0.5f;

        _MapMinY = platform.transform.position.y - platform.bounds.size.y * 0.5f;
        _MapMaxY = platform.transform.position.y + platform.bounds.size.y * 0.5f;
    }


    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }
    private void MoveCamera()
    {
        Vector3 CameraPos = transform.position;
        if (mouse)
        {
            Vector2 MousePos = Mouse.current.position.ReadValue();
            if (Keyboard.current.wKey.isPressed || MousePos.y >= Screen.height - panBorderWidth)
                CameraPos.y += panSpeed * Time.deltaTime;
            if (Keyboard.current.sKey.isPressed || MousePos.y <= panBorderWidth)
                CameraPos.y -= panSpeed * Time.deltaTime;
            if (Keyboard.current.dKey.isPressed || MousePos.x >= Screen.width - panBorderWidth)
                CameraPos.x += panSpeed * Time.deltaTime;
            if (Keyboard.current.aKey.isPressed || MousePos.x <= panBorderWidth)
                CameraPos.x -= panSpeed * Time.deltaTime;
        }
        if (keyboard)
        {
            if (Keyboard.current.wKey.isPressed)
                CameraPos.y += panSpeed * Time.deltaTime;
            if (Keyboard.current.sKey.isPressed)
                CameraPos.y -= panSpeed * Time.deltaTime;
            if (Keyboard.current.dKey.isPressed)
                CameraPos.x += panSpeed * Time.deltaTime;
            if (Keyboard.current.aKey.isPressed)
                CameraPos.x -= panSpeed * Time.deltaTime;
        }
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
        _Zoom -= Scroll * zoomSpeed * Time.deltaTime;
        _Zoom = Mathf.Clamp(_Zoom, minZoom, maxZoom);

        _Camera.orthographicSize = _Zoom;
        _Camera.transform.position = ClampCamera(_Camera.transform.position);
    }

    private Vector3 ClampCamera(Vector3 TargetPos)
    {
        float CamHeight = _Camera.orthographicSize;
        float CamWidth = _Camera.orthographicSize * _Camera.aspect;

        Vector2 Max = new Vector2(_MapMaxX - CamWidth, _MapMaxY - CamHeight);
        Vector2 Min = new Vector2(_MapMinX + CamWidth, _MapMinY + CamHeight);

        Vector2 ClampedPos = new Vector2(Mathf.Clamp(TargetPos.x, Min.x, Max.x), Mathf.Clamp(TargetPos.y, Min.y, Max.y));

        return new Vector3(ClampedPos.x, ClampedPos.y, TargetPos.z);
    }
}


    // Move Camera Code
