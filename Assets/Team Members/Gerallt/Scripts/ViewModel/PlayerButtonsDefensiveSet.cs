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
            var combatGameManager = PlayerButtonsParentView.combatGameManager;
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IDefendAction defendAction = (IDefendAction)currentCharacter;

            if (defendAction != null)
            {
                defendAction.Defend();
                
                combatGameManager.FinishedTurn(currentCharacter);
                combatGameManager.RaiseDefendEvent(currentCharacter, null);
            }
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }
        
        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
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