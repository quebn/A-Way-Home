using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance {get; private set;}
    public Character character;
    public Transform characterHome;
    public Animator homeAnimator;

    [SerializeField] private uint characterLevel;
    [SerializeField] private int characterEnergy;
    [SerializeField] private int characterSkillCount;
    [SerializeField] private int playerLives;
    [SerializeField] private int playerMoves;
    [SerializeField] private float timeLimitInSecs;

    [HideInInspector] public LevelData levelData;

    public static Dictionary<string, GameObject> gameObjectList;
    public static string characterName;

    private void Awake()
    {
        Initialize();
        if (Instance != null)
            return;
        Instance  = this;
    }

    private void InitCharacter()
    {
        character.homePosition = characterHome.transform.position;
        character.energy = levelData.characterEnergy;
        if (GameEvent.isSceneSandbox)
        {
            character.speed = 5f;    
            return;
        }
        character.speed = GameData.Instance.gameSpeed;
    }

    private void Initialize()
    {
        gameObjectList = new Dictionary<string, GameObject>();
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
        Debug.Assert(character != null, "Character is null");
        InitCharacter();
        Debug.Assert(levelData.level != 0, "ERROR: level is 0");
        Debug.Assert(character.homePosition != null, "Error: characterHome is null!");
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
