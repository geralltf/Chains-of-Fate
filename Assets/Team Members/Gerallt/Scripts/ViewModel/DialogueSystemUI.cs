using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ChainsOfFate.Gerallt
{
    public class DialogueSystemUI : SingletonBase<DialogueSystemUI>
    {
        [SerializeField] private GameObject view;
        [SerializeField] private Button buttonAddPartyMember;
        [SerializeField] private Button buttonInterruptConversation;
        [SerializeField] private Button buttonAdvanceConversation;
        [SerializeField] private Button buttonTestPopulateOptionsView;
        [SerializeField] private TextMeshProUGUI textCharacterName;
        [SerializeField] private TextMeshProUGUI textTalkerLine;
        [SerializeField] private GameObject viewButtonOptions;
        [SerializeField] private GameObject optionButtonPrefab;
        
        public float openAllowTime = 1.0f;
        public float closeAllowTime = 2.0f;
        public CharacterBase talkingToCharacter;

        private DialogueUI dialogueUI;
        private YarnInteractable yarnInteractable;
        private Champion player;
        private DialogueOption[] dialogueOptionsTmp;

        private Action lastDialogueLineOnFinished;

        public void Show(CharacterBase talkingTo)
        {
            talkingToCharacter = talkingTo;
            player = GameManager.Instance.GetPlayer();

            if (dialogueUI == null)
            {
                dialogueUI = FindObjectOfType<DialogueUI>();
            }
            
            yarnInteractable = talkingTo.GetComponent<YarnInteractable>();
            
            SetVisibility(true);
            
            yarnInteractable.StartConversation();
        }

        public void Hide()
        {
            yarnInteractable.EndConversation();
            
            SetVisibility(false);
        }
        
        public void SetVisibility(bool visibility)
        {
            if (visibility)
            {
                bool showButton = false;
                
                if (talkingToCharacter is Champion)
                {
                    if (!player.partyMembers.Contains(talkingToCharacter as Champion))
                    {
                        showButton = true;
                    }
                }
                
                buttonAddPartyMember.gameObject.SetActive(showButton);
            }
            
            view.SetActive(visibility);
        }

        public void AddPartyMember()
        {
            Debug.Log("Add Party Member");
            
            if (talkingToCharacter is Champion)
            {
                buttonAddPartyMember.gameObject.SetActive(false);
                
                FriendlyNPC friendlyNpc = talkingToCharacter.GetComponent<FriendlyNPC>();

                friendlyNpc.AddAsPartyMember(player);
            }
        }
        
        public void DialogueStarted()
        {
            Debug.Log("DialogueUI.DialogueStarted(): ");
        }

        public void DialogueComplete()
        {
            Debug.Log("DialogueUI.DialogueComplete(): ");
        }

        public void UserRequestedViewAdvancement()
        {
            Debug.Log("DialogueUI.UserRequestedViewAdvancement(): ");
        }
        
        public void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Debug.Log("DialogueUI.RunLine(): ");
            
            Debug.Log(dialogueLine.CharacterName + " says: " + dialogueLine.RawText);

            textCharacterName.text = dialogueLine.CharacterName;
            textTalkerLine.text = dialogueLine.TextWithoutCharacterName.Text;
            
            // Tell YarnSpinner that the dialogue has finished.
            //onDialogueLineFinished?.Invoke();

            lastDialogueLineOnFinished = onDialogueLineFinished; // Defer line finishing until player presses 'Next' button "Advance Line"
        }

        public void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Debug.Log("DialogueUI.InterruptLine(): ");
        }

        public void DismissLine(Action onDismissalComplete)
        {
            Debug.Log("DialogueUI.DismissLine(): ");

            //textTalkerLine.text = string.Empty;
        }

        public void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            Debug.Log("DialogueUI.RunOptions(): ");

            PopulateView(dialogueOptions, onOptionSelected);

            dialogueOptionsTmp = dialogueOptions; // Store temporarily for testing
        }

        public void ClearView()
        {
            // Clear dialogue button options view.
            for (int i = 0; i < viewButtonOptions.transform.childCount; i++)
            {
                Transform child = viewButtonOptions.transform.GetChild(i);
                
                Destroy(child.gameObject);
            }
        }
        
        public void PopulateView(DialogueOption[] dialogueOptions, [CanBeNull] Action<int> onOptionSelected)
        {
            ClearView();
            
            // Display dialogue options as buttons on screen. Make them not interactable if they aren't available. (I think that's how that works) 
            foreach (DialogueOption dialogueOption in dialogueOptions)
            {
                GameObject optionButtonInstance = Instantiate(optionButtonPrefab, viewButtonOptions.transform);

                Button optionButton = optionButtonInstance.GetComponent<Button>();
                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = dialogueOption.Line.TextWithoutCharacterName.Text;

                optionButton.interactable = dialogueOption.IsAvailable;
                optionButton.onClick.AddListener(() =>
                {
                    // Tell Yarn to select the specified dialogue option.
                    onOptionSelected(dialogueOption.DialogueOptionID);
                    
                    if (dialogueOption.TextID == "line:Assets/Dialogue/COF-Maria.yarn-MariaStart-6") // HACK: Yarn needs to provide a better way to identify options by name or ID. So if script line numbers change the code doesn't have to
                    {
                        AddPartyMember();
                    }
                    
                    if (dialogueOption.TextID == "line:Assets/Dialogue/COF-Bann'jo.yarn-BannjoStart-13") // HACK: Yarn needs to provide a better way to identify options by name or ID. So if script line numbers change the code doesn't have to
                    {
                        AddPartyMember();
                    }
                    
                    ClearView();
                });
            }
        }
        
        public void InterruptLine()
        {
            dialogueUI.InterruptConversation();
        }

        public void NextLine()
        {
            lastDialogueLineOnFinished?.Invoke();
        }

        public void PopulateOptionsView_OnClick()
        {
            if (dialogueOptionsTmp != null)
            {
                PopulateView(dialogueOptionsTmp, null);
            }
        }

        public override void Awake()
        {
            base.Awake();

            SetVisibility(false);
            
            buttonAddPartyMember.onClick.AddListener(AddPartyMember);
            buttonInterruptConversation.onClick.AddListener(InterruptLine);
            buttonAdvanceConversation.onClick.AddListener(NextLine);
            buttonTestPopulateOptionsView.onClick.AddListener(PopulateOptionsView_OnClick);
        }
    }
}