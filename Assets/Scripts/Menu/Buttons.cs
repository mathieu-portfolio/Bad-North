using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    public void PlayMenu()
    {
        SceneManager.LoadScene("PlayMenu");
    }

    public void Settings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        int i = 1;
        SceneManager.LoadScene("Level" + i);
    }
}
