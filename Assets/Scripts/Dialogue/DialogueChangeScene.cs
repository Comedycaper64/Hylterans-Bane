using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used in a conversation to trigger a scene transition or a change to the music
[CreateAssetMenu(menuName = "DialogueChangeScene", fileName = "NewDialogueChangeScene")]
public class DialogueChangeScene : ConversationNode
{
    public AudioClip musicTrack;
    public int changeToScene;
}
