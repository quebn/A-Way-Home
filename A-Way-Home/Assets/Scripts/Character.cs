using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Public
    public Transform Home;
    public float Speed = 20.0f;
 
    // Private
    int _TargetIndex = 0;
    Vector3[] _Path;
    Pathfinding _Pathfinding;


    // Static
    // static Character s_Character;

    // GetSet
    public Vector3[] Path{
        get {return _Path;}
        set {_Path = value;}
    }
    void Awake()
    {
        _Pathfinding = GameObject.Find("Algorithm").GetComponent<Pathfinding>();
        print("StartPos: " + StartPos);
        print("TargetPos: " + TargetPos);
        print(_Pathfinding);
        Path = _Pathfinding.FindPath(StartPos, TargetPos);
    }

    void Start()
    {
        GoHome();
        print("FinalPos: " + transform.position);
    }
    public Vector3 StartPos{
        get {return transform.position;}
    }
    public Vector3 TargetPos{
        get {return Home.position;}
    }

    public void GoHome()
    {
        Vector3 CurrentTargetPos = _Path[0];

        while (true)
        {
            if (transform.position == CurrentTargetPos)
            {
                _TargetIndex++;
                if (_TargetIndex >= _Path.Length)
                    break;
                CurrentTargetPos = _Path[_TargetIndex];
            }
            // transform.position = Vector3.MoveTowards(transform.position, CurrentTargetPos, Speed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, CurrentTargetPos, 2.0f );
        }

    }

    public void OnDrawGizmos()
    {
        if (_Path == null)
            return;

        for (int i = _TargetIndex; i < _Path.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_Path[i], new Vector3(1.0f, 1.0f, 0));

            if (i == _TargetIndex)
                Gizmos.DrawLine(transform.position, _Path[i]);
            else 
                Gizmos.DrawLine(_Path[i-1],_Path[i]);
        }
    }
}
