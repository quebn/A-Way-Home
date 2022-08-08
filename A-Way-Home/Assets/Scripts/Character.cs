using UnityEngine;
using UnityEngine.InputSystem;
public class Character : MonoBehaviour
{
    // Public
    public Transform Home;
    public float Speed = 20.0f;
    public uint Energy = 0;

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

    public Vector3 CurrentPos{
        get {return transform.position;}
    }

    public Vector3 TargetPos{
        get {return Home.position;}
    }

    private void Awake()
    {
        _Pathfinding = GameObject.Find("Algorithm").GetComponent<Pathfinding>();
        Debug.Log("Character StartingPos: " + CurrentPos);
        Debug.Log("Character TargetPos: " + TargetPos);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            _Path = _Pathfinding.FindPath(CurrentPos, TargetPos);
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
        if (CurrentPos == _CurrentTargetPos)
        {
            Energy--;
            _TargetIndex++;
            if (_TargetIndex >= _Path.Length)
            {
                _IsPressed = false;
                return;
            }
            _CurrentTargetPos = _Path[_TargetIndex];
        }
        if (Energy == 0)
        {
            _IsPressed = false;
            Debug.Log("Ending Pos: " + CurrentPos);
            Debug.Log("GAME OVER! \n Character ran out of energy!!");
            return;
        }
        transform.position = Vector3.MoveTowards(CurrentPos, _CurrentTargetPos, Speed * Time.deltaTime);
    }

}
