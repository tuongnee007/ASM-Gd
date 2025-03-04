using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour
{
    public TMP_Text answerText;
    public Button btnComp;
    public void SetAnswerText(string content)
    {
        if (answerText)
        {
            answerText.text = content;
        }
    }
}
