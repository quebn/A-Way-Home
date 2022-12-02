using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance {get; private set;}

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
    [SerializeField] private TextMeshProUGUI timeCounter;

    private bool pathDisplayToggle = false;
    public GameObject getGameEndWindow { get { return gameEndWindow; } }
    private int energyValueUI {
        set { 
            energyLeftTMP.text = value.ToString();
            energySlider.value = value;
        }
    }
    private float timeCounterUI{
        get {return PlayerLevelData.Instance.levelData.secondsLeft; }
        set {
            PlayerLevelData.Instance.levelData.secondsLeft = value;
            timeCounter.text = ((int)timeCounterUI).ToString();
        }
    }

    private void Awake()
    {
        GameEvent.InitializeLevel();
    }

    private void Start()
    {
        PlayerLevelData playerLevelData = PlayerLevelData.Instance;
        InitCharacterUI(playerLevelData.levelData, playerLevelData.character);
        endGameType = EndGameType.None;
        if (Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (!PlayerLevelData.Instance.character.isGoingHome)
            TimeCountdown();
    }



    public void TimeCountdown()
    {
        if (timeCounterUI > 0)
            timeCounterUI -= Time.deltaTime;
        if (timeCounterUI <= 0 && !GameEvent.isEndWindowActive)
            GameEvent.SetEndWindowActive(EndGameType.TimeRanOut);
    }

    private void InitCharacterUI(LevelData levelData, Character character)
    {
        this.characterImage.sprite  = character.characterImage.sprite;
        this.characterNameTMP.text  = levelData.characterName;
        this.movesLeftTMP.text      = levelData.moves.ToString();
        this.livesLeftTMP.text      = levelData.lives.ToString();
        this.skillCounter.text      = levelData.skillCount.ToString();
        SetMaxEnergy(0);
    }

    public void SetMaxEnergy(int increment)
    {
        Character character = PlayerLevelData.Instance.character;
        character.energy += increment;
        energySlider.maxValue = character.energy;
        energyValueUI = character.energy;
    }

    public void SetCharacterEnergy(int increment)
    {
        PlayerLevelData.Instance.character.energy += increment;
        energyValueUI = PlayerLevelData.Instance.character.energy;
    }

    public void SetSkillCounter(int increment)
    {
        PlayerLevelData.Instance.levelData.skillCount += increment;
        this.skillCounter.text = $"{PlayerLevelData.Instance.levelData.skillCount}";
    }

    public void SetPlayerMoves(int increment)
    {
        PlayerLevelData.Instance.levelData.moves += increment;
        this.movesLeftTMP.text = $"{PlayerLevelData.Instance.levelData.moves}";
    }

    public void ShowCurrentPath()
    {
        if (!pathDisplayToggle)
            pathDisplayToggle = true;
        else
            pathDisplayToggle = false;
        NodeGrid.ToggleGridTiles(pathDisplayToggle);
        // PlayerLevelData.Instance.character.DisplayPath(pathDisplayToggle);
    }

    public void UndoAction()
    {
        Debug.Log("Pressed Undo Button!");
        PlayerActions.Instance.Undo();
    }

    public void ReloadAction()
    {
        GameEvent.RestartGame();
    }

    public void PlayAction()
    {
        PlayerLevelData.Instance.character.InitCharacter();
    }

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
}
