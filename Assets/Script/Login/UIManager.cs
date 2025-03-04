using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Ins;
    public TMP_Text timeText;
    public TMP_Text questionText;
    public Dialog dialog;
    public AnswerButton[] answerButtons;
    private void Awake()
    {
        MakeSingleToon();
    }

    public void SetTimeText(string content)
    {
        if(timeText)
            timeText.text = content;
    }

    public void SetQuestionText(string content)
    {
        if(questionText)
            questionText.text = content;
    }

    public void ShuffleAnswers()
    {
        if(answerButtons != null && answerButtons.Length > 0)
        {
            for(int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i])
                {
                    answerButtons[i].tag = "Untagged";
                }
            }

            int randIdx = Random.Range(0, answerButtons.Length);

            if (answerButtons[randIdx])
            {
                answerButtons[randIdx].tag = "RightAnswer";
            }
        }
    }
    public void MakeSingleToon()
    {
        if(Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ConrrectAnswerSelected()
    {
        string conrrectAnswer = GetCorrectAnswer();
        dialog.UpdateDialogContent(conrrectAnswer);
    }

    private string GetCorrectAnswer()
    {
        return "This is the correct answer";
    }
    public void DisplayNewQuestion()
    {
        string newQuestion = "What";
        SetQuestionText(newQuestion);
        dialog.SetDialogContent("");
    }
}
