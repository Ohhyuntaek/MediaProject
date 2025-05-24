using System;
using UnityEngine;

public class EntireGameManager : MonoBehaviour
{
    public static EntireGameManager Instance;
    
    [Header("소리 설정")]
    public SoundManager soundManager;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
