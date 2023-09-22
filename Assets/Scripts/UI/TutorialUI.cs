using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    private List<TutorialPageUI> tutorialPageUIs = new List<TutorialPageUI>();
    private TutorialPageUI currentTutorialPage;

    private void Start()
    {
        tutorialPageUIs = GetComponentsInChildren<TutorialPageUI>().ToList();
        currentTutorialPage = tutorialPageUIs[0];
    }

    public void OpenTutorial()
    {
        currentTutorialPage.ShowTutorial(true);
    }

    public void CloseTutorial()
    {
        currentTutorialPage.ShowTutorial(false);
    }

    public void OpenNextPage()
    {
        CloseTutorial();
        currentTutorialPage = tutorialPageUIs[
            (tutorialPageUIs.IndexOf(currentTutorialPage) + 1) % tutorialPageUIs.Count
        ];
        OpenTutorial();
    }

    public void OpenPreviousPage()
    {
        CloseTutorial();
        if (tutorialPageUIs.IndexOf(currentTutorialPage) == 0)
        {
            currentTutorialPage = tutorialPageUIs[tutorialPageUIs.Count - 1];
        }
        else
        {
            currentTutorialPage = tutorialPageUIs[
                (tutorialPageUIs.IndexOf(currentTutorialPage) - 1) % tutorialPageUIs.Count
            ];
        }
        OpenTutorial();
    }
}
