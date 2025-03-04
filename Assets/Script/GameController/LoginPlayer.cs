using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayerData
{
    public string playerName;
    public int playerPasswordLength;
    public string playFabId;
}
public class LoginPlayer : MonoBehaviour
{
    [Header("Login")]
    public TMP_InputField nameInputField;
    public TMP_InputField passwordInputField;
    public GameObject loginPanel;
    public GameObject newLoginPanel;
    public TMP_Text messageText;

    [Header("Regiest")]
    public TMP_InputField usernameInputFieldRegiest;
    public TMP_InputField passwordInputFieldRegiest;

    public TMP_Text leaderboardText;
    //PlayerFab
    private const string TitleId = "83147";
    private List<PlayerData> players = new List<PlayerData>();

    void Start()
    {
        LoadPlayerData();
        PlayFabSettings.TitleId = TitleId;
    }
    private void ShowMessageText(string message)
    {
        messageText.text = message;
    }
    public void Level1()
    {
        if (string.IsNullOrEmpty(nameInputField.text) || string.IsNullOrEmpty(passwordInputField.text))
        {
            ShowMessageText("Vui lòng nhập tên và mật khẩu");
            return;
        }

        foreach (PlayerData playerData in players)
        {
            if (playerData.playerName == nameInputField.text && playerData.playerPasswordLength == passwordInputField.text.Length)
            {
                ShowMessageText("Đăng nhập thành công!");
                SceneManager.LoadScene("Game");
                return;
            }
        }
        ShowMessageText("Tên người dùng hoặc mật khẩu không đúng");
    }
    public void OpenLoginPanel()
    {
        loginPanel.SetActive(true);
        newLoginPanel.SetActive(false);
    }

    public void OpenNewAccount()
    {
        loginPanel.SetActive(false);
        newLoginPanel.SetActive(true);
    }
    public void RegiestLoginPanel()
    {
        if (string.IsNullOrEmpty(usernameInputFieldRegiest.text) || string.IsNullOrEmpty(passwordInputFieldRegiest.text))
        {
            ShowMessageText("Vui lòng nhập tên và mật khẩu");
            return;
        }

        foreach (PlayerData playerData in players)
        {
            if (playerData.playerName == usernameInputFieldRegiest.text)
            {
                ShowMessageText("Tên người dùng đã tồn tại");
                return;
            }
        }

        PlayerData currentPlayer = new PlayerData();
        currentPlayer.playerName = usernameInputFieldRegiest.text;
        currentPlayer.playerPasswordLength = passwordInputFieldRegiest.text.Length;

        players.Add(currentPlayer);
        UpdatePlayerName(currentPlayer.playerName);
        loginPanel.SetActive(true);
        newLoginPanel.SetActive(false);

        SavePlayerData(currentPlayer);
        ShowMessageText("Đăng ký thành công và đăng nhập!");
    }
    private void UpdatePlayerName(string newName)
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);
        PlayerPrefs.SetString("PlayerName_" + playerCount, newName);
        PlayerPrefs.Save();
    }
    private void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerCount"))
        {
            int playerCount = PlayerPrefs.GetInt("PlayerCount");
            for (int i = 0; i < playerCount; i++)
            {
                PlayerData player = new PlayerData();
                player.playerName = PlayerPrefs.GetString("PlayerName_" + i);
                player.playerPasswordLength = PlayerPrefs.GetInt("PlayerPasswordLength_" + i);
                players.Add(player);
            }
        }
    }
    private void SavePlayerData(PlayerData player)
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);
        PlayerPrefs.SetInt("PlayerCount", playerCount + 1);

        PlayerPrefs.SetString("PlayerName_" + playerCount, player.playerName);
        PlayerPrefs.SetInt("PlayerPasswordLength_" + playerCount, player.playerPasswordLength);

        PlayerPrefs.Save();
    }

    //Đăng nhập với PlayFab

    //Tạo người dùng mới
    public void RegiestUser()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Username = usernameInputFieldRegiest.text,
            Password = passwordInputFieldRegiest.text,
            
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnregisterSucces,OnError);
    }
    public void OnLogin()
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = nameInputField.text,
            Password = passwordInputField.text,
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginGame, OnErrorLogin);
    }

    //Login game 
    private void OnLoginGame(LoginResult result)
    {
        PlayerData currentPlayer = new PlayerData();
        currentPlayer.playerName = nameInputField.text;
        currentPlayer.playerPasswordLength = passwordInputField.text.Length;
        players.Add(currentPlayer);
        SavePlayerData(currentPlayer);

        SceneManager.LoadScene("Game");
    }
    private void OnErrorLogin(PlayFabError error)
    {
        Debug.Log("Đăng nhập thất bại: " +error.ErrorMessage);
    }
    private void OnregisterSucces(RegisterPlayFabUserResult result)
    {
        UpdateUserName(usernameInputFieldRegiest.text);
        Debug.Log("Đăng kí người dùng mới thành công");
        loginPanel.SetActive(true);
        newLoginPanel.SetActive(false);
    }
    private void OnError(PlayFabError error)
    {  
        Debug.Log(error.ToString());  
    }

    //Cập nhật tên người dùng unity lên hệ thống
    private void UpdateUserName(string userName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUsernameSucces, OnErroeUpdateUser);
    }

    private void OnUpdateUsernameSucces(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Cập nhật tên người dùng thành công");
    }

    private void OnErroeUpdateUser(PlayFabError error)
    {
        Debug.Log("Lỗi khi cập nhật tên người dùng: " + error.ErrorMessage);
    }  
}
