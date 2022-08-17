using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static bool s_IsPaused = false;

    public Slider EnergySlider;
    public TextMeshProUGUI MovesLeft;
    public Manipulation Manipulation;
    public Character Character;
    public GameObject OptionsUI;

    void Start()
    {
        Debug.Log("Level Loaded!");
        MovesLeft.text = Manipulation.Moves.ToString();
        InitCharacterEnergy(Character.Energy);
    }

    void Update()
    {
        if (s_IsPaused)
            return;
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ReloadAction();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Character.InitCharacter();
        if (Mouse.current.leftButton.wasPressedThisFrame && Manipulation.CurrentType != ToolType.None)
            MovesLeft.text = Manipulation.Moves.ToString();
        if (Character.IsPressed)
        {
            Character.GoHome();
            SetCharacterEnergy(Character.Energy);
        }
    }

    private void InitCharacterEnergy(uint energy)
    {
        EnergySlider.maxValue = energy;
        SetCharacterEnergy(energy);
    }

    private void SetCharacterEnergy(uint energy)
    {
        EnergySlider.value = energy;
    }


    // Action Bar
    public void ReloadAction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayAction()
    {
        Character.InitCharacter();
        if (Character.IsPressed)
        {
            Character.GoHome();
        }
    }
    // Options
    public void HelpButton()
    {

    }
    
    public void OptionsButton()
    {
        Debug.Assert(s_IsPaused == false);
        s_IsPaused = true;
        OptionsUI.SetActive(true);
        Time.timeScale = 0f;
    }


    // ToolBar
    public void SetToolNone() 
    {
        Manipulation.CurrentType = ToolType.None; 
        Debug.Log("Current Tool: None");
    }
    public void SetToolPickAxe() 
    {
        Manipulation.CurrentType = ToolType.Pickaxe;
        Debug.Log("Current Tool: Pickaxe");

    }
    public void SetToolWoodAxe() 
    {
        Manipulation.CurrentType = ToolType.WoodAxe;
        Debug.Log("Current Tool: Woodaxe");
    }

}
