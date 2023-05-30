using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardSlotUI : MonoBehaviour
{
    [SerializeField] private int slotIndexNumber;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject data;
    [SerializeField] private GameObject noData;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI score;

    private void InitData ()
    {
        PlayerScoreData data = GetRankingData(slotIndexNumber);
        bool hasData = data != null;
        this.noData.SetActive(!hasData);
        this.data.SetActive(hasData);
        if (hasData)
            SetValues(data);
    }

    private PlayerScoreData GetRankingData(int index)
    {
        GameData.Instance.leaderboards.Sort();
        if (GameData.Instance.leaderboards == null || GameData.Instance.leaderboards.Count == 0 || GameData.Instance.leaderboards.Count <= slotIndexNumber)
            return null;
        PlayerScoreData data = GameData.Instance.leaderboards[index]; 
        return data;
    }

    private void SetValues(PlayerScoreData data)
    {
        this.characterImage.sprite = Resources.Load<Sprite>($"Characters/Images/{data.charName}");
        this.playerName.text = data.playerName;
        this.characterName.text = data.charName;
        this.score.text = data.score.ToString();
    }

    public static void LoadLeaderbordsInfo()
    {
        LeaderboardSlotUI[] slots =  GameObject.FindObjectsOfType<LeaderboardSlotUI>();
        if(slots == null || slots.Length == 0)
            return;
        for(int i = 0; i < slots.Length; i++)
            slots[i].InitData();
    }
}