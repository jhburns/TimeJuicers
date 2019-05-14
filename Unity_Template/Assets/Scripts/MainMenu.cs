using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Toggle loadFourth;

    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html
    public void StartNormalScene()
    {
        CreateDifficultyPersister(600, 150, "normal");
        FirstLevel();
    }

    public void StartHardScene()
    {
        CreateDifficultyPersister(400, 200, "hard");
        FirstLevel();
    }

    public void StartFreeScene()
    {
        CreateDifficultyPersister(60000, 0, "free");
        FirstLevel();
    }

    private void CreateDifficultyPersister(int maxFrames, int framePenalty, string modeName)
    {
        // https://answers.unity.com/questions/572852/how-do-you-create-an-empty-gameobject-in-code-and.html
        GameObject diffPersister = new GameObject("DifficultyManager");
        diffPersister.AddComponent<DifficultyPersister>();

        diffPersister.GetComponent<DifficultyPersister>().MaxFrames = maxFrames;
        diffPersister.GetComponent<DifficultyPersister>().FramePenalty = framePenalty;
        diffPersister.GetComponent<DifficultyPersister>().modeName = modeName;

        GameObject.DontDestroyOnLoad(diffPersister);
    }

    private void FirstLevel()
    {
        if (loadFourth.isOn)
        {
            SceneManager.LoadScene("4Guess");
        } else
        {
            SceneManager.LoadScene("1Tutorial");
        }
    }
}
