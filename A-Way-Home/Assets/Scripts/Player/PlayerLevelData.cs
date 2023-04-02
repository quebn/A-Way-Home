using System;
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
    // [HideInInspector] public LevelData levelData;// should be in GameData
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
        SaveSystem.saveables = GetAllSaveables();
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
            LoadSpawnedObstacles();
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
            Debug.LogWarning($"{pair.Key} -> {pair.Value}");
        foreach(ISaveable saveable in SaveSystem.saveables)
            saveable.LoadData(GameData.levelData);
    }

    public LevelData SaveLevelData()
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
        return (playerMoves * movesMultiplier) + (GameData.levelData.lives * livesMultiplier);
    }

    public void LoadSpawnedObstacles()
    {
        throw new NotImplementedException();
        // for (int i = 1; i <= GameData.levelData.spawnCount; i++)
        // {
        //     string ID = $"{i}";
        //     if(GameData.levelData.obstacles[ID].typeName == typeof(TreeLog).ToString())
        //     {
        //         TreeLog log = GameObject.Instantiate(logPrefab, GameData.levelData.obstacles[ID].position, Quaternion.identity).GetComponent<TreeLog>();
        //         log.AddAsSpawned(ID);
        //     }
        // }
        // SaveSystem.saveables = GetAllSaveables();
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