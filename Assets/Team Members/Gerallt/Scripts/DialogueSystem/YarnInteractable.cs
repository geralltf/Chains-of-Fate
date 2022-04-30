using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnInteractable : MonoBehaviour
{
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private Light lightIndicatorObject = null;
    private bool interactable = true;
    private bool isCurrentConversation = false;

    public bool IsDialogueRunning => dialogueRunner.IsDialogueRunning;

    public void StartConversation()
    {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;

        dialogueRunner.StartDialogue(conversationStartNode);
    }

    public void EndConversation()
    {
        if (isCurrentConversation)
        {
            isCurrentConversation = false;
            Debug.Log($"Started conversation with {name}.");
        }
    }

//    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
    
    private void Start()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
    }
}