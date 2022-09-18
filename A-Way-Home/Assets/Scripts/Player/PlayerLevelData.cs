using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelLoadType{ NewGame, LoadGame}// <- should not exist
public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance;
    public Character character;
    public Transform characterHome;
    public uint characterEnergy;
    public uint playerLives;
    public uint playerMoves;
    [HideInInspector] public Dictionary<string, bool> removedObstacles;
    [HideInInspector] public string levelSceneName;


    public void Awake()
    {
        Debug.Assert(GameData.loadType != null, "Error: loadType is null!");
        character.home = characterHome;
        character.energy = characterEnergy;
        character.speed = 10f;
        levelSceneName = SceneManager.GetActiveScene().name;
        removedObstacles = new Dictionary<string, bool>();
        if (Instance != null)
        {
            return;
        }
        Instance  = this;
        Debug.Assert(character.home != null, "Error: characterHome is null!");
    }
    
    public void Start()
    {
        if (GameData.loadType == LevelLoadType.LoadGame) //<- should not exist
            LoadGame();
    }

    public void LoadGame()
    {
        playerLives = GameData.loadedLevelData.playerLives;
        playerMoves = GameData.loadedLevelData.playerMoves;
        removedObstacles = GameData.loadedLevelData.removedObstacles;
    }

}
