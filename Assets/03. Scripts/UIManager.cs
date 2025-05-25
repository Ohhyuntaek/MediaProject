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
        nextButton.gameObject.SetActive(false);
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
}
