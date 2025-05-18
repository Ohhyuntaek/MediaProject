using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] public InGameUIManager inGameUIManager;
    [SerializeField] public ClearUIManager clearUIManager;
    [SerializeField] public CostManager costManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
