using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public TMP_Text dialogContentText;
    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void ShowDiaLog(string title, string content)
    {
        dialogContentText.text = title;
        SetDialogContent(content);
        Show(true);
    }
    public void SetDialogContent(string content)
    {
        if(dialogContentText)      
            dialogContentText.text = content;     
    }

    public void UpdateDialogContent(string content)
    {
        SetDialogContent(content);
    }
}
