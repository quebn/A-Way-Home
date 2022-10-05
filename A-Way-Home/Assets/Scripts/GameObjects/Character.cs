using UnityEngine;
// using UnityEngine.UI;
public class Character : MonoBehaviour
{
    [HideInInspector] public bool isHome = false;
    [HideInInspector] public float speed = 0f;
    [HideInInspector] public Vector3[] path;
    // public Image image;
    [HideInInspector] public string charName;
    [HideInInspector] public Transform home;
    [HideInInspector] public uint energy;
    // Private
    private Pathfinding _Pathfinding;
    private Vector3 _CurrentTargetPos;   
    private bool _IsPressed = false;
    private int _TargetIndex;

    public bool isPressed       { get{return _IsPressed;} }
    public Vector3 currentPos   { get{return transform.position;} }

    private void Awake()
    {
        _Pathfinding = GameObject.Find("Algorithm").GetComponent<Pathfinding>();
    }

    private void Start()
    {
        // Debug.Log("Character StartingPos: " + currentPos);
        // Debug.Log("Character home Position: " + home.position);
        Debug.Assert(home != null, "Error: home of Chracter is not found!");
    }
    private void Update()
    {
        if (isPressed)
        {
            GoHome();
            InGameUI.Instance.SetCharacterEnergy(energy);
        }
    }
    public void InitCharacter()
    {
        path = _Pathfinding.FindPath(currentPos, home.position);
        Debug.Log(path.Length);
        if (path.Length <=0)
            return;
        _CurrentTargetPos =path[0];
        _TargetIndex = 0;
        _IsPressed = true;        
    }
    
    public void GoHome()
    {
        if (currentPos == _CurrentTargetPos)
        {
            energy--;
            _TargetIndex++;
            if (_TargetIndex >= path.Length)
            {
                StopCharacter();
                // Set EndGame window active with level clear as context
                GameEvent.SetEndWindowActive(EndGameType.LevelClear);
                return;
            }
            if (energy == 0)
            {
                if (PlayerLevelData.Instance.levelData.lives == 1)
                {
                    StopCharacter();
                    GameEvent.SetEndWindowActive(EndGameType.GameOver);
                    return;
                }
                StopCharacter();
                GameEvent.SetEndWindowActive(EndGameType.NoEnergy);
                return;   
            }
            _CurrentTargetPos = path[_TargetIndex];
        }
        transform.position = Vector3.MoveTowards(currentPos, _CurrentTargetPos, speed * Time.deltaTime);
    }

    private void StopCharacter()
    {
        // Debug.Log("Ending Pos: " + currentPos);
        isHome = true;
        _IsPressed = false;
    }

}
