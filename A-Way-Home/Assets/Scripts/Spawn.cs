using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// public - Show up in inspector and accessible by other scripts
// [SerialiseField] private - Show up in inspector, not accessible by other scripts
// [HideInInspector] public - Doesn't show in inspector, accessible by other scripts
// private - Doesn't show in inspector, not accessible by other scripts

public class Spawn : MonoBehaviour
{
    public GameObject RockPrefab;
    public GameObject WoodPrefab;
    
    private GameObject _CurrentObject;
    private HashSet<GameObject> _SpawnedObjects = new HashSet<GameObject>();
    private bool _ClearMode = false;

    private void Update()
    {
        SetObjectPrefab();
        switch(_ClearMode)
        {
            case true:
                ClearObject();
                break;
            case false:
                SpawnObject();
                break;
        }
    }
    int HashCode;
    private void SetObjectPrefab()
    {

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            Debug.Log("Current Object: Rock");
            _CurrentObject = RockPrefab;
            _ClearMode = false;
        }
        else if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Debug.Log("Current Object: Wood");
            _CurrentObject = WoodPrefab;
            _ClearMode = false;
        }
        else if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            Debug.Log("Clear Mode On");
            _CurrentObject = null;
            _ClearMode = true;
        }    

    }

    private void SpawnObject()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _CurrentObject != null)
        {
            Vector2 MousePos = Mouse.current.position.ReadValue();
            Vector2 WorldPos = (Vector2)Camera.main.ScreenToWorldPoint(MousePos);
            GameObject Object = Instantiate(_CurrentObject, WorldPos, Quaternion.identity);
            _SpawnedObjects.Add(Object);
            Debug.Log("("+ Object +") spawned at: " + WorldPos);
        }
    }
    

    private void ClearObject()
    {
        if (_ClearMode && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && _SpawnedObjects.Contains(hit.collider.gameObject))
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Destroyed " + hit.collider.gameObject);
            }
        }
    }
}
