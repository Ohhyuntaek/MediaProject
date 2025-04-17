using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AutoFitByHeight : MonoBehaviour
{
    public float targetHeight = 900f; // 고정하고 싶은 세로 크기(px)

    private Image image;
    private Sprite lastSprite;

    void Start()
    {
        image = GetComponent<Image>();
        lastSprite = image.sprite;

        FitWidthToHeight();
    }

    void LateUpdate()
    {
        if (image.sprite != lastSprite)
        {
            lastSprite = image.sprite;
            FitWidthToHeight();
        }
    }

    void FitWidthToHeight()
    {
        if (image.sprite == null) return;

        float spriteWidth = image.sprite.rect.width;
        float spriteHeight = image.sprite.rect.height;

        float ratio = spriteWidth / spriteHeight;
        float newWidth = targetHeight * ratio;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newWidth, targetHeight);
    }
}