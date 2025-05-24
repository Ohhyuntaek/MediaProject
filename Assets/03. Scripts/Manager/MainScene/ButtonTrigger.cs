using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTrigger : MonoBehaviour
{
    private Dictionary<TMP_Text, Vector3> buttonTextOriginalPositions = new();
    private Dictionary<Image, Vector3> buttonImageOriginalPositions = new();
    
    [SerializeField]
    private AudioClip buttonClickSound;
    
    // 텍스트만 이동 (눌렀을 때)
    public void OnTextButtonPress(TMP_Text text)
    {
        EntireGameManager.Instance.soundManager.PlaySfx(buttonClickSound,transform.position,false);
        
        if (text == null) return;

        if (!buttonTextOriginalPositions.ContainsKey(text))
            buttonTextOriginalPositions.Add(text, text.rectTransform.anchoredPosition);

        Vector3 pressedPosition = buttonTextOriginalPositions[text] + new Vector3(0, -10f, 0);
        text.rectTransform.anchoredPosition = pressedPosition;
    }

    // 텍스트만 복귀 (뗄 때)
    public void OnTextButtonRelease(TMP_Text text)
    {
        if (text == null) return;

        if (buttonTextOriginalPositions.ContainsKey(text))
        {
            text.rectTransform.anchoredPosition = buttonTextOriginalPositions[text];
        }
    }

    // 이미지만 이동 (눌렀을 때)
    public void OnImageButtonPress(Image image)
    {
        EntireGameManager.Instance.soundManager.PlaySfx(buttonClickSound,transform.position,false);
        
        if (image == null) return;

        if (!buttonImageOriginalPositions.ContainsKey(image))
            buttonImageOriginalPositions.Add(image, image.rectTransform.anchoredPosition);

        Vector3 pressedPosition = buttonImageOriginalPositions[image] + new Vector3(0, -10f, 0);
        image.rectTransform.anchoredPosition = pressedPosition;
    }

    // 이미지만 복귀 (뗄 때)
    public void OnImageButtonRelease(Image image)
    {
        if (image == null) return;

        if (buttonImageOriginalPositions.ContainsKey(image))
        {
            image.rectTransform.anchoredPosition = buttonImageOriginalPositions[image];
        }
    }
}