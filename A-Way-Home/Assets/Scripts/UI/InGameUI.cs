using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public static InGameUI Instance {get; private set;}
    [HideInInspector] public EndGameType endGameType;
    [SerializeField] private GameObject optionsWindow;
    [SerializeField] private GameObject gameEndWindow;
    [SerializeField] private GameObject howToPlayWindow;
    [SerializeField] private Image characterImage;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI characterNameTMP;
    [SerializeField] private TextMeshProUGUI movesLeftTMP;
    [SerializeField] private TextMeshProUGUI energyLeftTMP;
    [SerializeField] private TextMeshProUGUI livesLeftTMP;
    [SerializeField] private TextMeshProUGUI essenceNeededTMP;
    [SerializeField] private TextMeshProUGUI timeCounter;

    [HideInInspector] public bool activatedCharacter = false;
    public GameObject getGameEndWindow => gameEndWindow;

    public int energyValueUI {
        set { 
            energyLeftTMP.text = $"{value}";
            energySlider.value = value;
        }
    }
    public int energyMaxValueUI {
        set { 
            energyLeftTMP.text = $"{value}";
            energySlider.maxValue = value;
            energySlider.value = value;
        }
    }

    public int essenceCounterUI {
        set => this.essenceNeededTMP.text = $"{value}";
    }

    public int playerMovesUI {
        set =>  this.movesLeftTMP.text = $"{value}";
    }

    private float timeCounterUI{
        get => GameData.levelData.secondsLeft; 
        set {
            GameData.levelData.secondsLeft = value;
            timeCounter.text = $"{(int)timeCounterUI}";
        }
    }

    private void Awake()
    {
        GameEvent.InitializeLevel();
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        InitCharacterUI(GameData.levelData, Character.instance);
        activatedCharacter = false;
        endGameType = EndGameType.None;
    }

    public void TimeCountdown()
    {
        if (timeCounterUI > 0)
        {
            timeCounterUI -= Time.deltaTime;
        }
        if (timeCounterUI <= 0 && !GameEvent.isEndWindowActive && !activatedCharacter)
        {
            if(Character.instance.hasPath)
                Character.instance.GoHome();
            else
                Character.instance.TriggerDeath();
            activatedCharacter = true;
        }
    }

    private void InitCharacterUI(LevelData levelData, Character character) 
    {
        this.characterImage.sprite  = character.image;
        this.characterNameTMP.text  = levelData.characterName;
        this.movesLeftTMP.text      = levelData.moves.ToString();
        this.livesLeftTMP.text      = levelData.lives.ToString();
        this.energyValueUI          = levelData.characterEnergy;
    }

    public void ShowCurrentPath()
    {
        if(Character.instance.isMoving)
            return;
        NodeGrid.nodesVisibility = !NodeGrid.nodesVisibility;
        NodeGrid.ToggleGridTiles(NodeGrid.nodesVisibility);
    }

    public void HelpButton()
    {
        Debug.Log("Pressed Help Button!");
        GameEvent.PauseGame();
        this.howToPlayWindow.SetActive(true);
    }
    
    public void CloseHowToPlay()
    {
        GameEvent.UnpauseGame();
        this.howToPlayWindow.SetActive(false);
    }

    public void OptionsButton()
    {
        GameEvent.PauseGame();
        Debug.Assert(GameEvent.isPaused, "ERROR: Game is not paused!");
        this.optionsWindow.SetActive(true);
    }

    public void SetTool(int toolIndex)
    {
        PlayerActions.Instance.SetCurrentTool(toolIndex);
        // PlayerActions.Instance.currentManipulationType = (ManipulationType)toolindex;
    }

    public void ReloadAction()
    {
        GameEvent.SetEndWindowActive(EndGameType.Restart);
    }

    public void PlayAction()
    {
        if (Character.instance.isMoving)
            return;
        if(GameData.levelData.moves == 0 && !Character.instance.hasPath)
            Character.instance.TriggerDeath();
        else
            Character.instance.GoHome();
    }
}
