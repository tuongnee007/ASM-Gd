using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class doiMau : MonoBehaviour
{
    public float blinkInterval = 2.0f; // Thay đổi tần số nhấp nháy
    private Text textComponent;
    private Color originalColor; // Màu gốc của chữ
    public Color blinkColor = Color.red; // Màu sẽ đổi khi nhấp nháy

    void Start()
    {
        textComponent = GetComponent<Text>();
        originalColor = textComponent.color; // Lưu màu gốc của chữ

        StartCoroutine(BlinkText());
    }

    IEnumerator BlinkText()
    {
        while (true)
        {
            // Đổi màu của chữ
            textComponent.color = (textComponent.color == originalColor) ? blinkColor : originalColor;

            // Đợi một khoảng thời gian trước khi thực hiện lại
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
