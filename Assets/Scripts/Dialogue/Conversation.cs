using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable objects are able to be created in Unity's asset hierarchy.
//This is useful for creating multiple objects based on a certain script
//In this instance, it allows creating multiple conversations and storing them in the Dialogue folder of the Asset hierarchy
[CreateAssetMenu(menuName = "Conversation", fileName = "NewConversation")]
public class Conversation : ScriptableObject
{
   //A conversation is a list of conversation nodes. A conversation node can be a Dialogue, DialogueAddSkill, or DialogueChangeScene
   public ConversationNode[] conversationNodes;
}
