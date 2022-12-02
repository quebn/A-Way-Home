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

    private void Start()
    {
        InitData();
    }

    private void InitData ()
    {
        if (GameData.Instance.leaderboards.Count <= slotIndexNumber)
            return;
        PlayerScoreData data = GetRankingData(slotIndexNumber);
        if (data != null)
        {
            this.noData.SetActive(false);
            this.data.SetActive(true);
            SetValues(data);
            return;
        }
        this.data.SetActive(false);
        this.noData.SetActive(true);
    }

    private PlayerScoreData GetRankingData(int index)
    {
        GameData.Instance.leaderboards.Sort();
        if (GameData.Instance.leaderboards[index] == null)
        {
            return null;
        }
        PlayerScoreData data = GameData.Instance.leaderboards[index]; 
        return data;
    }

    private void SetValues(PlayerScoreData data)
    {
        this.characterImage.sprite = GameData.characterSprites[data.charName];
        this.playerName.text = data.playerName;
        this.characterName.text = data.charName;
        this.score.text = data.score.ToString();
    }
}