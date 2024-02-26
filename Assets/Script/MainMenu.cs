using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    
    Animator anim;    

    // Start is called before the first frame update
    void Start()
    {
       //gameStart.SetActive(false);
        anim = GetComponent<Animator>();
    }



   

    public void OnClickNewGame()
    {
        Debug.Log("게임시작 ");
        //gameStart = EventSystem.current.currentSelectedGameObject;
        //EventSystem.current.firstSelectedGameObject = this.gameStart;
        //gameStart.SetActive(true);
        //anim.Play("Front");
        anim.Play("Back");
    }

    public void OnClickBack()
    {
        Debug.Log("뒤로 가기 ");
        anim.Play("Front");
        //gameStart.SetActive(false);
    }

    public void OnClickMainGame()
    {
        LoadingScene.LoadScene("Main_Scene");
        
    }

    public void OnClickReloadGame()
    {
        Time.timeScale = 1;
        LoadingScene.LoadScene("Main_Scene");

    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
