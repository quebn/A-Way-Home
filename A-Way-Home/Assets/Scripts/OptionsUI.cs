using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    public InGameUI InGameUI;


    public void Resume()
    {
        Debug.Assert(InGameUI.s_IsPaused == true);
        InGameUI.s_IsPaused = false;
        InGameUI.OptionsUI.SetActive(false);
        Time.timeScale = 1f;

    }
}
