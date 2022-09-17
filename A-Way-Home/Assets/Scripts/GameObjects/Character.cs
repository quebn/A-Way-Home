using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector] public bool IsHome = false;
    [HideInInspector] public float Speed = 0f;
    [HideInInspector] public Vector3[] Path;
    public Transform Home;
    public uint Energy;

    // Private
    private Pathfinding _Pathfinding;
    private Vector3 _CurrentTargetPos;   
    private bool _IsPressed = false;
    private int _TargetIndex;

    public bool IsPressed       { get{return _IsPressed;} }
    public Vector3 CurrentPos   { get{return transform.position;} }

    private void Awake()
    {
        _Pathfinding = GameObject.Find("Algorithm").GetComponent<Pathfinding>();
    }

    private void Start()
    {
        Debug.Log("Character StartingPos: " + CurrentPos);
        Debug.Log("Character Home Position: " + Home.position);
        Debug.Assert(Home != null, "Error: Home of Chracter is not found!");
    }

    public void InitCharacter()
    {
        Path = _Pathfinding.FindPath(CurrentPos, Home.position);
        Debug.Log(Path.Length);
        if (Path.Length <=0)
            return;
        _CurrentTargetPos =Path[0];
        _TargetIndex = 0;
        _IsPressed = true;        
    }
    
    public void GoHome()
    {
        if (CurrentPos == _CurrentTargetPos)
        {
            Energy--;
            _TargetIndex++;
            if (_TargetIndex >= Path.Length || Energy == 0)
            {
                Debug.Log("Ending Pos: " + CurrentPos);
                Debug.Log("GAME OVER!");
                IsHome = true;
                _IsPressed = false;
                return;
            }
            _CurrentTargetPos = Path[_TargetIndex];
        }
        transform.position = Vector3.MoveTowards(CurrentPos, _CurrentTargetPos, Speed * Time.deltaTime);
    }

}
