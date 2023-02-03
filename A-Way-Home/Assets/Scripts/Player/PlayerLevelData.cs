using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance {get; private set;}

    [SerializeField] private uint characterLevel;
    [SerializeField] private int characterEnergy;
    [SerializeField] private int characterSkillCount;
    [SerializeField] private int playerLives;
    [SerializeField] private int playerMoves;
    [SerializeField] private int essenceNeeded;
    [SerializeField] private float timeLimitInSecs;
    [SerializeField] private Vector2 cameraBoundary;

    [HideInInspector] public LevelData levelData;
    [HideInInspector] public List<Vector3> currentDestinations;

    // private List<ISaveable> saveables;

    public Vector2 levelBoundary => this.cameraBoundary;
    public static string characterName;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Instance == null)
            Instance  = this;
        Obstacle.list = new Dictionary<string, Obstacle>();
        Essence.list = new Dictionary<Vector2, Essence>();
        this.currentDestinations = new List<Vector3>();
        // this.saveables = FindAllSaveableObjects();
        // Debug.Log($"Saveable objects count: {saveables.Count}"); 
        switch(GameEvent.loadType)
        {
            case LevelLoadType.NewGame:
                NewGame();
                break;
            case LevelLoadType.LoadGame:
                LoadGame();
                break;
            case LevelLoadType.RestartGame:
                RestartGame();
                break;
        }
    }

    private void Start()
    {
        Character.instance.Initialize(levelData.characterEnergy, levelData.characterRequiredEssence);
    }

    private void NewGame()
    {
        uint currentLevel = this.characterLevel;
        this.levelData = new LevelData {
            level = currentLevel,
            characterName = characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = 0,
            secondsLeft = this.timeLimitInSecs
        };
    }

    private void RestartGame()
    {
        uint currentLevel = this.characterLevel;
        this.levelData = new LevelData {
            level = currentLevel,
            characterName = characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives - GameEvent.restartCounter,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = 0, //<-TODO: score should be retained from previous game
            secondsLeft = this.timeLimitInSecs
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
    }
    
    public void LoadGame()
    {
        this.levelData = GameData.loadedLevelData.levelData;
        // foreach(ISaveable saveable in saveables)
        //     saveable.LoadData(this.levelData);
        Debug.Assert(this.characterLevel == levelData.level, "ERROR: Level does not match");
    }

    public LevelData SaveGame()
    {
        // foreach(ISaveable saveable in saveables)
        //     saveable.SaveData(ref this.levelData);
        return this.levelData;
    }

    public void IncrementPlayerMoves(int increment)
    {
        this.levelData.moves += increment;
        InGameUI.Instance.playerMovesUI = this.levelData.moves;
    }

    public int GetScore(int movesMultiplier, int livesMultiplier)
    {
        return (playerMoves * movesMultiplier) + (playerLives * livesMultiplier);
    }

    [SerializeField] private bool enableBoundaryGizmo;
    private void OnDrawGizmos()
    {
        if (!enableBoundaryGizmo)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(cameraBoundary.x, cameraBoundary.y, 0));
    }

    // private static List<ISaveable> FindAllSaveableObjects() 
    // {
    //     // FindObjectsofType takes in an optional boolean to include inactive gameobjects
    //     IEnumerable<ISaveable> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();

    //     return new List<ISaveable>(dataPersistenceObjects);
    // }

}

[System.Serializable]
public struct LevelData
{

    // Character Data
    public string characterName;
    public int characterEnergy;
    public int characterRequiredEssence;
    public SerializedVector3 characterPosition;

    // Player Data 
    public uint level;
    public int lives;
    public int moves;
    public int score;
    public float secondsLeft;
}