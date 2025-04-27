using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    private DawnData selectedDawnData;

    // 선택된 플레이어 데이터 (MainScene에서 설정 -> InStageScene에서 사용)
    public DawnData SelectedDawnData { get; private set; }

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
    public void SetSelectedDawn(DawnData playerData)
    {
        SelectedDawnData = playerData;
    }
    
    public DawnData GetSelectedDawn()
    {
        return selectedDawnData;
    }
}
