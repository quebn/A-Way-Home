using UnityEngine;

public class LevelSelectButtonUI : MonoBehaviour
{
    [SerializeField] private int stage;
    [SerializeField] private int level;
    [SerializeField] private GameObject lockedPanel;
    [SerializeField] private GameObject charSelectWindow;

    public static string selectedStageLevel;

    public void SelectLevel()
    {
        if(!GameData.Instance.unlockedLevels.Contains($"Stage{stage}Level{level}"))
            return;
        selectedStageLevel = $"Stage{stage}Level{level}";
        Debug.Assert(GameData.Instance.unlockedLevels.Contains(selectedStageLevel)|| MainMenuUI.isAllLevelUnlock);
        charSelectWindow.SetActive(true);
    }
    
    private void Initialize()
    {
        lockedPanel.SetActive(!(GameData.Instance.unlockedLevels.Contains($"Stage{stage}Level{level}") || MainMenuUI.isAllLevelUnlock));
    }

    public static void LoadUnlockedLevelsUI()
    {
        LevelSelectButtonUI[] buttonUIs = GameObject.FindObjectsOfType<LevelSelectButtonUI>();
        for(int i = 0; i < buttonUIs.Length; i++)
            buttonUIs[i].Initialize();
    }
}
