using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    
    public static InGameUI Instance {get; private set;}
    // [HideInInspector] public bool isPaused = false; //TODO: should be in GameEvent.cs
    [HideInInspector] public EndGameType endGameType;
    [SerializeField] private GameObject optionsWindow;
    [SerializeField] private GameObject gameEndWindow;
    [SerializeField] public Image characterImage;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI characterNameTMP;
    [SerializeField] private TextMeshProUGUI movesLeftTMP;
    [SerializeField] private TextMeshProUGUI energyLeftTMP;
    [SerializeField] private TextMeshProUGUI livesLeftTMP;
    [SerializeField] private TextMeshProUGUI skillCounter;
    public GameObject getGameEndWindow { get { return gameEndWindow; } }
    private bool pathDisplayToggle = false;


    public uint SkillCounter{
        set { this.skillCounter.text = $"{value}";}
    }
    
    private void Start()
    {
        PlayerLevelData playerLevelData = PlayerLevelData.Instance;
        InitCharacterUI(playerLevelData.levelData, playerLevelData.character);
        endGameType = EndGameType.None;
        if (Instance == null)
            Instance = this;
    }

    private void InitCharacterUI(LevelData levelData, Character character)
    {
        this.characterImage.sprite  = character.characterImage.sprite;
        this.characterNameTMP.text  = levelData.characterName;
        this.movesLeftTMP.text      = levelData.moves.ToString();
        this.livesLeftTMP.text      = levelData.lives.ToString();
        this.skillCounter.text      = levelData.skillCount.ToString();
        InitCharacterEnergy(character.energy);
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
    public void ShowCurrentPath()
    {
        if (!pathDisplayToggle)
            pathDisplayToggle = true;
        else
            pathDisplayToggle = false;
        PlayerLevelData.Instance.character.DisplayPath(pathDisplayToggle);
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
        Debug.Assert(GameEvent.isPaused, "ERROR: Game is not paused!");
        this.optionsWindow.SetActive(true);
    }


    #endregion
    #region ToolBar
    public void SetTool(int toolindex)
    {
        if (toolindex > 3)
        {
            Debug.LogWarning("Tool not yet added in Manipulation enum");
            return;
        }
        ICharacter character = (ICharacter)PlayerLevelData.Instance.character;
        if (toolindex != (int)ManipulationType.UniqueSkill)
            character.OnDeselect();
        PlayerActions.Instance.currentManipulationType = (ManipulationType)toolindex;
        Debug.Log("Current Tool index: " + PlayerActions.Instance.currentManipulationType);
    }
    #endregion
    


}
