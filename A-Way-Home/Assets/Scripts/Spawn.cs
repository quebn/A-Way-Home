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
    


    private void Update()
    {
        SetObjectPrefab();
        SpawnObject();
    }

    private void SetObjectPrefab()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
            _CurrentObject = RockPrefab;
        else if (Keyboard.current.bKey.wasPressedThisFrame)
            _CurrentObject = WoodPrefab;
        // else if (Keyboard.current.cKey.wasPressedThisFrame)
            // return;
        
    }
    private void SpawnObject()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && _CurrentObject != null)
        {
            Vector3 MousePos = Mouse.current.position.ReadValue();
            Vector3 WorldPos = Camera.main.ScreenToWorldPoint(MousePos);
            Instantiate(_CurrentObject, WorldPos, Quaternion.identity);
            Debug.Log("Object("+ _CurrentObject +") spawned at: " + WorldPos);
        }
    }
    
}
