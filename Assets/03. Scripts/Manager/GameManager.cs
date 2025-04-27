using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private DawnData selectedDawn;

    void Awake()
    {
        // 싱글톤 패턴
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
    public void SetSelectedDawn(DawnData dawn)
    {
        selectedDawn = dawn;
    }
    
    public DawnData GetSelectedDawn()
    {
        return selectedDawn;
    }
}
