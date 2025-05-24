using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("뒤로가기 버튼")]
    [SerializeField] private Button backButton;
    
    [Header("보유 루멘")]
    [SerializeField] private TMP_Text lumenText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lumenText.text = RuntimeDataManager.Instance.lumenCalculator.Lumen.ToString();
    }

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("MapScene");
    }
}
