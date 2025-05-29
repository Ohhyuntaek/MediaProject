using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] public InGameUIManager inGameUIManager;
    [SerializeField] public ClearUIManager clearUIManager;
    [SerializeField] public CostManager costManager;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject soundPanel;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }
        
    }

    private void Update()
    {
        if (InGameSceneManager.Instance.isGameOver)
        {
            clearUIManager.SetActiveStageClearText(true, "GameOver");
            nextButton.gameObject.SetActive(true);
        }
    }

    public void OnNextButtonClick()
    {
        LoadingSceneManager.LoadScene("MainScene");
    }

    public void OnPanelButtonClick()
    {
        
        soundPanel.SetActive(true);
        Time.timeScale = 0f;
        EntireGameManager.Instance.soundManager.ButtonClick(soundPanel);
    }

    public void OnBackButtonClick()
    {
        soundPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnExitButtonClick()
    {
        Time.timeScale = 1f;
        RuntimeDataManager.Instance.InitMapState();
        RuntimeDataManager.Instance.enhancement.InitialEnhanceValue();
        LoadingSceneManager.LoadScene("MainScene");
    }
}
