using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    private Dictionary<TMP_Text, Vector3> buttonTextOriginalPositions = new();
    
    // 버튼을 누를 때
    public void OnButtonPress(TMP_Text text)
    {
        if (text == null) return;

        if (!buttonTextOriginalPositions.ContainsKey(text))
        {
            buttonTextOriginalPositions.Add(text, text.rectTransform.anchoredPosition);
        }

        Vector3 pressedPosition = buttonTextOriginalPositions[text] + new Vector3(0, -10f, 0);
        text.rectTransform.anchoredPosition = pressedPosition;
    }

    // 버튼에서 손을 뗄 때
    public void OnButtonRelease(TMP_Text text)
    {
        if (text == null) return;

        if (buttonTextOriginalPositions.ContainsKey(text))
        {
            text.rectTransform.anchoredPosition = buttonTextOriginalPositions[text];
        }
    }

}
