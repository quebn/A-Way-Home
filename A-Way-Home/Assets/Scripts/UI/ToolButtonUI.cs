using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolButtonUI : MonoBehaviour
{
    [SerializeField] private int index; 
    [SerializeField] private GameObject image; 
    [SerializeField] private GameObject locked; 

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Debug.Assert(this.index >= 0, "ERROR: Number is negative");
        int unlockedIndex = PlayerLevelData.Instance.unlockedTools;
        if(unlockedIndex < this.index){
            image.SetActive(false);
            locked.SetActive(true);
        }else{
            image.SetActive(true);
            locked.SetActive(false);
        }
    }
}
