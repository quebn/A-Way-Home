using UnityEngine;

public class LevelSelectButtonUI : MonoBehaviour
{
    [SerializeField] private int stage;
    [SerializeField] private int level;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private GameObject charSelectWindow;

    public static string selectedStageLevel;

    private void Start()
    {
        Initialize();
    }

    public void SelectLevel()
    {
        selectedStageLevel = $"Stage{stage}Level{level}";
        Debug.Assert(GameData.Instance.unlockedLevels.Contains(selectedStageLevel));
        charSelectWindow.SetActive(true);
    }
    
    private void Initialize()
    {
        if (GameData.Instance.unlockedLevels.Contains($"Stage{stage}Level{level}"))
            lockedPanel.SetActive(false);
        else
            lockedPanel.SetActive(true);
    }
}
