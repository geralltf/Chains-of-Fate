using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsDefensiveSet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;

        public void DefendButton_OnClick()
        {
            // var combatGameManager = PlayerButtonsParentView.combatGameManager;
            // CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            //
            // // Show Quick Time Event to determine block percentage.
            // var blockBarUI = PlayerButtonsParentView.parentView.blockBarUI;
            // blockBarUI.defendingCharacter = currentCharacter;
            // blockBarUI.SetVisibility(true);
        }

        // private void BlockBarUI_OnWonEvent(float blockPercentage, bool doCounterAttack)
        // {
        //     var blockBarUI = PlayerButtonsParentView.parentView.blockBarUI;
        //
        //     IDefendAction defendAction = (IDefendAction)blockBarUI.defendingCharacter;
        //     //defendAction?.Defend(blockPercentage);
        //     
        //     // TODO: Schedule a counter attack in some sort of 'moves stack to apply'
        //     
        //     StartCoroutine(CompleteTurnSequence());
        // }
        
        // private void BlockBarUI_OnLostEvent()
        // {
        //     // Player lost the QTE so doesn't get a block percentage.
        //     StartCoroutine(CompleteTurnSequence());
        // }

        // private IEnumerator CompleteTurnSequence()
        // {
        //     var blockBarUI = PlayerButtonsParentView.parentView.blockBarUI;
        //     var currentCharacter = blockBarUI.defendingCharacter;
        //     var combatGameManager = PlayerButtonsParentView.parentView.combatGameManager;
        //     
        //     blockBarUI.SetVisibility(false);
        //     yield return new WaitForSeconds(1.0f);
        //     
        //     combatGameManager.FinishedTurn(currentCharacter);
        //     combatGameManager.RaiseDefendEvent(currentCharacter, null);
        // }

        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }
        
        public void OnEnable()
        {
            // var blockBarUI = PlayerButtonsParentView.parentView.blockBarUI;
            // blockBarUI.onWonEvent += BlockBarUI_OnWonEvent;
            // blockBarUI.onLostEvent += BlockBarUI_OnLostEvent;
        }

        public void OnDisable()
        {
            // var blockBarUI = PlayerButtonsParentView.parentView.blockBarUI;
            // blockBarUI.onWonEvent -= BlockBarUI_OnWonEvent;
            // blockBarUI.onLostEvent -= BlockBarUI_OnLostEvent;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }

}