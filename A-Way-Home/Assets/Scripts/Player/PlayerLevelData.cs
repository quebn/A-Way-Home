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
    [SerializeField] private int characterEnergy;
    [SerializeField] private int characterSkillCount;
    [SerializeField] private int playerLives;
    [SerializeField] private int playerMoves;
    [HideInInspector] public LevelData levelData;
    public static Dictionary<string, GameObject> gameObjectList;
    public static string characterName;
    public int minimumEnergy {get { return characterEnergy - 5; } }

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
            characterName = characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives,
            moves = playerMoves,
            score = 0,
            skillCount = this.characterSkillCount,
            actionList = new List<Action>(),
            removedObstacles = new Dictionary<string, bool>()
        };
    }

    private void RestartGame()
    {
        uint currentLevel = this.characterLevel;
        this.levelData = new LevelData {
            sceneName = SceneManager.GetActiveScene().name,
            level = currentLevel,
            characterName = characterName,
            characterEnergy = this.characterEnergy,
            lives = playerLives - GameEvent.restartCounter,
            moves = playerMoves,
            score = 0, //<-TODO: score should be retained from previous game
            skillCount = this.characterSkillCount,
            actionList = new List<Action>(),
            removedObstacles = new Dictionary<string, bool>()//should be private
        };
        Debug.Assert(playerLives > 0, "ERROR: Lives is less than 1");
    }

    public void LoadGame()
    {
        this.levelData = GameData.loadedLevelData.levelData;
        Debug.Assert(this.characterLevel == levelData.level, "ERROR: Level does not match");
    }

    public static void AddRemovedToList(ManipulationType manipulationType, string obstacleID, bool isRemoved)
    {
        Instance.levelData.actionList.Add(new Action(manipulationType, obstacleID));
        Instance.levelData.removedObstacles.Add(obstacleID, isRemoved);
    }
}

[System.Serializable]
public struct LevelData
{
    public string sceneName;
    public uint level;
    public string characterName;
    public int characterEnergy; 
    public int lives;
    public int moves;
    public int score;
    public int skillCount;
    public List<Action> actionList;
    public Dictionary<string, bool> removedObstacles;
}

[System.Serializable]
public struct WorldCoords{
    public float x, y;
    public WorldCoords(Vector2 vector2){
        this.x = vector2.x;
        this.y = vector2.y;
    }
}

[System.Serializable]
public struct Action 
{
    public ManipulationType type;
    private WorldCoords[] skillCoords;
    public string obstacleID;

    public Vector3 skillCoord {
        get { return new Vector3(skillCoords[0].x, skillCoords[0].y, 0);}
        set { skillCoords[0].x = value.x; skillCoords[0].y = value.y;}
    }

    public Vector3 GetCoordByIndex(int index)
    {
        return new Vector3(skillCoords[index].x, skillCoords[index].y, 0);
    }

    public Action(ManipulationType manipulationType, Vector3 skillCoord, string ID)
    {
        this.type = manipulationType;
        this.skillCoords = new WorldCoords[1];
        this.skillCoords[0] = new WorldCoords(skillCoord);
        this.obstacleID = ID;
    }

    public Action(ManipulationType manipulationType, WorldCoords[] worldCoords)
    {
        this.type = manipulationType;
        this.skillCoords = worldCoords;
        this.obstacleID = "";
    }
    
    public Action(ManipulationType manipulationType, string ID)
    {
        this.type = manipulationType;
        this.skillCoords = new WorldCoords[0];
        this.obstacleID = ID;
    }
}
