using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
public class GameController : MonoBehaviour
{
    public float timePerQuestion;
    float m_curTime;
    int m_rightCount;
    int m_score;
    [Header("Điểm")]
    public TMP_Text scoreText;
    public TMP_Text score;
    public int Score { get { return m_score; } }
    private void Awake()
    {
        m_curTime = timePerQuestion;
    }

    void Start()
    {
        m_score = 0;
        UpdatePlayerHightScoreOnPLayFab(m_score);
        UIManager.Ins.SetTimeText("00 : " + m_curTime);
        CreateQuestion();
        StartCoroutine(TimeCountingDown());
    }

    public void CreateQuestion()
    {
        QuestionData qs = QuestionManager.Ins.GetRandomQuestion();

        if (qs != null)
        {
            UIManager.Ins.SetQuestionText(qs.question);
            string[] wrongAnswers = new string[] { qs.answerA, qs.answerB, qs.answerC };

            ClearAnswerColor();
            UIManager.Ins.ShuffleAnswers();
            var temp = UIManager.Ins.answerButtons;
            if (temp != null && temp.Length > 0)
            {
                int wrongAnswerCount = 0;
                for (int i = 0; i < temp.Length; i++)
                {
                    int answerID = i;
                    if (string.Compare(temp[i].tag, "RightAnswer") == 0)
                    {
                        temp[i].SetAnswerText(qs.rightAnswer);
                    }
                    else
                    {
                        temp[i].SetAnswerText(wrongAnswers[wrongAnswerCount]);
                        wrongAnswerCount++;
                    }
                    temp[answerID].btnComp.onClick.RemoveAllListeners();
                    temp[answerID].btnComp.onClick.AddListener(() => CheckRightAnswerEvent(temp[answerID]));
                }
            }
        }
    }

    void CheckRightAnswerEvent(AnswerButton answerButton)
    {
        if (answerButton.CompareTag("RightAnswer"))
        {
            m_curTime = timePerQuestion;
            UIManager.Ins.SetTimeText("00: " + m_curTime);
            UpdateScoreUI();
            UpdatePlayerHightScore(1, m_score);
            m_rightCount++;
            m_score += 100;

            if (m_rightCount == QuestionManager.Ins.questions.Length)
            {
                if(m_score > 1000)
                {
                    UIManager.Ins.dialog.SetDialogContent("Bạn đã chiến thắng!");
                    UIManager.Ins.dialog.Show(true);
                    AudioController.Ins.PlayWinSound();
                    StopAllCoroutines();
                }
                else
                {
                    StartCoroutine(showEndGame());
                }
            }
            else
            {
                StartCoroutine(showCorrectNextQuestion());
                AudioController.Ins.PlayRightSound();
                Debug.Log("Bạn đã trả lời đúng.");
                UpdateScoreUI();
            }
            UpdatePlayerHightScoreOnPLayFab(m_score);
        }
        else
        {
            StartCoroutine(showCorrectAnswerDialog());
            Debug.Log("Bạn đã trả lời sai");
        }     
    }
    IEnumerator showCorrectNextQuestion()
    {
        HightlightRightAnswer(true);
        yield return new WaitForSeconds(5);
        m_curTime = timePerQuestion;
        UIManager.Ins.SetTimeText("00: " + m_curTime);
        CreateQuestion();
        AudioController.Ins.PlayRightSound();
    }
    IEnumerator showCorrectAnswerDialog()
    {
        HightlightRightAnswer(false);
        yield return new WaitForSeconds(3);
        UIManager.Ins.dialog.SetDialogContent("Bạn đã trả lời sai!");
        UIManager.Ins.dialog.Show(true);
    }
    void HightlightRightAnswer(bool isCorrect)
    {
        Color highlightColor = isCorrect? Color.green : Color.red;
        var temp = UIManager.Ins.answerButtons;
        if(temp != null && temp.Length > 0)
        {
            foreach (var button in temp)
            {
                if (button.CompareTag("RightAnswer"))
                {
                    button.GetComponent<Image>().color = highlightColor;
                }
            }
        }
    }
    public void ClearAnswerColor()
    {
        var temp = UIManager.Ins.answerButtons;
        if(temp != null && temp.Length > 0)
        {
            foreach(var button in temp)
            {
                button.GetComponent<Image>().color = Color.white;
            }
        }
    }
    IEnumerator showEndGame()
    {
        yield return new WaitForSeconds(5);
        UIManager.Ins.dialog.SetDialogContent("Bạn đã chiến thắng");
        AudioController.Ins.PlayWinSound();
        StopAllCoroutines();
    }
    IEnumerator TimeCountingDown()
    {
        yield return new WaitForSeconds(1);
        if (m_curTime > 0)
        {
            m_curTime--;
            UIManager.Ins.SetTimeText("00 : " + m_curTime);
        }
        else
        {
            UIManager.Ins.dialog.SetDialogContent("Đã hết thời gian!");
            UIManager.Ins.dialog.Show(true);
            StopAllCoroutines();
        }
        StartCoroutine(TimeCountingDown());
    }
    public void Replay()
    {
        AudioController.Ins.StopMusic();
        SceneManager.LoadScene("Game");
    } 
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
    private void UpdateScoreUI()
    {
        if (m_score < 100)
        {
            score.text = "Hạng đồng";
        }
        else if (m_score >= 100 && m_score < 500)
        {
            score.text = "Bạc";
        }
        else if (m_score >= 500 && m_score < 1000)
        {
            score.text = "Vàng";
        }
        else
        {
            score.text = "Kim cương";
        }
        scoreText.text = "Score: " + m_score.ToString();
    }

    public void UpdatePlayerHightScore(int playerID, int newScore)
    {
        int currentHighScore = PlayerPrefs.GetInt("PlayerHighScore_" + playerID, 0);

        if(newScore > currentHighScore)
        {
            PlayerPrefs.SetInt("PlayerHighScore_" +playerID, newScore);
            PlayerPrefs.Save();
        }       
    }
    //cập nhật điểm cao lên playerfab
    public void UpdatePlayerHightScoreOnPLayFab(int newScore)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = newScore
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateStatisticSucces, OnError);
    }
    private void OnUpdateStatisticSucces(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Cập nhật điểm thành công");
    }
    private void OnError(PlayFabError error)
    {
        Debug.Log("Lỗi khi cập nhật điểm " +error.ErrorMessage);
    }
}