using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance {get; private set;}
    public Character character;
    public Transform characterHome;
    public Animator homeAnimator;
    public bool sandboxMode;
    [SerializeField] private uint characterLevel;
    [SerializeField] private string characterName;
    [SerializeField] private uint characterEnergy;
    [SerializeField] private uint characterSkillCount;
    [SerializeField] private uint playerLives;
    [SerializeField] private uint playerMoves;
    [HideInInspector] public LevelData levelData;
    
    private void Awake()
    {
        if (sandboxMode)
            GameEvent.loadType = LevelLoadType.Sandbox;
        Initialize();
        InitCharacter();
        if (Instance != null)
            return;
        Instance  = this;
        Debug.Assert(levelData.level != 0, "ERROR: level is 0");
        Debug.Assert(character.homePosition != null, "Error: characterHome is null!");
        Display();
    }

    private void InitCharacter()
    {
        character.homePosition = characterHome.transform.position;
        character.energy = levelData.characterEnergy;
        if (sandboxMode)
        {
            character.speed = 5f;    
            return;
        }
        character.speed = GameData.Instance.gameSpeed;
    }

    private void Initialize()
    {
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
            case LevelLoadType.Sandbox:
                NewGame();
                break;
        }
    }

    private void NewGame()
    {
        uint currentLevel = this.characterLevel;
        this.levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            level = currentLevel,
            characterName = this.characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives,
            moves = playerMoves,
            score = 0,
            skillCount = this.characterSkillCount,
            skillCoords = new List<WorldCoords>(),
            removedObstacles = new Dictionary<string, bool>()
        };
    }

    private void RestartGame()
    {
        uint currentLevel = this.characterLevel;
        this.levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            level = currentLevel,
            characterName = this.characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives - GameEvent.restartCounter,
            moves = playerMoves,
            score = 0, //<-TODO: score should be retained from previous game
            skillCount = this.characterSkillCount,
            skillCoords = new List<WorldCoords>(),
            removedObstacles = new Dictionary<string, bool>()
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
    }

    public void LoadGame()
    {
        this.levelData = GameData.loadedLevelData.levelData;
        Debug.Assert(this.characterLevel == levelData.level, "ERROR: Level does not match");
    }

    private void Display()
    {
        if (levelData.skillCoords.Count == 0)
            return;
        Debug.Log("Skill Coords list:");
        int index = 0;
        foreach(WorldCoords coords in levelData.skillCoords)
        {
            Debug.Log($" --list[{index}] => ({coords.x}, {coords.y})");
            index++;
        }
    }
}

[System.Serializable]
public struct LevelData
{
    public string sceneName;
    public uint level;
    public string characterName;
    public uint characterEnergy; 
    public uint lives;
    public uint moves;
    public uint score;
    public uint skillCount;
    public List<WorldCoords> skillCoords; 
    // public Dictionary<float, float> skillCoords;
    public Dictionary<string, bool> removedObstacles;
}
[System.Serializable]
public struct WorldCoords{
    public float x, y;
    public WorldCoords(float x, float y){
        this.x = x;
        this.y = y;
    }
}