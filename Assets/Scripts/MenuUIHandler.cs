using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField NameEntered;

    // Start is called before the first frame update
    private void Start()
    {

    }

    public void StartNew()
    {
        // load the main scene
        SceneManager.LoadScene(1); // defined in index in build settings window
    }

    public void Exit()
    {

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void StartMenu()
    {
        // load the start menu
        SceneManager.LoadScene(0);
    }

    public void NameEnteredInGUI()
    {
        Debug.Log("A name was entered!");
        MainManager.m_NameEntered = NameEntered.text;
    }
}
