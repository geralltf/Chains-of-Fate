using UnityEngine;
using UnityEngine.UI;

namespace ChainsOfFate.Gerallt
{
    public class DialogueSystemUI : SingletonBase<DialogueSystemUI>
    {
        [SerializeField] private GameObject view;
        [SerializeField] private Button buttonAddPartyMember;
        
        public float openAllowTime = 1.0f;
        public float closeAllowTime = 2.0f;
        public CharacterBase talkingToCharacter;

        private Champion player;
        
        public void Show(CharacterBase talkingTo)
        {
            talkingToCharacter = talkingTo;
            player = GameManager.Instance.GetPlayer();
            
            SetVisibility(true);
        }

        public void Hide()
        {
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
            if (talkingToCharacter is Champion)
            {
                buttonAddPartyMember.gameObject.SetActive(false);
                
                FriendlyNPC friendlyNpc = talkingToCharacter.GetComponent<FriendlyNPC>();

                friendlyNpc.AddAsPartyMember(player);
            }
        }
        
        public override void Awake()
        {
            base.Awake();

            SetVisibility(false);
            
            buttonAddPartyMember.onClick.AddListener(AddPartyMember);
        }
    }
}