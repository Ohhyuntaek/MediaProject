using UnityEngine;
using UnityEngine.Serialization;

public class DawnSelector : MonoBehaviour
{
    [SerializeField] private Dawn dawn;
    [SerializeField] private GameObject highlightFrame; // 선택 테두리

    public void OnClick()
    {
        MainSceneManager.Instance.OnSelectDawn(dawn, this);
    }

    public void SetHighlight(bool active)
    {
        if (highlightFrame != null)
            highlightFrame.SetActive(active);
    }
}