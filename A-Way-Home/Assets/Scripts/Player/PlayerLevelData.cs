using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance;
    public Character character;
    public Transform characterHome;
    public Animator homeAnimator;
    public bool sandboxMode;
    [SerializeField] private uint characterLevel;
    [SerializeField] private string characterName;
    [SerializeField] private uint characterEnergy;
    [SerializeField] private uint playerLives;
    [SerializeField] private uint playerMoves;
    [HideInInspector] public LevelData levelData;
    
    private void Awake()
    {

        if (Instance != null)
            return;
        Instance  = this;
        if (sandboxMode)
            GameEvent.loadType = LevelLoadType.Sandbox;
        Initialize();
        InitCharacter();
        Debug.Assert(levelData.level != 0, "ERROR: level is 0");
        Debug.Assert(character.home != null, "Error: characterHome is null!");
    }

    private void InitCharacter()
    {
        character.charName = characterName;
        character.home = characterHome;
        // Debug.Log(GameData.Instance.gameSpeed);
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
    private void RestartGame()
    {
        uint currentLevel = this.characterLevel;
        levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            level = currentLevel,
            characterEnergy = this.characterEnergy,
            lives = playerLives - GameEvent.restartCounter,
            moves = playerMoves,
            score = 0, //<-TODO: score should be retained from previous game
            removedObstacles = new Dictionary<string, bool>()
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
        
    }
    private void NewGame()
    {
        uint currentLevel = this.characterLevel;
        levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            level = currentLevel,
            characterEnergy = this.characterEnergy,
            lives = playerLives,
            moves = playerMoves,
            score = 0,
            removedObstacles = new Dictionary<string, bool>()
        };
    }
    public void LoadGame()
    {
        levelData = GameData.loadedLevelData.levelData;
        Debug.Assert(this.characterLevel == levelData.level, "ERROR: Level does not match");
    }
}

[System.Serializable]
public struct LevelData
{
    public string sceneName;
    public uint level;
    public uint characterEnergy; 
    public uint lives;
    public uint moves;
    public uint score;
    public Dictionary<string, bool> removedObstacles;
}