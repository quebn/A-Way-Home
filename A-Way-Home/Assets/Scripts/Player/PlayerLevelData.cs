using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance {get; private set;}
    public Character character;

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

    public Vector2 levelBoundary => this.cameraBoundary;
    public static string characterName;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Instance != null)
            return;
        Instance  = this;
        Obstacle.list = new Dictionary<string, Obstacle>();
        Essence.list = new Dictionary<Vector2, Essence>();
        this.currentDestinations = new List<Vector3>();
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
        // InitCharacter();
    }

    private void Start()
    {
        character.Initialize(characterEnergy, essenceNeeded);
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
            score = 0, //<-TODO: score should be retained from previous game
            secondsLeft = this.timeLimitInSecs
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
    }

    public void LoadGame()
    {
        this.levelData = GameData.loadedLevelData.levelData;
        Debug.Assert(this.characterLevel == levelData.level, "ERROR: Level does not match");
    }

    public void SetPlayerMoves(int increment)
    {
        this.levelData.moves += increment;
        InGameUI.Instance.playerMovesUI = $"{this.levelData.moves}";
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

[System.Serializable]
public struct LevelData
{
    public uint level;
    public string characterName;
    public int characterEnergy; 
    public int lives;
    public int moves;
    public int score;
    public float secondsLeft;
}

[System.Serializable]
public struct WorldCoords{
    public float x, y;
    public WorldCoords(Vector2 vector2){
        this.x = vector2.x;
        this.y = vector2.y;
    }
}
