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

    public GameObject getGameEndWindow { get { return gameEndWindow; } }

    private void Start()
    {
        Debug.Assert(!isPaused, "Game is Paused");
        movesLeftTMP.text = PlayerLevelData.Instance.levelData.moves.ToString();
        livesLeftTMP.text = PlayerLevelData.Instance.levelData.lives.ToString();
        InitCharacterEnergy(PlayerLevelData.Instance.character.energy);
        endGameType = EndGameType.None;
        if (Instance == null)
            Instance = this;
    }

    public void SetPlayerMoves()
    {
        this.movesLeftTMP.text = PlayerLevelData.Instance.levelData.moves.ToString();
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
        // +1 player moves
        // revert last player action
        // -1 character energy   
    }
    public void ReloadAction()
    {
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
        GameEvent.PauseGame();
        // Debug.Assert(OptionsUI.Instance != null, "OptionsUi instance is null");
        if (isPaused)
            this.optionsWindow.SetActive(true);
            // OptionsUI.Instance.gameObject.SetActive(true);
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
