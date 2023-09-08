using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public E_Scenes demoScene;

    public void OpenDemoLevel()
    {
        SceneManager.LoadScene(demoScene.ToString());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum E_Scenes
{
    SplashScreen, DemoLevel 
}