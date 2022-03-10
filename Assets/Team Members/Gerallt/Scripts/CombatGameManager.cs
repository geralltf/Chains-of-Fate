using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ChainsOfFate.Gerallt
{
    public class CombatGameManager : MonoBehaviour
    {
        public CharacterBase activeCharacter;
        public int currentIndex = 0;
        public List<CharacterBase> combatCharactersList;

        public void HaveTurn()
        {
            CharacterBase currentCharacter = activeCharacter;

            if (currentCharacter != null)
            {
                currentCharacter.HaveTurn();
            }
        }

        public CharacterBase GetCurrentCharacter()
        {
            if (combatCharactersList.Count == 0)
            {
                return null;
            }
            
            return combatCharactersList[currentIndex];
        }
        
        public CharacterBase GetNextCharacter()
        {
            if (combatCharactersList.Count == 0)
            {
                return null;
            }
            
            currentIndex++;

            if (currentIndex >= combatCharactersList.Count)
            {
                // If characterIndex is advanced past the last item in the list then set it to the first item in the list.
                currentIndex = 0;
            }

            return GetCurrentCharacter();
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