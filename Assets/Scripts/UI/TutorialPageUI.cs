using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPageUI : MonoBehaviour
{
    [SerializeField]
    private GameObject tutorialPage;

    public void ShowTutorial(bool toggle)
    {
        tutorialPage.SetActive(toggle);
    }
}
