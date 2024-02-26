using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{


    //public GameObject gameStart;
    static string nextScene;

    [SerializeField]
    private Image loadingBar;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
        
    }


    IEnumerator LoadSceneProcess()
    {
        //LoadSceneAsync 함수는 비동기 방식으로 씬을 불러오는 도중에 다른 작업이 가능함
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false; //씬을 비동기로 불러들일 때, 씬의 로딩이 끝나면 자동으로 불러온 씬으로 이동할 것인지를 설정

        float curTime = 0f;
        //씬 로딩이 끝나지 않은 상태라면 계속해서 반복 
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                loadingBar.fillAmount = op.progress;
            }
            else
            {
                curTime += Time.unscaledDeltaTime;
                loadingBar.fillAmount = Mathf.Lerp(0.9f, 1f, curTime);
                if (loadingBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }

    }
    
}
