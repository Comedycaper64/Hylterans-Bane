using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Conversation", fileName = "NewConversation")]
public class Conversation : ScriptableObject
{
    //A conversation is a list of conversation nodes. A conversation node can be a Dialogue, DialogueAddSkill, or DialogueChangeScene
    public ConversationNode[] conversationNodes;
}
