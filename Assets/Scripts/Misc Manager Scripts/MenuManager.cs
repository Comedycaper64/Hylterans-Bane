using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject[] tutorialScreens;

    private GameObject currentOpenScreen;

    private void Awake() 
    {
        mainMenuScreen.SetActive(true);
        currentOpenScreen = mainMenuScreen;
        levelSelectScreen.SetActive(false);
        optionsScreen.SetActive(false);
        foreach(GameObject tutorialScreen in tutorialScreens)
        {
            tutorialScreen.SetActive(false);
        }    
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

    public void OpenScreen(GameObject screen)
    {
        currentOpenScreen.SetActive(false);
        screen.SetActive(true);
        currentOpenScreen = screen;
    }

}
