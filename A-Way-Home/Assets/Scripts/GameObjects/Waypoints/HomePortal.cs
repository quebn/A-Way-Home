using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePortal : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private int foodRequired;

    public void OpenPortal()//only open 
    {
        animator.Play("PortalGreen_Open");
    }

    public void ClosePortal()
    {
        animator.Play("PortalGreen_Close");
        float delay = animator.GetNextAnimatorStateInfo(0).length;
        Invoke("OnPortalClose", delay);
    }

    private void OnPortalClose()
    {
        GameEvent.SetEndWindowActive(EndGameType.LevelClear);
    }
}
