using UnityEngine;

public class LevelSelectButtonUI : MonoBehaviour
{
    [SerializeField] private string sceneLevelName;
    [SerializeField] private GameObject lockedPanel;
    private void Start()
    {
        Initialize();
    }

    public void SelectLevel()
    {
        if (GameData.Instance.unlockLevels.Contains(sceneLevelName))
            GameEvent.NewGame(sceneLevelName);
    }
    
    private void Initialize()
    {
        if (GameData.Instance.unlockLevels.Contains(sceneLevelName))
            lockedPanel.SetActive(false);
        else
            lockedPanel.SetActive(true);
    }
}
