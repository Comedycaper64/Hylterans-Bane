using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

//Script that controls the dialogue system
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // SERIALIZABLES
    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private Image darkBackground;

    [SerializeField]
    private Image characterImageLeft;

    [SerializeField]
    private Image characterImageRight;

    [SerializeField]
    private Animator dialogueAnimator;

    [SerializeField]
    private AudioSource dialogueAudioSource;

    [SerializeField]
    private float timeBetweenLetterTyping;

    // TRACKERS
    private Conversation currentConversation;
    private Dialogue currentDialogue;
    private string currentSentence;
    public bool inConversation = false;
    private bool currentTextTyping = false;

    private Coroutine typingCoroutine;
    private Image currentCharacterImage;
    private AudioClip currentCharacterTalkSound;
    private Queue<ConversationNode> conversationNodes;
    private Queue<string> sentences;
    private Queue<Sprite> characterImages;
    private InputManager input;

    //MISC
    private float inactiveTalkerAlpha = 0.2f;
    public event Action OnConversationStart;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        sentences = new Queue<string>();
        characterImages = new Queue<Sprite>();
        conversationNodes = new Queue<ConversationNode>();
    }

    void Start()
    {
        //The manager uses player input to advance the text, so it needs the InputReader from the player
        input = InputManager.Instance;

        input.OnLeftClickEvent += OnInteract;
        currentCharacterImage = null;
    }

    private void OnDisable()
    {
        input.OnLeftClickEvent -= OnInteract;
    }

    private void OnInteract()
    {
        if (inConversation && currentDialogue != null)
        {
            //If the text is in the process of appearing, causes it to appear fully. Otherwise the next sentence begins to display
            if (!currentTextTyping)
            {
                DisplayNextSentence();
            }
            else
            {
                FinishTypingSentence();
            }
        }
    }

    public void SetNameText()
    {
        nameText.text = "";
    }

    public void SetNameText(Dialogue dialogue)
    {
        nameText.text = dialogue.characterName;
    }

    private void ClearCharacterImages()
    {
        ClearCharacterImage(characterImageLeft);
        ClearCharacterImage(characterImageRight);
    }

    private void ClearCharacterImage(Image characterImage)
    {
        characterImage.enabled = false;
    }

    public void StartConversation(Conversation conversation)
    {
        //Performs setup, as Dialogue mode is entered
        inConversation = true;
        OnConversationStart?.Invoke();
        dialogueAnimator.SetTrigger("startConversation"); //Starts Conversation in Animator, causes dialogue UI elements to appear
        darkBackground.enabled = true;
        currentConversation = conversation;
        conversationNodes.Clear();
        if (SoundManager.Instance)
        {
            dialogueAudioSource.volume = SoundManager.Instance.GetSoundEffectVolume() / 4;
        }
        ClearCharacterImages();
        //Enqueues all conversation nodes to go through them in the specified order
        foreach (ConversationNode conversationNode in currentConversation.conversationNodes)
        {
            conversationNodes.Enqueue(conversationNode);
        }
        StartDialogue(conversationNodes.Dequeue());
    }

    public void StartDialogue(ConversationNode conversationNode)
    {
        //Acts differently depending on what type of conversation node it is
        string nodeType = conversationNode.GetType().ToString();

        switch (nodeType)
        {
            case "DialogueChangeScene":
                currentDialogue = null;
                ChangeScene((DialogueChangeScene)conversationNode);
                break;
            case "DialogueTrigger":
                currentDialogue = null;
                DialogueTrigger trigger = (DialogueTrigger)conversationNode;
                trigger.BeginCombat();
                EndDialogue();
                break;
            default:
            case "Dialogue":
                //If default dialogue, then enqueues all sentences and begins displaying them
                currentDialogue = (Dialogue)conversationNode;
                sentences.Clear();
                foreach (string sentence in currentDialogue.sentences)
                {
                    sentences.Enqueue(sentence);
                }
                foreach (Sprite image in currentDialogue.characterImages)
                {
                    characterImages.Enqueue(image);
                }
                currentCharacterTalkSound = currentDialogue.characterTalkSound;
                if (currentCharacterTalkSound)
                    dialogueAudioSource.clip = currentCharacterTalkSound;

                DisplayNextSentence();
                break;
        }
    }

    private void ChangeScene(DialogueChangeScene changeScene)
    {
        if (changeScene.changeToScene != -1)
        {
            SceneManager.LoadScene(changeScene.changeToScene);
        }
        if (changeScene.musicTrack)
        {
            //SoundManager.Instance.SetMusicTrack(changeScene.musicTrack);
        }
        EndDialogue();
    }

    public void DisplayNextSentence()
    {
        SetNameText(currentDialogue);
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();

        //Changes sprite opacity to match whoever is currently speaking
        if (currentDialogue.rightSideCharacterImage)
        {
            characterImageLeft.color = new Color(255, 255, 255, inactiveTalkerAlpha);
            currentCharacterImage = characterImageRight;
        }
        else if (currentDialogue.leftSideCharacterImage)
        {
            characterImageRight.color = new Color(255, 255, 255, inactiveTalkerAlpha);
            currentCharacterImage = characterImageLeft;
        }
        else
        {
            characterImageLeft.color = new Color(255, 255, 255, inactiveTalkerAlpha);
            characterImageRight.color = new Color(255, 255, 255, inactiveTalkerAlpha);
            currentCharacterImage = null;
        }

        if (currentCharacterImage)
            currentCharacterImage.color = new Color(255, 255, 255, 1);

        if (characterImages.Count > 0)
        {
            Sprite image = characterImages.Dequeue();
            if (image != null)
            {
                currentCharacterImage.enabled = true;
                currentCharacterImage.sprite = image;
            }
            else
                ClearCharacterImage(currentCharacterImage);
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    //Incrementally causes the dialogue to appear in the dialogue box
    IEnumerator TypeSentence(string sentence)
    {
        currentSentence = sentence;
        currentTextTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            dialogueAudioSource.Play();
            yield return new WaitForSeconds(timeBetweenLetterTyping);
        }
        currentTextTyping = false;
    }

    //Foregoes the dialogue typing to show the entire dialogue sentence
    private void FinishTypingSentence()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        dialogueText.text = currentSentence;
        currentTextTyping = false;
    }

    //Performs cleanup from displaying dialogue and tries to dequeue the next conversation node
    public void EndDialogue()
    {
        if (currentCharacterImage != null)
            currentCharacterImage.color = new Color(255, 255, 255, inactiveTalkerAlpha);

        SetNameText();
        StartCoroutine(TypeSentence(""));

        if (conversationNodes.TryDequeue(out ConversationNode nextNode))
        {
            StartDialogue(nextNode);
        }
        else
        {
            EndConversation();
        }
    }

    private void EndConversation()
    {
        dialogueAnimator.SetTrigger("endConversation"); //Another animator trigger
        darkBackground.enabled = false;
        StartCoroutine(DialogueCooldown());
    }

    //Necessary grace period so that the input the ends the conversation does not immediately start it again
    private IEnumerator DialogueCooldown()
    {
        yield return new WaitForEndOfFrame();
        inConversation = false;
    }
}
