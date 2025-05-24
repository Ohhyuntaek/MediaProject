using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;

    [SerializeField] 
    private Image progressCircle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator LoadScene()
    {
        yield return null;
        
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            
            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                progressCircle.fillAmount = Mathf.Lerp(progressCircle.fillAmount, op.progress, timer);

                if (progressCircle.fillAmount >= op.progress)
                {
                    timer = 0.0f;
                }
            }
            else
            {
                progressCircle.fillAmount = Mathf.Lerp(progressCircle.fillAmount, 1f, timer);

                if (progressCircle.fillAmount >= 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
