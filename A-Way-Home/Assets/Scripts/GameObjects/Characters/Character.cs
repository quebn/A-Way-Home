using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;
public class Character : MonoBehaviour
{
    //[HideInInspector] public Image image;
    [HideInInspector] public Transform home;
    [HideInInspector] public string charName;
    [HideInInspector] public uint energy;
    [HideInInspector] public float speed;
    [HideInInspector] public bool isGoingHome = false;
    [HideInInspector] public Vector3[] path;
    private Vector3 currentTargetPos;   
    private int targetIndex;

    public Vector3 currentPos{ get { return transform.position; } }
    public bool isHome{ get { return this.transform.position == home.transform.position;}}

    private void Update()
    {
        if (isGoingHome)
        {
            GoHome();
            InGameUI.Instance.SetCharacterEnergy(energy);
        }
    }
    public void InitCharacter()
    {
        path = Pathfinding.FindPath(currentPos, home.position);
        Debug.Log(path.Length);
        if (path.Length <=0)
            return;
        currentTargetPos = path[0];
        targetIndex = 0;
        isGoingHome = true;
    }
    public void DisplayPath()
    {
        GameObject tile = NodeGrid.Instance.revealedTile;
        Vector3[] nodePath = Pathfinding.FindPath(currentPos, home.position);
        foreach (Vector3 position in nodePath)
        {
            Instantiate(tile, position, Quaternion.identity);
        }
    }
    public virtual void GoHome()
    {
        if (currentPos ==  currentTargetPos)
        {
            energy--;
            targetIndex++;
            if (targetIndex >= path.Length)
            {
                isGoingHome = false;
                GameEvent.SetEndWindowActive(EndGameType.LevelClear);
                return;
            }
            if (energy == 0)
            {
                if (PlayerLevelData.Instance.levelData.lives == 1)
                {
                    isGoingHome = false;
                    GameEvent.SetEndWindowActive(EndGameType.GameOver);
                    return;
                }
                isGoingHome = false;
                GameEvent.SetEndWindowActive(EndGameType.NoEnergy);
                return;   
            }
            currentTargetPos = path[targetIndex];
        }
        transform.position = Vector3.MoveTowards(currentPos, currentTargetPos, speed * Time. deltaTime);
    }
}
