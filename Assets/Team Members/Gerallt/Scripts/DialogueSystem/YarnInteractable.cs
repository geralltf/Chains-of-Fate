using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.Events;

public class YarnInteractable : MonoBehaviour
{
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;

    // internal properties not exposed to editor
    [SerializeField] private DialogueRunner dialogueRunner;
    public bool resetDialogWhenComplete = true;
    public bool interactable = true;
    private bool isCurrentConversation = false;
    public bool IsDialogueRunning => dialogueRunner.IsDialogueRunning;
    public UnityEvent DialogueOutcomes;

    public void StartConversation()
    {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;

        dialogueRunner.StartDialogue(dialogueRunner.startNode);
    }

    public void EndConversation()
    {
        if (isCurrentConversation)
        {
            isCurrentConversation = false;
            Debug.Log($"Ended conversation with {name}.");

            dialogueRunner.Stop();
        }
    }

    public void OnViewRequestedInterrupt()
    {
        dialogueRunner.OnViewRequestedInterrupt();
    }

//    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
    
    private void NodeStart(string nodeName)
    {
        //DialogueSystemUI.Instance.NodeStart(nodeName);
        DialogueOutcomes.Invoke();
    }
    
    private void Start()
    {
        if (dialogueRunner == null)
        {
            dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        }
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        dialogueRunner.onNodeStart.AddListener(NodeStart);
    }
}