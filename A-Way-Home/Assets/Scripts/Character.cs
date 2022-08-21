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
    public bool IsPressed{
        get { return _IsPressed; }
    }
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


    public void InitCharacter()
    {
        _Path = _Pathfinding.FindPath(CurrentPos, TargetPos);
        if (_Path.Length <=0)
            return;
        _CurrentTargetPos =_Path[0];
        _TargetIndex = 0;
        _IsPressed = true;        
    }
    
    public void GoHome()
    {
        if (CurrentPos == _CurrentTargetPos)
        {
            Energy--;
            _TargetIndex++;
            if (_TargetIndex >= _Path.Length || Energy == 0)
            {
                Debug.Log("Ending Pos: " + CurrentPos);
                Debug.Log("GAME OVER!");
                _IsPressed = false;
                return;
            }
            _CurrentTargetPos = _Path[_TargetIndex];
        }
        transform.position = Vector3.MoveTowards(CurrentPos, _CurrentTargetPos, Speed * Time.deltaTime);
    }

}
