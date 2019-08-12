using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public string levelName;

    /*
     * StartScene - creates DifficultyManager thats saved between levels,
     * And calls LoadLevel to start the begin level
     */
    public void StartScene(DifficultyPersister settings)
    {
        GameObject.DontDestroyOnLoad(settings);

        LoadLevel();
    }

    /*
     * LoadLevel - loads into levelName scene
     */
    private void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }
}
