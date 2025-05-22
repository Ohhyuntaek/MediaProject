using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneController : MonoBehaviour
{
    private bool isKeyPressed = false;

    private void Start()
    {
        SoundManager.Instance.PlayBgmList(0,true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isKeyPressed && Input.anyKeyDown)
        {
            isKeyPressed = true;
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
