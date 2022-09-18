using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance {get; private set;}
    public bool isPaused = false;
    
    [SerializeField] private GameObject optionsWindow;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI movesLeftTMP;
    [SerializeField] private TextMeshProUGUI energyLeftTMP;
    [SerializeField] private TextMeshProUGUI livesLeftTMP;
    private void Start()
    {

        Debug.Assert(PlayerLevelData.Instance != null, "Error: No PlayerLevelData instance found!");
        Debug.Assert(PlayerLevelData.Instance.character != null, "Error: No character found!");

        Debug.Assert(!isPaused, "Game is Paused");
        movesLeftTMP.text = PlayerLevelData.Instance.playerMoves.ToString();
        livesLeftTMP.text = PlayerLevelData.Instance.playerLives.ToString();
        InitCharacterEnergy(PlayerLevelData.Instance.character.energy);
        if (Instance == null)
            Instance = this;
    }
    private void Update()
    {
        if (PlayerLevelData.Instance.character.isPressed)
        {
            PlayerLevelData.Instance.character.GoHome();
            SetCharacterEnergy(PlayerLevelData.Instance.character.energy);
        }
    }
    public void SetPlayerMoves()
    {
        this.movesLeftTMP.text = PlayerLevelData.Instance.playerMoves.ToString();
    }
    private void InitCharacterEnergy(uint energy)
    {
        energySlider.maxValue = energy;
        SetCharacterEnergy(energy);
    }

    private void SetCharacterEnergy(uint energy)
    {
        energyLeftTMP.text = energy.ToString();
        energySlider.value = energy;
    }


    #region Action Bar
    public void UndoAction()
    {
        Debug.Log("Pressed Undo Button!");        
    }
    public void ReloadAction()
    {
        SceneManager.LoadScene(PlayerLevelData.Instance.levelSceneName);
    }

    public void PlayAction()
    {
        PlayerLevelData.Instance.character.InitCharacter();
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
        // Debug.Assert(OptionsUI.Instance != null, "OptionsUi instance is null");
        if (isPaused)
            this.optionsWindow.SetActive(true);
            // OptionsUI.Instance.gameObject.SetActive(true);
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
        PlayerActions.Instance.currentManipulationType = (ManipulationType)toolindex;
        Debug.Log("Current Tool index: " + PlayerActions.Instance.currentManipulationType);
    }
    public void SetToolNone() 
    {
        PlayerActions.Instance.currentManipulationType = ManipulationType.None; 
        Debug.Log("Current Tool: None");
    }
    public void SetToolPickAxe() 
    {
        PlayerActions.Instance.currentManipulationType = ManipulationType.Pickaxe;
        Debug.Log("Current Tool: Pickaxe");

    }
    public void SetToolWoodAxe() 
    {
        PlayerActions.Instance.currentManipulationType = ManipulationType.WoodAxe;
        Debug.Log("Current Tool: Woodaxe");
    }

    #endregion
}
