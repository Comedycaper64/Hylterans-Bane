using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField]
    private GameObject levelLostUI;

    [SerializeField]
    private GameObject levelBeatUI;

    [SerializeField]
    private TextMeshProUGUI turnsTakenText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one LevelManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UnitManager.Instance.OnEnemyDied += UnitManager_OnEnemyDied;
        UnitManager.Instance.OnFriendlyUnitDied += UnitManager_OnFriendlyUnitDied;
    }

    private void OnDisable()
    {
        UnitManager.Instance.OnEnemyDied -= UnitManager_OnEnemyDied;
        UnitManager.Instance.OnFriendlyUnitDied -= UnitManager_OnFriendlyUnitDied;
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNextLevel()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex + 1);
    }

    public void OpenNextLevelUI()
    {
        if (levelBeatUI && turnsTakenText)
        {
            levelBeatUI.SetActive(true);
            turnsTakenText.text = "Turns Taken: " + TurnSystem.Instance.GetTurnNumber();
        }
    }

    private void UnitManager_OnEnemyDied(object sender, EventArgs e)
    {
        if (UnitManager.Instance.GetEnemyUnitList().Count == 0)
        {
            OpenNextLevelUI();
        }
    }

    private void UnitManager_OnFriendlyUnitDied(object sender, EventArgs e)
    {
        if (UnitManager.Instance.GetFriendlyUnitList().Count == 0)
        {
            levelLostUI.SetActive(true);
        }
    }
}
