using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitAttackTextUI : MonoBehaviour
{
    [SerializeField]
    private float textLifetime;
    private float lifeTimer;

    [SerializeField]
    private TextMeshProUGUI attackText;
    private string state;
    private Color textColour;

    private void Start()
    {
        lifeTimer = textLifetime;
        //attackText.color = new Color(255f, 255f, 0f, 1f);
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;

        if ((lifeTimer <= (textLifetime / 2)) && (state != null))
        {
            attackText.text = state;
        }

        if (lifeTimer < 1f)
        {
            attackText.color = new Color(textColour.r, textColour.g, textColour.b, lifeTimer / 1f);
            if (lifeTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetupText(string text, Color textColour)
    {
        attackText.text = text;
        this.textColour = textColour;
        attackText.color = textColour;
    }

    public void SetupText(string text, Color textColour, string state)
    {
        attackText.text = text;
        this.textColour = textColour;
        attackText.color = textColour;
        this.state = state;
    }
}
