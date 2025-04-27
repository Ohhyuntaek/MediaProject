using UnityEngine;

public class DawnSelector : MonoBehaviour
{
    [SerializeField] private DawnData dawnData;
    [SerializeField] private GameObject highlightFrame; // 선택 테두리

    public void OnClick()
    {
        MainSceneManager.Instance.OnSelectDawn(dawnData, this);
    }

    public void SetHighlight(bool active)
    {
        if (highlightFrame != null)
            highlightFrame.SetActive(active);
    }
}