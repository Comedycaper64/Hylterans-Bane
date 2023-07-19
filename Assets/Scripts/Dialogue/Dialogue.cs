using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A dialogue contains information for the character speaking
[CreateAssetMenu(menuName = "Dialogue", fileName = "NewDialogue")]
public class Dialogue : ConversationNode
{
	public string characterName;
	//A list of sprites allows for a different image to be displayed alongside each sentence
	public Sprite[] characterImages;

	//The sound made when they speak
	public AudioClip characterTalkSound;

	//Whether the character image should appear on the right or left of the dialogueUI
	public bool leftSideCharacterImage;
	public bool rightSideCharacterImage;

	//The dialogue that they speak
	[TextArea(3, 10)]
	public string[] sentences;
}
