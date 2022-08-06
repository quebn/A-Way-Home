using UnityEngine;
using UnityEngine.InputSystem;
public class Character : MonoBehaviour
{
    // Public
    public Transform Home;
    public float Speed = 20.0f;
 
    // Private
    private int _TargetIndex;
    private Vector3[] _Path;
    private Pathfinding _Pathfinding;
    private bool _IsPressed = false;
    private Vector3 _CurrentTargetPos;

    // GetSets
    public Vector3[] Path{
        get {return _Path;}
        set {_Path = value;}
    }

    public Vector3 StartPos{
        get {return transform.position;}
    }

    public Vector3 TargetPos{
        get {return Home.position;}
    }

    private void Awake()
    {
        _Pathfinding = GameObject.Find("Algorithm").GetComponent<Pathfinding>();
        Debug.Log("StartPos: " + StartPos);
        Debug.Log("TargetPos: " + TargetPos);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _Path = _Pathfinding.FindPath(StartPos, TargetPos);
            if (_Path.Length <=0)
                return;
            _CurrentTargetPos =_Path[0];
            _TargetIndex = 0;
            _IsPressed = true;
        }
    }
    private void FixedUpdate()
    {
        if (_IsPressed)
        {
            GoHome();
        }
    }

    private void GoHome()
    {
        if (_Path.Length <= 0)
            return;
        if (StartPos == _CurrentTargetPos)
        {
            _TargetIndex++;
            if (_TargetIndex >= _Path.Length)
            {
                _IsPressed = false;
                return;
            }
            _CurrentTargetPos = _Path[_TargetIndex];
        }
        transform.position = Vector3.MoveTowards(transform.position, _CurrentTargetPos, Speed * Time.deltaTime);
    }

}
