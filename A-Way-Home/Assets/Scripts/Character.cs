using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Public
    public Transform Home;
    public float Speed = 20.0f;
 
    // Private
    int _TargetIndex;
    Vector3[] _Path;
    Pathfinding _Pathfinding;
    bool _IsPressed = false;
    Vector3 _CurrentTargetPos;

    // GetSet
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

    void Awake()
    {
        _Pathfinding = GameObject.Find("Algorithm").GetComponent<Pathfinding>();
        Debug.Log("StartPos: " + StartPos);
        Debug.Log("TargetPos: " + TargetPos);
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _Path = _Pathfinding.FindPath(StartPos, TargetPos);
            _CurrentTargetPos =_Path[0];
            _TargetIndex = 0;
            _IsPressed = true;
        }
    }
    void FixedUpdate()
    {
        if (_IsPressed)
        {
            GoHome();
        }
    }

    void GoHome()
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
