using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance {get; private set;}
    private bool isPaused = false;
    
    public Slider EnergySlider;
    public TextMeshProUGUI MovesLeftTMP;
    public TextMeshProUGUI EnergyLeftTMP;
    public TextMeshProUGUI LivesLeftTMP;
    public GameObject OptionUI;
    
    
    private void Start()
    {

        Debug.Assert(PlayerLevelData.Instance != null, "Error: No PlayerLevelData instance found!");
        Debug.Assert(PlayerLevelData.Instance.Character != null, "Error: No character found!");

        PlayerActions.CurrentManipulationType = ManipulationType.None;

        Debug.Assert(!isPaused, "Game is Paused");
        MovesLeftTMP.text = PlayerLevelData.Instance.PlayerMoves.ToString();
        LivesLeftTMP.text = PlayerLevelData.Instance.PlayerLives.ToString();
        InitCharacterEnergy(PlayerLevelData.Instance.Character.Energy);
        if (Instance == null)
            Instance = this;
    }
    // TODO: Player input should be handled in player input script
    private void Update()
    {
        if (isPaused || PlayerLevelData.Instance.Character.IsHome)
            return;
        PlayerActions.SetCurrentTool();
        PlayerActions.ClearItem();
        if (Keyboard.current.rKey.wasPressedThisFrame)
            ReloadAction();
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            PlayerLevelData.Instance.Character.InitCharacter();
        if (Mouse.current.leftButton.wasPressedThisFrame && PlayerActions.CurrentManipulationType != ManipulationType.None)
            MovesLeftTMP.text = PlayerLevelData.Instance.PlayerMoves.ToString();
        if (PlayerLevelData.Instance.Character.IsPressed)
        {
            PlayerLevelData.Instance.Character.GoHome();
            SetCharacterEnergy(PlayerLevelData.Instance.Character.Energy);
        }
    }

    private void InitCharacterEnergy(uint energy)
    {
        EnergySlider.maxValue = energy;
        SetCharacterEnergy(energy);
    }

    private void SetCharacterEnergy(uint energy)
    {
        EnergyLeftTMP.text = energy.ToString();
        EnergySlider.value = energy;
    }


    #region Action Bar
    public void UndoAction()
    {
        Debug.Log("Pressed Undo Button!");        
    }
    public void ReloadAction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayAction()
    {
        PlayerLevelData.Instance.Character.InitCharacter();
        if (PlayerLevelData.Instance.Character.IsPressed)
        {
            PlayerLevelData.Instance.Character.GoHome();
        }
    }

    #endregion
    #region Options
    public void HelpButton()
    {
        Debug.Log("Pressed Help Button!");
        // Should Pause the game
    }
    
    public void OptionsButton()
    {
        PauseGame();
        if (isPaused)
            OptionUI.SetActive(true);
    }

    public void PauseGame()
    {
        Debug.Assert(!isPaused, "Game is Already Paused");
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        Debug.Assert(isPaused, "Game is not Paused");
        isPaused = false;
        Time.timeScale = 1f;
    }
    #endregion
    #region ToolBar
    public void SetTool(int toolindex)
    {
        PlayerActions.CurrentManipulationType = (ManipulationType)toolindex;
        Debug.Log("Current Tool index: " + PlayerActions.CurrentManipulationType);
    }
    public void SetToolNone() 
    {
        PlayerActions.CurrentManipulationType = ManipulationType.None; 
        Debug.Log("Current Tool: None");
    }
    public void SetToolPickAxe() 
    {
        PlayerActions.CurrentManipulationType = ManipulationType.Pickaxe;
        Debug.Log("Current Tool: Pickaxe");

    }
    public void SetToolWoodAxe() 
    {
        PlayerActions.CurrentManipulationType = ManipulationType.WoodAxe;
        Debug.Log("Current Tool: Woodaxe");
    }

    #endregion
}
