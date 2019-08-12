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
     * And calls LoadLevel to start the first level
     */
    public void StartScene(DifficultyPersister settings)
    {
        GameObject diffPersister = new GameObject("DifficultyManager");
        diffPersister.AddComponent<DifficultyPersister>();
        DifficultyPersister diffComponent = diffPersister.GetComponent<DifficultyPersister>();

        diffComponent.MaxFrames = settings.MaxFrames;
        diffComponent.FramePenalty = settings.FramePenalty;
        diffComponent.name = settings.name;

        GameObject.DontDestroyOnLoad(diffPersister);

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
