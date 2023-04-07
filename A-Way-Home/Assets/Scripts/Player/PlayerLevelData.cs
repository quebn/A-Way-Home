using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance {get; private set;}

    [SerializeField] private uint currentStage;
    [SerializeField] private uint currentLevel;
    [SerializeField] private int characterEnergy;
    [SerializeField] private int playerMoves;
    [SerializeField] private int essenceNeeded;
    [SerializeField] private float timeLimitInSecs;
    [SerializeField] private Vector2 cameraBoundary;
    [SerializeField] public int unlockedTools = 0;
    [SerializeField] private GameObject characterLocation;
    [SerializeField] private Vector2 startCameraPos;
    [HideInInspector] public List<Vector3> currentDestinations;// should be in GameData

    public string currentStageLevel => $"Stage{currentStage}Level{currentLevel}";
    public Vector2 levelBoundary => this.cameraBoundary;
    public Vector3 cameraCenterPos => startCameraPos;
    public GameObject logPrefab;
    public GameObject miasmaPrefab;


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
        // Debug.Assert(GameData.levelData != null , "ERROR:LevelData is null");
        if(!GameEvent.isSceneSandbox)
            Debug.Assert(GameData.characters != null || GameData.characters.Count != 0, "ERROR:Characters not found!");
        switch(GameEvent.loadType)
        {
            case LevelLoadType.NewGame:
                NewLevelData();
                break;
            case LevelLoadType.LoadGame:
                LoadLevelData();
                break;
            case LevelLoadType.RestartGame:
                RestartLevelData();
                break;
            case LevelLoadType.ContinueGame:
                ContinueLevelData();
                break;
        }
        Debug.LogWarning($"Current Score:{GameData.levelData.score}");
        if(!GameEvent.isSceneSandbox)
            GameObject.Instantiate(
                GameData.characters[GameData.levelData.characterName], 
                this.characterLocation.transform.position, 
                Quaternion.identity, 
                this.transform
            );
        Destroy(characterLocation);
        if(GameEvent.loadType == LevelLoadType.LoadGame)
            LoadObstacles();
    }

    private void Start()
    {
        Debug.LogWarning($"[{GameEvent.loadType.ToString()}]: Initializing Character with {GameData.levelData.characterEnergy} energy and {GameData.levelData.characterRequiredEssence} ");
        Character.instance.Initialize(GameData.levelData);
    }


    private void NewLevelData()
    {
        GameData.levelData = new LevelData {
            stage = this.currentStage,
            level = this.currentLevel,
            characterName = GameData.selectedCharacter,
            characterEnergy = this.characterEnergy,
            lives = GameData.PLAYER_STARTING_LIVES,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = 0,
            spawnCount = 0,
            secondsLeft = this.timeLimitInSecs,
            obstacles = new Dictionary<string, ObstacleData>(),
            essences = new Dictionary<string, bool>()
        };
    }

    private void RestartLevelData()
    {
        GameData.levelData = new LevelData {
            stage = this.currentStage,
            level = this.currentLevel,
            characterName = GameData.selectedCharacter,
            characterEnergy = this.characterEnergy,
            lives  = GameData.levelData.lives - 1,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = GameData.levelData.score, //<-TODO: score should be retained from previous game
            spawnCount = 0,
            secondsLeft = this.timeLimitInSecs,
            obstacles = new Dictionary<string, ObstacleData>(),
            essences = new Dictionary<string, bool>()
        };
        Debug.Assert(GameData.levelData.lives > 0, "ERROR: Lives is less than 1");
    }

    private void ContinueLevelData()
    {
        GameData.levelData = new LevelData {
            stage = this.currentStage,
            level = this.currentLevel,
            characterName = GameData.selectedCharacter,
            characterEnergy = this.characterEnergy,
            lives = GameData.levelData.lives,
            moves = playerMoves,
            characterRequiredEssence = this.essenceNeeded,
            score = GameData.levelData.score, //<-TODO: score should be retained from previous game
            spawnCount = 0,
            secondsLeft = this.timeLimitInSecs,
            obstacles = new Dictionary<string, ObstacleData>(),
            essences = new Dictionary<string, bool>()
        };
        Debug.Assert(GameData.levelData.lives > 0, "ERROR: Lives is less than 1");
    }

    private void LoadLevelData()
    {
        Debug.Assert(GameData.levelData != null, "ERROR: No load level data found");
        Debug.Assert(this.currentLevel == GameData.levelData.level, "ERROR: Level does not match");
        GameData.selectedCharacter = GameData.levelData.characterName;
        foreach(KeyValuePair<string, ObstacleData> pair in GameData.levelData.obstacles)
            Debug.Log($"Loaded Leveldata Obstacles :{pair.Value.typeName} with hp: {pair.Value.valuePairs["hp"]} -> {pair.Key}");

    }

    public LevelData SaveLevelData()
    {
        List<ISaveable> saveables = GetAllSaveables();
        foreach(ISaveable saveable in saveables)
            saveable.SaveData(GameData.levelData);
        return GameData.levelData;
    }

    private void LoadObstacles()
    {
        Debug.LogWarning("-------------------Spawning Obstacles-----------------");
        List<ISaveable> saveables = GetAllSaveables();
        Debug.LogWarning("-------------------Loading Obstacles-----------------");
        foreach(ISaveable saveable in saveables)
            saveable.LoadData(GameData.levelData);
        LoadSpawnedObstacles();
    }

    public List<ISaveable> GetAllSaveables()
    {
        IEnumerable<ISaveable> saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        return new List<ISaveable>(saveables);
    }

    public int GetScore(int movesMultiplier, int livesMultiplier)
    {
        return (playerMoves * movesMultiplier) + (GameData.levelData.lives * livesMultiplier);
    }

    public void LoadSpawnedObstacles()
    {
        GameObject fireField = Resources.Load<GameObject>("Spawnables/FireField");
        GameObject lilyPad = Resources.Load<GameObject>("Spawnables/Lilypad");
        GameObject logPlatform = Resources.Load<GameObject>("Spawnables/LogPlatform");
        GameObject logSpawnable = Resources.Load<GameObject>("Spawnables/LogSpawnable");
        GameObject poisonMiasma = Resources.Load<GameObject>("Spawnables/PoisonMiasma");
        Debug.Assert(fireField != null);
        Debug.Assert(lilyPad != null);
        Debug.Assert(logPlatform != null);
        Debug.Assert(logSpawnable != null);
        Debug.Assert(poisonMiasma != null);
        uint count = GameData.levelData.spawnCount;
        GameData.levelData.spawnCount = 0;
        for (uint i = 1; i <= count; i++)
        {
            string ID = $"{i}";
            if(!GameData.levelData.obstacles.ContainsKey(ID))
                continue;
            ObstacleData obstacleData = GameData.levelData.obstacles[ID];
            GameObject prefab;
            switch(obstacleData.typeName)
            {
                case "FireField":
                    prefab = fireField;
                    break;
                case "LilyPad":
                    prefab = lilyPad;
                    break;
                case "LogPlatform":
                    prefab = logPlatform;
                    break;
                case "LogSpawned":
                    prefab = logSpawnable;
                    break;
                case "PoisonMiasma":
                    prefab = poisonMiasma;
                    break;
                default:
                    Debug.LogWarning($"TYPENAME: {obstacleData.typeName} not identified!!!!");
                    continue;
            }
            ISaveable saveable = GameObject.Instantiate(prefab, obstacleData.position, Quaternion.identity).GetComponent<Spawnable>();
            saveable.LoadData(GameData.levelData);
            Debug.Log($"Type name:{obstacleData.typeName} \n hitpoints: {obstacleData.GetValue("hp")} \n position: {obstacleData.position.ToString()}");
        }
    }


    public string GetNextStageLevel()
    {
        Debug.Assert(this.currentStage < 4 || this.currentStage > 0, $"ERROR: Unexpected Stage value: {this.currentStage}");
        Debug.Assert(this.currentLevel < 6, $"ERROR: Unexpected Level value: {this.currentLevel}");
        uint stage = (this.currentLevel == 5)? currentStage + 1 : currentStage;
        uint level = (this.currentLevel == 5)?  1 : currentLevel + 1;
        return $"Stage{stage}Level{level}";
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