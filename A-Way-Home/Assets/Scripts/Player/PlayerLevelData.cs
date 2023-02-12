using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance {get; private set;}

    [SerializeField] private uint characterLevel;
    [SerializeField] private int characterEnergy;
    [SerializeField] private int playerLives;
    [SerializeField] private int playerMoves;
    [SerializeField] private int essenceNeeded;
    [SerializeField] private float timeLimitInSecs;
    [SerializeField] private Vector2 cameraBoundary;
    [SerializeField] public int unlockedTools = 0;
    // [HideInInspector] public LevelData levelData;// should be in GameData
    [HideInInspector] public List<Vector3> currentDestinations;// should be in GameData
    public Vector2 levelBoundary => this.cameraBoundary;
    public GameObject logPrefab;
    public static string characterName;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Instance == null)
            Instance  = this;
        Essence.list = new Dictionary<Vector2, Essence>();
        this.currentDestinations = new List<Vector3>();
        // this.saveables = FindAllSaveableObjects();
        // Debug.Log($"Saveable objects count: {saveables.Count}"); 
        SaveSystem.saveables = GetAllSaveables();
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
        LoadSpawnedObstacles();

    }

    private void Start()
    {
        Debug.LogWarning($"[{GameEvent.loadType.ToString()}]: Initializing Character with {GameData.levelData.characterEnergy} energy and {GameData.levelData.characterRequiredEssence} ");
        Character.instance.Initialize(GameData.levelData.characterEnergy, GameData.levelData.characterRequiredEssence);
    }

    private void NewGame()
    {
        GameData.levelData = new LevelData {
            level = this.characterLevel,
            characterName = characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = 0,
            spawnCount = 0,
            secondsLeft = this.timeLimitInSecs,
            obstacles = new Dictionary<string, ObstacleData>(),
            // spawneds = new Dictionary<string, ObstacleData>(),
            essences = new Dictionary<string, bool>()
        };
    }

    private void RestartGame()
    {
        GameData.levelData = new LevelData {
            level = this.characterLevel,
            characterName = characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives - GameEvent.restartCounter,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = GameData.levelData.score, //<-TODO: score should be retained from previous game
            spawnCount = 0,
            secondsLeft = this.timeLimitInSecs,
            obstacles = new Dictionary<string, ObstacleData>(),
            // spawneds = new Dictionary<string, ObstacleData>(),
            essences = new Dictionary<string, bool>()
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
    }
    
    public void LoadGame()
    {
        Debug.Assert(GameData.levelData != null, "ERROR: No load level data found");
        Debug.Assert(this.characterLevel == GameData.levelData.level, "ERROR: Level does not match");
        foreach(KeyValuePair<string, ObstacleData> pair in GameData.levelData.obstacles)
            Debug.LogWarning($"{pair.Key} -> {pair.Value}");
        foreach(ISaveable saveable in SaveSystem.saveables)
            saveable.LoadData(GameData.levelData);
    }

    public LevelData SaveGame()
    {
        foreach(ISaveable saveable in SaveSystem.saveables)
            saveable.SaveData(GameData.levelData);
        return GameData.levelData;
    }

    public List<ISaveable> GetAllSaveables()
    {
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        return new List<ISaveable>(saveables);
    }

    public int GetScore(int movesMultiplier, int livesMultiplier)
    {
        return (playerMoves * movesMultiplier) + (playerLives * livesMultiplier);
    }

    public void LoadSpawnedObstacles()
    {
        for (int i = 1; i <= GameData.levelData.spawnCount; i++)
        {
            string ID = $"{i}";
            if(GameData.levelData.obstacles[ID].typeName == typeof(TreeLog).ToString())
            {
                TreeLog log = GameObject.Instantiate(logPrefab, GameData.levelData.obstacles[ID].position, Quaternion.identity).GetComponent<TreeLog>();
                log.AddAsSpawned(ID);
            }
        }
        // List<ObstacleData> spawnedDatas = new List<ObstacleData>(GameData.levelData.spawneds.Values.ToList());
        // foreach(ObstacleData spawnedData in spawnedDatas)
        // {
            // if(spawnedData.typeName == typeof(TreeLog).ToString()){
                // TreeLog log = GameObject.Instantiate( logPrefab, spawnedData.spawnedLocation, Quaternion.identity, this.gameObject.transform).GetComponent<TreeLog>();
                // log.AddAsSpawned();
            // }
        // }
        SaveSystem.saveables = GetAllSaveables();
    }

    [SerializeField] private bool enableBoundaryGizmo;
    private void OnDrawGizmos()
    {
        if (!enableBoundaryGizmo)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(cameraBoundary.x, cameraBoundary.y, 0));
    }

}