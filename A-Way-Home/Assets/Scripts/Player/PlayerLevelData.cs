using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelLoadType{ NewGame, LoadGame}
public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance;
    public Character Character;
    public Transform CharacterHome;
    public uint CharacterEnergy;
    public uint PlayerLives;
    public uint PlayerMoves;
    [HideInInspector] public Dictionary<string, bool> RemovedObstacles;
    [HideInInspector] public string LevelSceneName;


    public void Awake()
    {
        Debug.Assert(GameData.LoadType != null, "Error: LoadType is null!");
        Character.Home = CharacterHome;
        Character.Energy = CharacterEnergy;
        Character.Speed = 10f;
        LevelSceneName = SceneManager.GetActiveScene().name;
        RemovedObstacles = new Dictionary<string, bool>();
        if (Instance != null)
        {
            return;
        }
        Instance  = this;
        Debug.Assert(Character.Home != null, "Error: CharacterHome is null!");
    }
    
    public void Start()
    {
        if (GameData.LoadType == LevelLoadType.LoadGame)
            LoadGame();
    }

    public void LoadGame()
    {
        PlayerLives = GameData.LoadedLevelData.PlayerLives;
        PlayerMoves = GameData.LoadedLevelData.PlayerMoves;
    }

}
