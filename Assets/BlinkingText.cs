using System.Collections;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    [SerializeField] private float blinkInterval = 0.5f;
    
    private bool isVisible = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            isVisible = !isVisible;
            
            // 알파값 조절
            Color color = pressAnyKeyText.color;
            color.a = isVisible ? 1f : 0.2f;
            pressAnyKeyText.color = color;
            
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
