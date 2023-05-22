using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InspectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI kind;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI characterEffect;
    [SerializeField] private GameObject[] effects;

    public static InspectInfo inspectInfo;
    

    public void UpdateDataInfo()
    {
        kind.text = inspectInfo.name;
        type.text = inspectInfo.type;
        hp.text = inspectInfo.hp.ToString();
        characterEffect.text = inspectInfo.heal.ToString();
        effects[0].SetActive(inspectInfo.isDeadly);
        effects[1].SetActive(!inspectInfo.isDeadly);
        // if(inspectInfo.isDeadly)
        // {
        // }
        // else
        // {
        //     effects[0].SetActive(false);
        //     effects[1].SetActive(true);
        // }
    }

    public void Display(Vector3 position)
    {
        if(inspectInfo == null) 
            return;
        if(!this.gameObject.activeSelf)
            this.gameObject.SetActive(true);
        UpdateDataInfo();
        this.transform.position = position;
    }

    public void Hide()
    {
        inspectInfo = null;
        if(this.gameObject.activeSelf)
            this.gameObject.SetActive(false);
    }
}
