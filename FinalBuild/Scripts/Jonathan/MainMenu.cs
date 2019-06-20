using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* 
 * Full Name: Jonathan Burns
 * Student ID: 2288851
 * Chapman email: jburns@chapman.edu/
 * Course number and section: 236-02
 * Assignment Number: 5
 */

/*
 * Purpose:
 *  - MainMenu: loads the starting scene, and sets difficulty 
 */


public class MainMenu : MonoBehaviour
{
    public Toggle loadFourth;

    // https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html

    /*
     * StartNormalScene - trigger the starting level to load,
     * Creates Difficulty object with normal settings
     */
    public void StartNormalScene()
    {
        CreateDifficultyPersister(600, 150, "normal");
        FirstLevel();
    }

    /*
     * StartHardScene - trigger the starting level to load,
     * Creates Difficulty object with hard settings
     */
    public void StartHardScene()
    {
        CreateDifficultyPersister(400, 200, "hard");
        FirstLevel();
    }

    /*
     * StartFreeScene - trigger the starting level to load,
     * Creates Difficulty object with nearly unlimited settings
     */
    public void StartFreeScene()
    {
        CreateDifficultyPersister(60000, 0, "free");
        FirstLevel();
    }

    /*
     * CreateDifficultyPersister - creates an DifficultyPersister object to store difficulty between scenes,
     * Note it isn't destroyed on next scene 
     * Params:
     *  - int maxFrames: the stack size for the state controller
     *  - int framePenalty: how many frames are removed with each death
     *  - string modeName: the identifier of the difficulty, should only be 'normal', 'hard', or 'free'
     */
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

    /*
     * FirstLevel - loads into the game
     * Loads the fourth level instead of first if toggled in UI
     */
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
