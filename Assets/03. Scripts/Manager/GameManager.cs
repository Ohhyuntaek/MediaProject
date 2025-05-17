using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("선택한 Dawn 캐릭터")]
    [SerializeField] private Dawn selectedDawn;
    
    [Header("현재 스테이지")]
    [SerializeField] public StageData currentStageData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 안 없어지게
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 플레이어 선택할 때 호출
    public void SetSelectedDawn(Dawn dawn)
    {
        selectedDawn = dawn;
    }
    
    public Dawn GetSelectedDawn()
    {
        return selectedDawn;
    }
}
