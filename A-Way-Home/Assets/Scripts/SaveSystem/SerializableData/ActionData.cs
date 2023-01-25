using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionData 
{
    public Tool tool;
    // Depending on the Obstacle interacted with should contain:
    //  - Type of Obstacle
    //  - previous Data
    public string ID {private get; set;}
    public bool isActive {private get; set;}
    public int hitpoints {private get; set;}
    public SerializedVector3 previousPosition {private get; set;}
    // public Plant.Stage growthStage {private get; set;}

    public ActionData(Obstacle obstacle, Tool toolUsed)
    {
        this.tool = toolUsed;
        obstacle.SetActionData(this);
    }


    public Obstacle GetObstacle()
    {
        return Obstacle.list[this.ID];
    }

    public bool GetLogData()
    {
        return isActive;
    }

    public Tuple<bool, int> GetBoulderData()
    {
        return new Tuple<bool, int>(isActive, hitpoints);
    }   
}
