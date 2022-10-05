using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance;
    public Character character;
    public Transform characterHome;
    [SerializeField] private string characterName;
    [SerializeField] private uint characterEnergy;
    [SerializeField] private uint playerLives;
    [SerializeField] private uint playerMoves;
    [HideInInspector] public LevelData levelData;

    public void Awake()
    {
        // Debug.Assert(GameEvent.loadType != null, "Error: loadType is null!");
        InitCharacter();
        if (Instance != null)
            return;
        Instance  = this;
        Debug.Assert(character.home != null, "Error: characterHome is null!");
    }    
    public void Start()
    {
        Initialize();
    }

    private void InitCharacter()
    {
        character.charName = characterName;
        character.home = characterHome;
        character.energy = characterEnergy;
        // character.speed = GameData.Instance.gameSpeed;
        character.speed = 10f;
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
        }
    }

    private void RestartGame()
    {
        levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            lives = playerLives - GameEvent.restartCounter,
            moves = playerMoves,
            score = 0, //<-TODO: score should be retained from previous game
            removedObstacles = new Dictionary<string, bool>()
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
        
    }
    private void NewGame()
    {
        levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            lives = playerLives,
            moves = playerMoves,
            score = 0,
            removedObstacles = new Dictionary<string, bool>()
        };
    }
    public void LoadGame()
    {
        levelData = GameData.loadedLevelData.levelData;
    }
}

[System.Serializable]
public struct LevelData
{
    public string sceneName;
    public uint lives;
    public uint moves;
    public uint score;
    public Dictionary<string, bool> removedObstacles;
}