using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityDetailUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI abilityNameText;

    [SerializeField]
    private TextMeshProUGUI abilityDescriptionText;

    public void SetupAbilityUI(string name, string description)
    {
        abilityNameText.text = name;
        abilityDescriptionText.text = description;
    }
}
