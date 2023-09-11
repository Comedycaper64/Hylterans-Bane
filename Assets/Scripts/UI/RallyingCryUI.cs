using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyingCryUI : MonoBehaviour
{
    [SerializeField]
    private Transform rallyButtonPrefab;

    [SerializeField]
    private Transform rallyButtonContainerTransform;

    private void Awake()
    {
        ToggleUI(false);
        ClearRallyUIs();
    }

    //Rallying Cry UI makes vertical stack of rallying cries, greying out used one, highlighting ones where the unit has enough held actions

    private void Start()
    {
        RallyingCryButtonUI.OnChooseRallyingCry += RallyingCryButtonUI_OnChooseRallyingCry;
        InputManager.Instance.OnRallyingCryEvent += InputManager_OnRallyingCry;
    }

    private void OnDisable()
    {
        RallyingCryButtonUI.OnChooseRallyingCry -= RallyingCryButtonUI_OnChooseRallyingCry;
        InputManager.Instance.OnRallyingCryEvent -= InputManager_OnRallyingCry;
    }

    private void ToggleUI(bool toggle)
    {
        rallyButtonContainerTransform.gameObject.SetActive(toggle);
    }

    private void InputManager_OnRallyingCry()
    {
        if (rallyButtonContainerTransform.gameObject.activeInHierarchy)
        {
            ToggleUI(false);
        }
        else
        {
            if (rallyButtonContainerTransform.childCount == 0)
            {
                List<Unit> unitList = UnitManager.Instance.GetFriendlyUnitList();
                foreach (Unit unit in unitList)
                {
                    RallyingCry rallyingCry;
                    if (!unit.TryGetComponent<RallyingCry>(out rallyingCry))
                    {
                        continue;
                    }
                    RallyingCryButtonUI cryButton = Instantiate(
                            rallyButtonPrefab,
                            rallyButtonContainerTransform
                        )
                        .GetComponent<RallyingCryButtonUI>();
                    cryButton.Setup(rallyingCry);
                }
            }
            else
            {
                RallyingCryButtonUI[] cryButtons =
                    rallyButtonContainerTransform.GetComponentsInChildren<RallyingCryButtonUI>();
                foreach (RallyingCryButtonUI buttonUI in cryButtons)
                {
                    buttonUI.UpdateButton();
                }
            }
            ToggleUI(true);
        }
    }

    private void ClearRallyUIs()
    {
        foreach (Transform buttonTransform in rallyButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }
    }

    private void RallyingCryButtonUI_OnChooseRallyingCry(object sender, RallyingCry e)
    {
        ToggleUI(false);
    }
}
