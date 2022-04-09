using System.Collections;
using System.Collections.Generic;
using ChainsOfFate.Gerallt;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class Champion : PlayableCharacter, IFleeAction, IAttackAction, IResolveAction, IDefendAction
    {
        /// <summary>
        /// Defend against the specified attacker.
        /// </summary>
        /// <param name="attacker">
        /// The attacker to defend against.
        /// </param>
        public void Defend()
        {
            Debug.Log("Test defend action");
        }
        
        /// <summary>
        /// Checks if the champion can flee by doing a random dice roll.
        /// </summary>
        /// <returns>
        /// True, if allowed to flee.
        /// </returns>
        public bool Flee()
        {
            Debug.Log("Test flee action");

            return Random.value > 0.5f;
        }

        public void Attack(CharacterBase target) // TODO: need to pass in equipped weapon WeaponBase
        {
            Debug.Log("Test attack action");
        }

        public void Encourage(CharacterBase targetCharacter)
        {
            Debug.Log("Test encourage action");
        }

        public void Taunt(CharacterBase targetCharacter)
        {
            Debug.Log("Test taunt action");
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