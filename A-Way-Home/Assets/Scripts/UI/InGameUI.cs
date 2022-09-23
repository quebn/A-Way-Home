using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance {get; private set;}
    [HideInInspector] public bool isPaused = false; //TODO: should be in GameEvent.cs
    [HideInInspector] public EndGameType endGameType;
    [SerializeField] private GameObject optionsWindow;
    [SerializeField] private GameObject gameEndWindow;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI movesLeftTMP;
    [SerializeField] private TextMeshProUGUI energyLeftTMP;
    [SerializeField] private TextMeshProUGUI livesLeftTMP;
    private void Start()
    {

        Debug.Assert(!isPaused, "Game is Paused");
        movesLeftTMP.text = PlayerLevelData.Instance.playerMoves.ToString();
        livesLeftTMP.text = PlayerLevelData.Instance.playerLives.ToString();
        InitCharacterEnergy(PlayerLevelData.Instance.character.energy);
        endGameType = EndGameType.None;
        if (Instance == null)
            Instance = this;
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

    public void SetCharacterEnergy(uint energy)
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
        // Reloading the Level should consume a Player Life.
        // GameEvent.instance.RestartGame();
        GameEvent.RestartGame();
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

    public void PauseGame()//TODO: should be in GameEvent.cs
    {
        Debug.Assert(!isPaused, "Game is Already Paused");
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void UnpauseGame()//TODO: should be in GameEvent.cs
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
    
    public static void SetEndWindowActive(EndGameType endGameType)
    {
        Debug.Assert(Instance != false, $"ERROR:{Instance.gameObject.name} instance is null");
        Debug.Assert(Instance.gameEndWindow != false, $"ERROR: Game End window is null/not found!");
        Instance.gameEndWindow.SetActive(true);
        Instance.endGameType = endGameType;
    }

}
