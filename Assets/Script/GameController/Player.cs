using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
public class Player : MonoBehaviour
{
    public struct PlayerInfo
    {
        public string PlayFabID;
        public int ID;
        public string Name;
        public int Score;
        public string Region;
        public string Rank;
    }

    [Header("PlayFab")]
    public TMP_Text leaderboardText;

    public GameObject panelBXH;
    [Header("Info Player")]
    public TMP_Text playerIDText;
    public TMP_Text playerNameText;
    public TMP_Text playerScoreText;
    public TMP_Text playerRegionText;
    public TMP_Text playerRankText;

    [Header("Tìm người chơi")]
    public TMP_InputField sreachID;
    private GameController gameController;
    //Danh sách List
    private List<PlayerInfo> players = new List<PlayerInfo>();
    //Quốc gia được Random ngẫu nhiên kk
    private List<string> regions = new List<string>()
    {
        "VN",
        "Nga",
        "Trung",
        "Pháp",
        "Mỹ",
        "Nhật",
        "Hàn"
    };
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        LoadPlayerData();
        UpdatePlayerList();
        ClerPlayerInfo();
    }
    public void UpdatePlayerList()
    {
        playerIDText.text = "";
        playerNameText.text = "";
        playerScoreText.text = "";
        playerRegionText.text = "";
        playerRankText.text = "";
        foreach (PlayerInfo player in players)
        {
            playerIDText.text += player.ID + "\n\n";
            playerNameText.text += player.Name + "\n\n";
            playerScoreText.text += player.Score + "\n\n";
            playerRegionText.text += player.Region + "\n\n";
            playerRankText.text += player.Rank + "\n\n";
        }
    }

    //Tránh sử dụng với public bên dưới tránh lỗi do truyền tham số
    public void UpdatePlayerList(List<PlayerInfo> playerList)
    {
        playerIDText.text = "";
        playerNameText.text = "";
        playerScoreText.text = "";
        playerRegionText.text = "";
        playerRankText.text = "";
        foreach (PlayerInfo player in players)
        {
            playerIDText.text += player.ID + "\n\n";
            playerNameText.text += player.Name + "\n\n";
            playerScoreText.text += player.Score + "\n\n";
            playerRegionText.text += player.Region + "\n\n";
            playerRankText.text += player.Region + "\n\n";
        }
    }
    private void LoadPlayerData()
    {
        players.Clear();
        if (PlayerPrefs.HasKey("PlayerCount"))
        {
            int playerCount = PlayerPrefs.GetInt("PlayerCount");
            for (int i = 0; i < playerCount; i++)
            {
                PlayerInfo player = new PlayerInfo();
                player.ID = PlayerPrefs.GetInt("PlayerID_" + (i + 1).ToString(), i + 1);
                player.Name = PlayerPrefs.GetString("PlayerName_" + player.ID.ToString(), "");
                player.Score = PlayerPrefs.GetInt("PlayerScore_" + player.ID.ToString(), 0);
                player.Region = PlayerPrefs.GetString("PlayerRegion_" + player.ID.ToString(), GetRandomRegion());
                player.Rank = GetRank(player.Score);
                players.Add(player);
            }
        }
    }
    private string GetRandomRegion()
    {
        int randomIndex = Random.Range(0, regions.Count);
        return regions[randomIndex];
    }
    public void OpenBXH()
    {
        panelBXH.SetActive(true);
    }

    public void CloseBXH()
    {
        panelBXH.SetActive(false);
    }
    private void ClerPlayerInfo()
    {
        players.Clear();
        UpdatePlayerList();
    }
    public void ClearAllPlayerData()
    {
        PlayerPrefs.DeleteAll();
        players.Clear();
        UpdatePlayerList();
        SavePlayerData();
    }
    private void SavePlayerData()
    {
        if (players.Count > 0)
        {
            PlayerPrefs.SetInt("PlayerCount", players.Count);
            for (int i = 1; i < players.Count; i++)
            {
                PlayerPrefs.SetInt("PlayerID_" + (i + 1).ToString(), players[i].ID);
                PlayerPrefs.SetString("PlayerName_" + (i + 1), players[i].Name);
                PlayerPrefs.SetInt("PlayerScore_" + (i + 1), players[i].Score);
                PlayerPrefs.SetString("PlayerRegion_" + (i + 1), players[i].Region);
                PlayerPrefs.SetString("PlayerRank_" + (i + 1), players[i].Rank);
            }
            PlayerPrefs.Save();
        }
    }
    //Lấy bảng xếp hạng từ play fab
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10,
        };
        PlayFabClientAPI.GetLeaderboard(request, OngetLeaderboardSucces, OnErrorGet);
    }
    private void OngetLeaderboardSucces(GetLeaderboardResult result)
    {
        int newID = 1;
        System.Random random = new System.Random();
        foreach (var point in result.Leaderboard)
        {
            PlayerInfo player = new PlayerInfo();
            player.PlayFabID = point.PlayFabId;
            player.Name = point.DisplayName;
            player.Score = (int)point.StatValue;
            player.ID = newID++;
            player.Region = regions[random.Next(regions.Count)];
            player.Rank = GetRank(player.Score);
            players.Add(player);
        }
        UpdatePlayerList();
    }
    private void OnErrorGet(PlayFabError error)
    {
        Debug.LogError("Lỗi khi lấy dữ liệu từ PlayFab: " + error.ErrorMessage);
    }
    //Xuất danh sách người chơi theo điểm giảm dần YC2
    public void playerListYC2()
    {
        //Ứng dụng của Linq dựa trên điểm người chs xắp xếp và có hiển thị Rank
        players = players.OrderByDescending(x => x.Score).ToList();
        UpdatePlayerList();
    }
    //Yêu cầu 3
    public void YC3()
    {
        if (gameController != null)
        {
            int currentPlayerScore = gameController.Score;
            if (currentPlayerScore <= 0)
            {
                Debug.Log("Không thể lấy điểm của người chơi hiện tại");
                return;
            }
            //linq tìm ng chơi thấp điểm hơn ng chơi hiện tại
            List<PlayerInfo> playerList = players.Where(x => x.Score < currentPlayerScore).ToList();
            UpdatePlayerList(playerList);

            playerIDText.text = "";
            playerNameText.text = "";
            playerScoreText.text = "";
            playerRegionText.text = "";
            playerRankText.text = "";
            foreach (var player in playerList)
            {
                playerIDText.text += player.ID + "\n\n";
                playerNameText.text += player.Name + "\n\n";
                playerScoreText.text += player.Score + "\n\n";
                playerRegionText.text += player.Region + "\n\n";
            }
        }
    }

    //Yêu cầu 4
    public void SreachID()
    {
        if (sreachID != null)
        {
            int playerID = int.Parse(sreachID.text);

            YC4(playerID);
        }
    }
    private void YC4(int currentPlayerID)
    {
        //Ứng dụng của first , có FirstOfDefault
        PlayerInfo currentPlayer = players.FirstOrDefault(x => x.ID == currentPlayerID);

        playerIDText.text = "";
        playerNameText.text = "";
        playerScoreText.text = "";
        playerRegionText.text = "";
        playerRankText.text = "";
        if (currentPlayer.ID != 0)
        {
            playerIDText.text += currentPlayer.ID + "\n\n";
            playerNameText.text += currentPlayer.Name + "\n\n";
            playerScoreText.text += currentPlayer.Score + "\n\n";
            playerRegionText.text += currentPlayer.Region + "\n\n";
        }
        else
        {
            Debug.Log("Không tìm thấy người chơi có ID: " + currentPlayerID);
        }
    }
    //Yêu cầu 5
    public void diemGiamDan(List<PlayerInfo> listPlayer)
    {
        //Ứng dụng của linq
        listPlayer = listPlayer.OrderByDescending(x => x.Score).ToList();

        playerIDText.text = "";
        playerNameText.text = "";
        playerScoreText.text = "";
        playerRegionText.text = "";
        playerRankText.text = "";
        foreach (var player in listPlayer)
        {
            playerIDText.text += player.ID + "\n\n";
            playerNameText.text += player.Name + "\n\n";
            playerScoreText.text += player.Score + "\n\n";
            playerRegionText.text += player.Region + "\n\n";
        }
    }
    public void YC5()
    {
        diemGiamDan(players);
    }
    //Yêu cầu 6
    public void YC6(List<PlayerInfo> listPlayer)
    {
        listPlayer = listPlayer.OrderBy(x => x.Score).ToList();
        //Ứng dụng của linq tìm 5 người chơi điểm thấp nhất tăng dần
        List<PlayerInfo> lowestScorePlayers = listPlayer.Take(5).ToList();

        playerIDText.text = "";
        playerNameText.text = "";
        playerScoreText.text = "";
        playerRegionText.text = "";
        playerRankText.text = "";
        foreach (var player in lowestScorePlayers)
        {
            playerIDText.text += player.ID + "\n\n";
            playerNameText.text += player.Name + "\n\n";
            playerScoreText.text += player.Score + "\n\n";
            playerRegionText.text += player.Region + "\n\n";
        }
    }
    public void YC6()
    {
        YC6(players);
    }
    //Lấy Rank ng chơi cho YC2
    private string GetRank(int score)
    {
        if (score < 100)
        {
            return "Hạng đồng";
        }
        else if (score >= 100 && score < 500)
        {
            return "Bạc";
        }
        else if (score >= 500 && score < 1000)
        {
            return "Vàng";
        }
        else
        {
            return "Kim cương";
        }
    }
    public void BackToLogin()
    {
        SceneManager.LoadScene("Login");
    }
}
